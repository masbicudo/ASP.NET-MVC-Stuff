using System;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace MvcStuff.Extensions
{
    /// <summary>
    /// Extensions for the UrlHelper class.
    /// </summary>
    public static class UrlExtensions
    {
        // todo: better way to decide whether https should be used... `!IsDebug` is not a good way to determine this

        public static string ActionAbsolute(this UrlHelper @this, [AspMvcAction] string actionName, [AspMvcController] string controllerName, object routeValues)
        {
            var isHttps = !DebugInfo.IsDebug || @this.RequestContext.HttpContext.Request.IsSecureConnection;
            var uriBuilder = new UriBuilder(@this.Action(actionName, controllerName, routeValues, isHttps ? "https" : "http"));
            ChangeUrlParams(uriBuilder);
            return uriBuilder.ToString();
        }

        public static string ActionAbsolute(this UrlHelper @this, [AspMvcAction] string actionName, object routeValues)
        {
            var isHttps = !DebugInfo.IsDebug || @this.RequestContext.HttpContext.Request.IsSecureConnection;
            var uriBuilder = new UriBuilder(@this.Action(actionName, "" + @this.RequestContext.RouteData.Values["controller"], routeValues, isHttps ? "https" : "http"));
            ChangeUrlParams(uriBuilder);
            return uriBuilder.ToString();
        }

        public static string ActionAbsolute(this UrlHelper @this, [AspMvcAction] string actionName, [AspMvcController] string controllerName)
        {
            var isHttps = !DebugInfo.IsDebug || @this.RequestContext.HttpContext.Request.IsSecureConnection;
            var uriBuilder = new UriBuilder(@this.Action(actionName, controllerName, new { }, isHttps ? "https" : "http"));
            ChangeUrlParams(uriBuilder);
            return uriBuilder.ToString();
        }

        public static string ActionAbsolute(this UrlHelper @this, [AspMvcAction] string actionName)
        {
            var isHttps = !DebugInfo.IsDebug || @this.RequestContext.HttpContext.Request.IsSecureConnection;
            var uriBuilder = new UriBuilder(@this.Action(actionName, "" + @this.RequestContext.RouteData.Values["controller"], new { }, isHttps ? "https" : "http"));
            ChangeUrlParams(uriBuilder);
            return uriBuilder.ToString();
        }

        private static void ChangeUrlParams(UriBuilder uriBuilder)
        {
#if DEBUG
            // todo: these ports should be configurable in a per-application basis... e.g. global.asax or web.config
            if (!DebugInfo.NoHttps)
            {
                if (DebugInfo.HostEnvironment == HostingServer.IisExpress)
                {
                    uriBuilder.Scheme = "https";
                    uriBuilder.Port = 44300;
                }
                else if (DebugInfo.HostEnvironment == HostingServer.Iis)
                {
                    uriBuilder.Scheme = "https";
                    uriBuilder.Port = /*RoleEnvironment.IsAvailable
                        ? RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["HttpsIn"].IPEndpoint.Port
                        :*/ 443;
                }

                // WebDev server does not support HTTPS... no redirects to HTTPS will happen.
            }
            else
            {
                if (DebugInfo.HostEnvironment == HostingServer.IisExpress)
                {
                    uriBuilder.Scheme = "http";
                    uriBuilder.Port = 12621;
                }
                else if (DebugInfo.HostEnvironment == HostingServer.Iis)
                {
                    uriBuilder.Scheme = "http";
                    uriBuilder.Port = /*RoleEnvironment.IsAvailable
                        ? RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["HttpIn"].IPEndpoint.Port
                        :*/ 80;
                }
            }
#endif
        }
    }
}