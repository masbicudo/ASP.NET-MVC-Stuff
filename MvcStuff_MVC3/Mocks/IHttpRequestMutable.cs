using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcStuff.Mocks
{
    public interface IHttpRequestMutable
    {
        string HttpMethod { get; set; }
        Uri Url { get; set; }
    }
}
