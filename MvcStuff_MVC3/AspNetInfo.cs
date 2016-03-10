using System.Linq;
using System.Web;

namespace MvcStuff
{
    public static class AspNetInfo
    {
        private static readonly bool hasSuppressFormsAuthenticationRedirect
            = typeof(HttpContextBase).GetProperties().Any(x => x.Name == "SuppressFormsAuthenticationRedirect");

        public static bool HasSuppressFormsAuthenticationRedirect
        {
            get { return hasSuppressFormsAuthenticationRedirect; }
        }
    }
}
