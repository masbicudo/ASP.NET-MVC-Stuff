using JetBrains.Annotations;
using System.Linq;
using System.Web.Mvc;

namespace MvcStuff
{
    public static class MvcAppOptimizer
    {
        public static void OptimizeForRazor([CanBeNull] string razorFileExtension = "cshtml", bool useTwoLevelViewCache = true)
        {
            // see: http://blogs.msdn.com/b/marcinon/archive/2011/08/16/optimizing-mvc-view-lookup-performance.aspx
            // optimizing view-engines lookup time
            // - place Razor view engine first
            // - allow only one type of Razor file extension
            // - use two level view cache
            var firstViewTypes = new[] { typeof(RazorViewEngine) };

            var firstViews = firstViewTypes
                .SelectMany(t => ViewEngines.Engines.Where(t.IsInstanceOfType))
                .ToList();

            var otherViews = ViewEngines.Engines.Except(firstViews);

            var allViews = firstViews.Concat(otherViews).ToList();

            allViews.OfType<RazorViewEngine>().ToList().ForEach(
                ve =>
                {
                    if (razorFileExtension != null)
                        ve.FileExtensions = new[] { razorFileExtension };

                    if (useTwoLevelViewCache
                        && ve.ViewLocationCache != null
                        && !(ve.ViewLocationCache is TwoLevelViewCache))
                    {
                        ve.ViewLocationCache = new TwoLevelViewCache(ve.ViewLocationCache);
                    }
                });

            ViewEngines.Engines.Clear();

            allViews.ForEach(ViewEngines.Engines.Add);
        }
    }
}
