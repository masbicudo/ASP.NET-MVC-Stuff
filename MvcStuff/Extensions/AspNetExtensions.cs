using System.Web;

namespace MvcStuff
{
    public static class AspNetExtensions
    {
        public static bool TrySetSuppressFormsAuthenticationRedirect(this HttpResponseBase response, bool value)
        {
            var result = AspNetInfo.HasSuppressFormsAuthenticationRedirect;
            if (result)
                ((dynamic)response).SuppressFormsAuthenticationRedirect = value;
            return result;
        }
    }
}
