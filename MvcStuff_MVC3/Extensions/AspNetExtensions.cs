using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using JetBrains.Annotations;
using MvcStuff.SystemExtensions;

namespace MvcStuff
{
    public static class AspNetExtensions
    {
        /// <summary>
        /// SuppressFormsAuthenticationRedirect is only available in ASP.NET 4.5.
        /// To make code compatible with multiple versions of ASP.NET, this method
        /// will set the property if it exists and return true, otherwise return false,
        /// and let the caller decide what work-around to use.
        /// </summary>
        public static bool TrySetSuppressFormsAuthenticationRedirect(this HttpResponseBase response, bool value)
        {
            var result = AspNetInfo.HasSuppressFormsAuthenticationRedirect;
            if (result)
                ((dynamic)response).SuppressFormsAuthenticationRedirect = value;
            return result;
        }

        /// <summary>
        /// Ends a request without interference from ASP.NET handlers (Forms Authentication and Custom Errors),
        /// and from IIS (IIS Custom Errors).
        /// </summary>
        public static void EndWithoutInterference(this HttpResponseBase response)
        {
            var statusCode = response.StatusCode;

            response.TrySkipIisCustomErrors = true;

            response.TrySetSuppressFormsAuthenticationRedirect(true);

            if (response is HttpResponseWrapper)
            {
                // THIS IS A HACK, IT IS NOT FUTURE PROOF
                // When a response is "cancellable", a call to response.End will finish it.
                // Although, this won't happen if it's "not cancellable", and execution will
                // continue after the response.End call, and also, Custom Errors handler
                // will come into action inside the End method!!! It'll replace the page
                // content, based on the StatusCode, thats why we fool it by setting
                // StatusCode temporarilly to 200, and set it back after the End call.
                #region pt-BR
                // ISSO É UMA GAMBIARRA
                // Quando um requests está no estado "cancelável", o efeito do método `response.End()` é um término brusco.
                // Caso contrário, o método `response.End()` não termina, e o código conteinua executando,
                // sendo que os Handlers de Erros Customizados vão ser renderizados dentro da chamada do `End()`,
                // o que por si só já é bizarro. Ao fazer isso, substitui-se o conteúdo já renderizado,
                // pelo conteúdo do handler de erro.
                // Por isso tudo, vamos enganar o ASP.NET, setando o status code temporariamente para 200,
                // e retornando o valor original após a chamada do `End()`.
                #endregion
                var isCancellableProperty =
                    typeof(HttpContext).GetProperty(
                        "IsInCancellablePeriod",
                        BindingFlags.NonPublic | BindingFlags.Static);
                var isCancellable = (bool)isCancellableProperty.GetValue(HttpContext.Current, null);

                response.StatusCode = isCancellable
                    ? (int)statusCode
                    : (response.StatusCode >= 400 ? 200 : response.StatusCode);
            }
            else
                response.StatusCode = (int)statusCode;

            response.End();

            if (response is HttpResponseWrapper)
                response.StatusCode = (int)statusCode;
        }

        /// <summary>
        /// Returns a value indicating whether this requests accepts a JSON response.
        /// </summary>
        /// <param name="request">Request to check for JSON response acceptance.</param>
        /// <returns>True if the request accepts a JSON response.</returns>
        public static bool IsJsonRequest(this HttpRequestBase request)
        {
            var acc0 = request.AcceptedMimesInOrder().First();
            var isJson = acc0.Contains("application/json", StringComparer.InvariantCultureIgnoreCase)
                || acc0.Contains("text/x-json", StringComparer.InvariantCultureIgnoreCase);
            return isJson;
        }

        /// <summary>
        /// Returns a value indicating whether this requests accepts an XML response, that is not XHTML.
        /// </summary>
        /// <param name="request">Request to check for XML response acceptance.</param>
        /// <returns>True if the request accepts an XML response.</returns>
        public static bool IsXmlRequest(this HttpRequestBase request)
        {
            var acc0 = request.AcceptedMimesInOrder().First();
            var isXml = acc0.Contains("application/xml", StringComparer.InvariantCultureIgnoreCase)
                && !acc0.Contains("application/xhtml", StringComparer.InvariantCultureIgnoreCase)
                && !acc0.Contains("application/xhtml+xml", StringComparer.InvariantCultureIgnoreCase);
            return isXml;
        }

        /// <summary>
        /// Returns a value indicating whether this requests accepts an HTML or an XHTML response.
        /// </summary>
        /// <param name="request">Request to check for HTML response acceptance.</param>
        /// <returns>True if the request accepts an HTML response.</returns>
        public static bool IsHtmlRequest(this HttpRequestBase request)
        {
            var acc0 = request.AcceptedMimesInOrder().First() ?? new string[0];
            var isHtml = acc0.Contains("text/html", StringComparer.InvariantCultureIgnoreCase)
                         || acc0.Contains("application/xhtml", StringComparer.InvariantCultureIgnoreCase)
                         || acc0.Contains("application/xhtml+xml", StringComparer.InvariantCultureIgnoreCase)
                         || acc0.Contains("*/*", StringComparer.InvariantCulture);

            return isHtml;
        }

        /// <summary>
        /// Gets the best negotiated response types, according to request accept header and the given response types.
        /// </summary>
        /// <param name="request">Request to get accept header from.</param>
        /// <param name="responseTypes">Available response types in the same format as the accept header.</param>
        /// <returns>The best negotiated mime types for the given request response.</returns>
        public static string[] GetBestResponseMimeTypes(
            [NotNull] this HttpRequestBase request,
            [NotNull] string responseTypes)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (responseTypes == null) throw new ArgumentNullException(nameof(responseTypes));

            var best = GetMimeTypesNegotiatedScores(request, responseTypes)
                .GroupBy(x => x.Value, x => x.Key)
                .OrderByDescending(g => g.Key)
                .Select(g => g.ToArray())
                .FirstOrDefault();

            return best;
        }

        private static IEnumerable<KeyValuePair<string, double>> GetMimeTypesNegotiatedScores(
            HttpRequestBase request,
            string responseTypes)
        {
            var accepted = AcceptedMimesAndQualities(request.Headers["Accept"]);

            var available = AcceptedMimesAndQualities(responseTypes)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var accKv in accepted)
            {
                double avQ;
                if (available.TryGetValue(accKv.Key, out avQ))
                {
                    var combinedQuality = avQ * accKv.Value;
                    yield return new KeyValuePair<string, double>(accKv.Key, combinedQuality);
                }
            }
        }

        /// <summary>
        /// Returns a list of Mime type groups ordered by requester preference.
        /// </summary>
        /// <param name="request">Request to get accept header from.</param>
        /// <returns>Groups of Mime types ordered by requester preference.</returns>
        public static string[][] AcceptedMimesInOrder([NotNull] this HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var strMimes = request.Headers["Accept"];

            var result = AcceptedMimesAndQualities(strMimes)
                .GroupBy(x => x.Value, x => x.Key)
                .OrderByDescending(g => g.Key)
                .Select(g => g.ToArray());

            return result.ToArray();
        }

        /// <summary>
        /// Returns a dictionary of mime types and their associated quality values.
        /// </summary>
        /// <param name="request">Request to get accept header from.</param>
        /// <returns>Dictionary of mime types to quality values.</returns>
        public static Dictionary<string, double> AcceptedMimesQualities([NotNull] this HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var strMimes = request.Headers["Accept"];
            var result = AcceptedMimesAndQualities(strMimes)
                .ToDictionary(x => x.Key, x => x.Value);
            return result;
        }

        private static IEnumerable<KeyValuePair<string, double>> AcceptedMimesAndQualities(
            string strMimes)
        {
            var result = from acceptedItem in strMimes.Split(',')
                         let parts = acceptedItem.Split(';')
                         where parts.Length > 0
                         let values = parts
                             .Select(x => x.Split('='))
                             .Where(x => x.Length == 2)
                         let q = values.Where(x => x[0] == "q").Select(x => x[1]).SingleOrDefault()
                         let quality = q == null ? 1.0 : double.Parse(q, CultureInfo.InvariantCulture)
                         group quality by parts.First() into g
                         select new KeyValuePair<string, double>(g.Key, g.Max());

            return result;
        }
    }
}
