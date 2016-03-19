using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcStuff
{
    /// <summary>
    /// HttpModule used to handle Unauthorized (401) status code.
    /// It helps you hide it from unauthenticated users,
    /// and disables the behaviour of redirecting to the Login page in an Ajax request.
    /// </summary>
    public class CustomAuthorizationModule :
        IHttpModule
    {
        // this came from MVCStuff project

        class NamedObject
        {
            public string Name { get; set; }
            public override string ToString() => this.Name;
        }

        private static readonly object previousStatusCodeKey = new NamedObject { Name = "previousStatusCodeKey" };

        void IHttpModule.Init(HttpApplication context)
        {
            context.PostReleaseRequestState += OnPostReleaseRequestState;
            context.EndRequest += OnEndRequest;
        }

        void IHttpModule.Dispose()
        {
        }

        private static void OnPostReleaseRequestState(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var response = context.Response;

            context.Context.Items[previousStatusCodeKey] = response.StatusCode;
        }

        private static void OnEndRequest(object source, EventArgs args)
        {
            var context = (HttpApplication)source;
            var request = new HttpRequestWrapper(context.Request);
            var response = context.Response;

            // when TrySkipIisCustomErrors is set, it means that a custom error is already set,
            // and also that nor we nor IIS should change the custom error
            if (response.TrySkipIisCustomErrors)
                return;

            var prevStatusCode = (int?)context.Context.Items[previousStatusCodeKey];
            const int unauthorized = (int)HttpStatusCode.Unauthorized; // 401
            if (prevStatusCode == unauthorized || response.StatusCode == unauthorized)
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
                        // let IIS redirect the user to the Login page in this case
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
            KnownUnauthorizedAjaxStatusCode = HttpStatusCode.Unauthorized;
        }

        /// <summary>
        /// Gets or sets the status code that should be used when
        /// the user is not authenticated, and tries to access a resource
        /// that resulted in Unauthorized (401) status code.
        /// </summary>
        public static HttpStatusCode? UnknownUserStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status code that should be used when
        /// the user is authenticated, and tries to access a resource
        /// that resulted in Unauthorized (401) status code.
        /// </summary>
        public static HttpStatusCode? KnownUnauthorizedUserStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status code that should be used when
        /// the user is authenticated, and tries to access a resource
        /// that resulted in Unauthorized (401) status code,
        /// in the context of an Ajax request.
        /// </summary>
        public static HttpStatusCode? KnownUnauthorizedAjaxStatusCode { get; set; }
    }
}
