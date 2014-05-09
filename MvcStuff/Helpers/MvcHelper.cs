using System.IO;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace MvcStuff.Helpers
{
    /// <summary>
    /// Class containing ASP.NET MVC utilities, to help with rendering views to strings,
    /// mocking Http classes and getting informations about actions.
    /// </summary>
    public static class MvcHelper
    {
        /// <summary>
        /// Renders a partial MVC view to a string.
        /// The view search locations is relative to the ControllerContext.
        /// </summary>
        /// <param name="controllerContext">ControllerContext that is used to locate the view.</param>
        /// <param name="viewName">The name of the partial view to render.</param>
        /// <param name="viewData">The viewData, containing the model object to pass to the partial view.</param>
        /// <returns>The string rendered from the partial view.</returns>
        public static string RenderPartialViewToString(
            ControllerContext controllerContext,
            [AspMvcPartialView] string viewName,
            ViewDataDictionary viewData = null)
        {
            var tempData = new TempDataDictionary();
            var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Renders a partial MVC view to a string.
        /// The view search locations is relative to the ControllerContext.
        /// </summary>
        /// <param name="controllerContext">ControllerContext that is used to locate the view.</param>
        /// <param name="viewName">The name of the partial view to render.</param>
        /// <param name="viewData">The viewData, containing the model object to pass to the partial view.</param>
        /// <param name="masterName">Name of the layout page.</param>
        /// <returns>The string rendered from the partial view.</returns>
        public static string RenderViewToString(
            ControllerContext controllerContext,
            [AspMvcView] string viewName,
            ViewDataDictionary viewData = null,
            [AspMvcMaster]string masterName = "")
        {
            var tempData = new TempDataDictionary();
            var viewResult = ViewEngines.Engines.FindView(controllerContext, viewName, masterName);
            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
