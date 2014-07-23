using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MvcStuff
{
    // TODO: use ConcurrentDictionary for a faster global cache, while monitoring the Views folder for changes with FileSystemWatcher
    public class TwoLevelViewCache : IViewLocationCache
    {
        // see: http://blogs.msdn.com/b/marcinon/archive/2011/08/16/optimizing-mvc-view-lookup-performance.aspx
        private static readonly CustomKey TwoLevelViewCacheKey = new CustomKey("TwoLevelViewCache");
        private readonly IViewLocationCache inner;

        public TwoLevelViewCache(IViewLocationCache cache)
        {
            this.inner = cache;
        }

        private static IDictionary<string, string> GetRequestCache(HttpContextBase httpContext)
        {
            var d = httpContext.Items[TwoLevelViewCacheKey] as IDictionary<string, string>;
            if (d == null)
            {
                d = new Dictionary<string, string>();
                httpContext.Items[TwoLevelViewCacheKey] = d;
            }

            return d;
        }

        public string GetViewLocation(HttpContextBase httpContext, string key)
        {
            var d = GetRequestCache(httpContext);
            string location;
            if (!d.TryGetValue(key, out location))
            {
                location = this.inner.GetViewLocation(httpContext, key);
                d[key] = location;
            }

            return location;
        }

        public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
        {
            this.inner.InsertViewLocation(httpContext, key, virtualPath);
        }
    }
}