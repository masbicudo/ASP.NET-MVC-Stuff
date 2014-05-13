using System;
using System.Web.Mvc;

namespace MvcStuff.ActionResults
{
    /// <summary>
    /// Wraps an action result and extends it with a call to `Response.End()`.
    /// </summary>
    public class EndRequestWrapperResult : ActionResult
    {
        private readonly ActionResult _innerResult;
        private readonly Predicate<ControllerContext> _conditionToEnd;

        public EndRequestWrapperResult(ActionResult innerResult, Predicate<ControllerContext> conditionToEnd)
        {
            this._innerResult = innerResult;
            this._conditionToEnd = conditionToEnd;
        }

        public EndRequestWrapperResult(ActionResult innerResult)
        {
            this._innerResult = innerResult;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            this._innerResult.ExecuteResult(context);
            if (this._conditionToEnd == null || this._conditionToEnd(context))
                context.HttpContext.Response.End();
        }
    }
}