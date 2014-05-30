using System.Net;
using System.Reflection;
using System.Web;

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
        /// and from IIS (Iis Custom Errors).
        /// </summary>
        public static void EndWithoutInterference(this HttpResponseBase response)
        {
            var statusCode = response.StatusCode;

            response.TrySkipIisCustomErrors = true;

            response.TrySetSuppressFormsAuthenticationRedirect(true);

            if (response is HttpResponseWrapper)
            {
                // THIS IS A HACK, IT IS NOT FUTURE PROOF
                // When a response is cancelable, a call to response.End will finish it.
                // Although, this won't happen if it's not cancelable, and execution will
                // continue after the response.End call, and also, Custom Errors handler
                // will come into action inside the End method!!! It'll replace the page
                // content, based on the StatusCode, that why we fool it by setting
                // StatusCode temporarilly to 200, and set it back after the End call.
                var isCancelable = (bool)typeof(HttpContext).GetProperty(
                    "IsInCancellablePeriod",
                    BindingFlags.NonPublic | BindingFlags.Static).GetValue(HttpContext.Current, null);

                response.StatusCode = isCancelable
                    ? (int)statusCode
                    : (response.StatusCode >= 400 ? 200 : response.StatusCode);
            }
            else
                response.StatusCode = (int)statusCode;

            response.End();

            if (response is HttpResponseWrapper)
                response.StatusCode = (int)statusCode;
        }
    }
}
