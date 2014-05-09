using System.Collections;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;

namespace MvcStuff.Mocks
{
    public class MockHttpContext : HttpContextBase
    {
        private readonly HttpResponseBase response = new MockHttpResponse();
        private readonly HttpContextBase httpContextBase;
        private Hashtable _items;
        private Cache _cache;


        public MockHttpContext()
        {
        }

        public MockHttpContext(HttpContextBase httpContextBase)
        {
            this.httpContextBase = httpContextBase;
        }




        public HttpRequestBase Request2 { get; set; }

        public override HttpRequestBase Request
        {
            get { return this.Request2; }
        }

        public override HttpResponseBase Response
        {
            get { return this.response; }
        }

        public override IDictionary Items
        {
            get
            {
                if (this._items == null)
                    this._items = new Hashtable();
                return (IDictionary)this._items;
            }
        }

        public override IPrincipal User
        {
            get { return this.User2 ?? httpContextBase.User; }
            set { this.User2 = value; }
        }

        public IPrincipal User2 { get; set; }

        public override Cache Cache
        {
            get
            {
                if (this._cache == null)
                    this._cache = new Cache();
                return (Cache)this._cache;
            }
        }
    }
}