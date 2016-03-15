using System;
using System.Web.Mvc;
using JetBrains.Annotations;

namespace MvcStuff
{
    public static class DataViewResultControllerExtensions
    {
        /// <summary>
        /// Renders the given data to the most appropriate format,
        /// by examining the accepted mime types of the request.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        [AspMvcView]
        public static DataViewResult DataView([NotNull] this Controller controller, object data)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            return new DataViewResult
            {
                Data = data,
            };
        }

        public static DataViewResult DataView([NotNull] this Controller controller, [AspMvcView] string viewName, object data)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            return new DataViewResult
            {
                ViewName = viewName,
                Data = data,
            };
        }
    }
}