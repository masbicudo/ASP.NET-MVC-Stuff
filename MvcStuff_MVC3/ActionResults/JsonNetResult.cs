using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;

namespace MvcStuff
{
    public class JsonNetResult : JsonResult
    {
        const string JsonRequest_GetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(JsonRequest_GetNotAllowed);
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            var settings = new JsonSerializerSettings();
            //if (this.MaxJsonLength.HasValue)
            //    scriptSerializer.MaxJsonLength = this.MaxJsonLength.Value;
            if (this.RecursionLimit.HasValue)
                settings.MaxDepth = this.RecursionLimit.Value;

            response.Write(JsonConvert.SerializeObject(this.Data, settings));
        }

#if ASP_MVC_3
        public int? RecursionLimit { get; set; }
#endif
    }
}