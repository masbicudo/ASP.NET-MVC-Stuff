using System.Web;

namespace MvcStuff.Mocks
{
    public class MockHttpResponse : HttpResponseBase
    {
        private HttpCookieCollection _cookies = new HttpCookieCollection();
        private HttpCachePolicyBase _cachePolicy;

        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }

        public override HttpCookieCollection Cookies
        {
            get { return this._cookies ?? (this._cookies = new HttpCookieCollection()); }
        }

        public override HttpCachePolicyBase Cache
        {
            get { return this._cachePolicy ?? (this._cachePolicy = new MockHttpCachePolicy()); }
        }
    }
}