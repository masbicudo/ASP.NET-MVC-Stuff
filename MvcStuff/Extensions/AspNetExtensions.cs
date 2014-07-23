using System;
using System.Linq;
using System.Reflection;
using System.Web;
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
        public static bool AcceptsJsonResponse(this HttpRequestBase request)
        {
            return request.Headers["Accept"].EnumerableSplit(',')
               .Any(t => t.Equals("application/json", StringComparison.OrdinalIgnoreCase));
        }
    }
}
