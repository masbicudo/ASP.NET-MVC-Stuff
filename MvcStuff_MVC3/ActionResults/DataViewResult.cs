using System;
using System.Web.Mvc;

namespace MvcStuff
{
    public class DataViewResult : ActionResult
    {
        public object Data { get; set; }

        public string ViewName { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            ActionResult result;
            if (context.HttpContext.Request.IsJsonRequest())
            {
                result = new JsonResult
                    {
                        Data = this.Data,
                        ContentType = null,
                        ContentEncoding = null,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    };
            }
            else if (context.HttpContext.Request.IsHtmlRequest())
            {
                if (this.Data != null)
                    context.Controller.ViewData.Model = this.Data;

                result = new ViewResult
                    {
                        ViewName = this.ViewName,
                        MasterName = null,
                        ViewData = context.Controller.ViewData,
                        TempData = context.Controller.TempData,
#if !ASP_MVC_3
                        ViewEngineCollection = (context.Controller as Controller)?.ViewEngineCollection,
#endif
                    };
            }
            else if (context.HttpContext.Request.IsXmlRequest())
            {
                // If this is ever needed, take a loot at this:
                // http://stackoverflow.com/questions/134905/return-xml-from-a-controllers-action-in-as-an-actionresult
                throw new NotSupportedException("Cannot respond to the request for XML.");
            }
            else
            {
                throw new NotSupportedException("Cannot respond to the request.");
            }

            result.ExecuteResult(context);
        }
    }
}