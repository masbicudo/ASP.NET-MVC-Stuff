using System;
using System.Collections.Specialized;
using System.Web;
using MvcStuff.Mocks;

namespace MvcStuff
{
    public class MockHttpRequest : HttpRequestBase,
        IHttpRequestMutable
    {
        /// <summary>
        /// Gets or sets the default Url getter, 
        /// that is used to construct an Url when none is provided to the mock.
        /// </summary>
        public static DefaultUrlGetterFunc DefaultUrlGetter { get; set; }




        private readonly HttpRequestBase httpRequestBase;
        private readonly HttpCookieCollection cookies = new HttpCookieCollection();
        private readonly NameValueCollection serverVars = new NameValueCollection();
        private readonly NameValueCollection headers = new NameValueCollection();
        private readonly NameValueCollection form = new NameValueCollection();
        private readonly NameValueCollection queryStr = new NameValueCollection();




        public MockHttpRequest()
        {
        }

        public MockHttpRequest(HttpRequestBase httpRequestBase)
        {
            this.httpRequestBase = httpRequestBase;
        }




        public override string FilePath
        {
            get
            {
                if (this.Url == null)
                    throw new Exception("Cannot determine the FilePath property, because Url property is null.");

                var path = this.Url.AbsolutePath;
                path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

                var point = path.IndexOf('.');
                var slash = point < 0 ? -1 : path.IndexOf('/', point);
                var result = slash < 0 ? path : path.Substring(0, slash);
                return result;
            }
        }

        public override string PathInfo
        {
            get
            {
                if (this.Url == null)
                    throw new Exception("Cannot determine the FilePath property, because Url property is null.");

                var pathAndQuery = this.Url.AbsolutePath;
                var path = this.Url.AbsolutePath;
                path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

                var point = path.IndexOf('.');
                var slash = point < 0 ? -1 : path.IndexOf('/', point);
                var result = slash < 0 ? pathAndQuery.Substring(path.Length) : pathAndQuery.Substring(slash);
                return result;
            }
        }

        public override string Path
        {
            get
            {
                if (this.Url == null)
                    throw new Exception("Cannot determine the FilePath property, because Url property is null.");

                return this.Url.AbsolutePath;
            }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                if (this.ApplicationPath == null)
                    throw new Exception("Cannot determine the AppRelativeCurrentExecutionFilePath property, because ApplicationPath property is null.");

                var appPath = this.ApplicationPath;
                appPath = appPath.EndsWith("/") ? appPath.Substring(0, appPath.Length - 1) : appPath;

                var filePath = this.FilePath;

                bool isInAppPath = filePath.StartsWith(appPath + '/', StringComparison.InvariantCultureIgnoreCase);
                bool isAppPath = filePath.Equals(appPath, StringComparison.InvariantCultureIgnoreCase);

                if (!isInAppPath && !isAppPath)
                    throw new Exception("Cannot get AppRelativeCurrentExecutionFilePath for FilePath outside the application path.");

                return "~" + filePath;
            }
        }

        public string MutableHttpMethod { get; set; }

        public Uri MutableUrl { get; set; }

        public override string HttpMethod
        {
            get { return this.MutableHttpMethod; }
        }

        public override Uri Url
        {
            get { return this.MutableUrl ?? (DefaultUrlGetter ?? (r => r.Url))(this.httpRequestBase); }
        }

        public override string ApplicationPath
        {
            get { return this.httpRequestBase.ApplicationPath; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return this.cookies; }
        }

        public override NameValueCollection ServerVariables
        {
            get { return this.serverVars; }
        }

        public override NameValueCollection Headers
        {
            get { return this.headers; }
        }

        public override NameValueCollection Form
        {
            get { return this.form; }
        }

        public override NameValueCollection QueryString
        {
            get { return this.queryStr; }
        }

        string IHttpRequestMutable.HttpMethod
        {
            get { return this.MutableHttpMethod; }
            set { this.MutableHttpMethod = value; }
        }

        Uri IHttpRequestMutable.Url
        {
            get { return this.MutableUrl; }
            set { this.MutableUrl = value; }
        }
    }
}