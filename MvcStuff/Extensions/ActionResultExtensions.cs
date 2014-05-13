using System;
using System.Web.Mvc;
using MvcStuff.ActionResults;

namespace MvcStuff.Extensions
{
    public static class ActionResultExtensions
    {
        public static EndRequestWrapperResult ThenEndRequest(this ActionResult result)
        {
            return new EndRequestWrapperResult(result);
        }

        public static EndRequestWrapperResult ThenEndRequest(this ActionResult result, Predicate<ControllerContext> conditionToEnd)
        {
            return new EndRequestWrapperResult(result, conditionToEnd);
        }
    }

}