using System.ComponentModel;
using System.Net;
using System.Web.Mvc;

namespace MvcStuff.ActionResults
{
    /// <summary>
    /// Representa um resultado contendo um código de status 403 (Forbidden).
    /// Esse resultado pode ser somente para requisições JSON ou XML (Ajax).
    /// </summary>
    public class JsonForbiddenResult : JsonResult
    {
        public JsonForbiddenResult()
            : this(null)
        {
        }

        public JsonForbiddenResult([Localizable(true)] string statusDescription)
        {
            this.Data = this.Data ?? new JsonResponseData
            {
                Success = false,
                Message = statusDescription,
                ErrorType = "forbidden",
                Status = (int)HttpStatusCode.Forbidden,
            };

            this.StatusDescription = statusDescription;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            base.ExecuteResult(context);

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            if (StatusDescription != null)
                context.HttpContext.Response.StatusDescription = StatusDescription;
        }

        [Localizable(true)]
        public string StatusDescription { get; private set; }
    }
}