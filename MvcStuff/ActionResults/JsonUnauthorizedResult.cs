using System.ComponentModel;
using System.Net;
using System.Web.Mvc;

namespace MvcStuff
{
    /// <summary>
    /// Representa um resultado contendo um código de status 401 (Unauthorized).
    /// Esse resultado pode ser somente para requisições JSON ou XML (Ajax).
    /// </summary>
    public class JsonUnauthorizedResult : JsonResult
    {
        public JsonUnauthorizedResult()
            : this(null)
        {
        }

        public JsonUnauthorizedResult([Localizable(true)] string statusDescription)
        {
            this.Data = this.Data ?? new JsonResponseData
            {
                Success = false,
                Message = statusDescription,
                ErrorType = "unauthorized",
                Status = (int)HttpStatusCode.Unauthorized,
            };

            this.StatusDescription = statusDescription;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            base.ExecuteResult(context);

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            if (StatusDescription != null)
                context.HttpContext.Response.StatusDescription = StatusDescription;
        }

        [Localizable(true)]
        public string StatusDescription { get; private set; }
    }
}
