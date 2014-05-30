﻿using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MvcStuff
{
    /// <summary>
    /// Representa um resultado contendo um código de status indicado por parâmetro.
    /// Esse resultado pode ser usando tanto para requisições normais,
    /// como para requisições JSON ou XML (Ajax).
    /// </summary>
    public class StatusCodeResult : ActionResult
    {
        public StatusCodeResult(HttpStatusCode statusCode)
            : this(statusCode, null)
        {
        }

        public StatusCodeResult(HttpStatusCode statusCode, [Localizable(true)] string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;

            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            this.Data = this.Data ?? new JsonResponseData
            {
                Success = false,
                Message = statusDescription,
                ErrorType = this.StatusCode.ToString().ToLowerInvariant(),
                Status = (int)statusCode,
            };
        }

        public HttpStatusCode StatusCode { get; private set; }

        [Localizable(true)]
        public string StatusDescription { get; private set; }

        public object Data { get; set; }
        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public Encoding JsonContentEncoding { get; set; }
        public string JsonContentType { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            response.StatusCode = (int)this.StatusCode;
            if (this.StatusDescription != null)
                response.StatusDescription = this.StatusDescription;

            // if it's an Ajax Request, must return a Json
            if (request.IsAjaxRequest())
            {
                // todo: check for json request: Request.AcceptTypes.Contains("application/json")
                // todo: json requests are being made without setting the content-type to "application/json" throughout the application
                // todo: Xhr requests made without "application/json" are meant to be XML, not JSON.

                if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                    String.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Json result via GET request is not allowed.");
                }

                this.JsonResult(response);

                // This is an ajax request, so to prevent:
                //  - FormsAuthentication module redirecting to the login page when the status-code is 401,
                //  - and also Custom Errors handling from rewriting the page contents inside request.End(),
                // we use the special 'EndWithoutInterference', made for this purpose.

                // This response will not be cacheable by using aps.net output-caching, because it
                // will be skipped, just like the forms-auth... and any other handler or module
                // that executes at the end of the request (just for 4xx and 5xx status codes).

                // Resources:
                //  - ResponseEnd + OutputCache: http://www.west-wind.com/weblog/posts/2009/May/21/Dont-use-ResponseEnd-with-OutputCache
                //  - Same solution of this guy: http://stackoverflow.com/questions/2580596/how-do-you-handle-ajax-requests-when-user-is-not-authenticated
                //  - Alternative solutions: http://haacked.com/archive/2011/10/04/prevent-forms-authentication-login-page-redirect-when-you-donrsquot-want.aspx
                //  - Another alternative: disable FormsAuth redirection in web.config, and do redirects manually, i.e. kind of use RedirectResult instead
                //  - ASP.NET 4.5 now supports response.SuppressFormsAuthenticationRedirect
                if (response.StatusCode >= 400)
                    response.EndWithoutInterference();

            }
        }

        protected void JsonResult(HttpResponseBase response)
        {
            response.ContentType = !string.IsNullOrEmpty(this.JsonContentType) ? this.JsonContentType : "application/json";

            if (this.JsonContentEncoding != null)
                response.ContentEncoding = this.JsonContentEncoding;

            if (this.Data != null)
            {
                var serializer = new JavaScriptSerializer();
                response.Write(serializer.Serialize(this.Data));
            }

        }
    }
}