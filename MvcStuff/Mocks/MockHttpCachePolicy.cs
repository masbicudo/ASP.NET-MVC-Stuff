using System;
using System.Web;

namespace MvcStuff.Mocks
{
    public class MockHttpCachePolicy : HttpCachePolicyBase
    {
        public HttpCacheVaryByContentEncodings varyByContentEncodings = new HttpCacheVaryByContentEncodings();
        public HttpCacheVaryByHeaders varyByHeaders = new HttpCacheVaryByHeaders();
        public HttpCacheVaryByParams varyByParams = new HttpCacheVaryByParams();
        
        public override void SetProxyMaxAge(TimeSpan delta)
        {
            this.ProxyMaxAge = delta;
        }

        public TimeSpan? ProxyMaxAge { get; private set; }

        public override void AddValidationCallback(HttpCacheValidateHandler handler, object data)
        {
        }

        public override void AppendCacheExtension(string extension)
        {
        }

        public override void SetAllowResponseInBrowserHistory(bool allow)
        {
        }

        public override void SetCacheability(HttpCacheability cacheability)
        {
        }

        public override void SetCacheability(HttpCacheability cacheability, string field)
        {
        }

        public override void SetETag(string etag)
        {
        }

        public override void SetETagFromFileDependencies()
        {
        }

        public override void SetExpires(DateTime date)
        {
        }

        public override void SetLastModified(DateTime date)
        {
        }

        public override void SetLastModifiedFromFileDependencies()
        {
        }

        public override void SetMaxAge(TimeSpan delta)
        {
        }

        public override void SetNoServerCaching()
        {
        }

        public override void SetNoStore()
        {
        }

        public override void SetNoTransforms()
        {
        }

        public override void SetOmitVaryStar(bool omit)
        {
        }

        public override void SetRevalidation(HttpCacheRevalidation revalidation)
        {
        }

        public override void SetSlidingExpiration(bool slide)
        {
        }

        public override void SetValidUntilExpires(bool validUntilExpires)
        {
        }

        public override void SetVaryByCustom(string custom)
        {
        }

        public override HttpCacheVaryByContentEncodings VaryByContentEncodings
        {
            get
            {
                return this.varyByContentEncodings;
            }
        }

        public override HttpCacheVaryByHeaders VaryByHeaders
        {
            get { return this.varyByHeaders; }
        }

        public override HttpCacheVaryByParams VaryByParams
        {
            get { return this.varyByParams; }
        }
    }
}
