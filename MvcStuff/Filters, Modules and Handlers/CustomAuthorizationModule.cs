using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcStuff.Filters
{
    public class CustomAuthorizationModule : IHttpModule
    {
        private static readonly object PreviousStatusCodeKey = new object();

        void IHttpModule.Init(HttpApplication context)
        {
            context.PostReleaseRequestState += OnPostReleaseRequestState;
            context.EndRequest += OnEndRequest;
        }

        void IHttpModule.Dispose()
        {
        }

        private void OnPostReleaseRequestState(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var response = context.Response;

            context.Context.Items[PreviousStatusCodeKey] = response.StatusCode;
        }

        private void OnEndRequest(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var request = new HttpRequestWrapper(context.Request);
            var response = context.Response;

            // when TrySkipIisCustomErrors is set, it means that a custom error is already set,
            // and also that nor we nor IIS should change the custom error
            if (response.TrySkipIisCustomErrors)
                return;

            var prevStatusCode = (int?)context.Context.Items[PreviousStatusCodeKey];
            if (prevStatusCode == 401 || response.StatusCode == 401)
            {
                if (!request.IsAuthenticated)
                {
                    // non-authenticated user won't even know whether the page exists or not
                    if (UnknownUserStatusCode.HasValue && response.StatusCode != (int)UnknownUserStatusCode.Value)
                    {
                        response.TrySkipIisCustomErrors = false;
                        response.ClearContent();
                        response.StatusCode = (int)UnknownUserStatusCode.Value;
                        response.RedirectLocation = null;
                    }
                }
                else if (request.IsAjaxRequest())
                {
                    // authenticated user with ajax request, will receive the proper 401,
                    // as it was before intervention of ASP.NET
                    // todo: we should copy all the request data from before ASP.NET,
                    // todo: not only status code, and restore all the values
                    if (KnownUnauthorizedAjaxStatusCode.HasValue && response.StatusCode != (int)KnownUnauthorizedAjaxStatusCode.Value)
                    {
                        response.TrySkipIisCustomErrors = true;
                        response.ClearContent();
                        response.StatusCode = (int)KnownUnauthorizedAjaxStatusCode.Value;
                        response.RedirectLocation = null;
                    }
                }
                else
                {
                    // user is authenticated, and this is not an ajax request
                    // redirect user to unauthorized page
                    if (KnownUnauthorizedUserStatusCode.HasValue && response.StatusCode != (int)KnownUnauthorizedUserStatusCode.Value)
                    {
                        response.TrySkipIisCustomErrors = false;
                        response.ClearContent();
                        response.StatusCode = (int)KnownUnauthorizedUserStatusCode.Value;
                        response.RedirectLocation = null;
                    }
                }
            }
        }

        static CustomAuthorizationModule()
        {
            UnknownUserStatusCode = HttpStatusCode.NotFound;
            KnownUnauthorizedUserStatusCode = HttpStatusCode.NotFound;
            KnownUnauthorizedUserStatusCode = HttpStatusCode.Unauthorized;
        }

        public static HttpStatusCode? UnknownUserStatusCode { get; set; }
        public static HttpStatusCode? KnownUnauthorizedUserStatusCode { get; set; }
        public static HttpStatusCode? KnownUnauthorizedAjaxStatusCode { get; set; }
    }
}
