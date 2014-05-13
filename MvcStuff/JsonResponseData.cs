using System;

namespace MvcStuff
{
    [Serializable]
    public class JsonResponseData
    {
        public string RedirectUrl { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public Object Obj { get; set; }
        public string ErrorType { get; set; }
        public int Status { get; set; }
    }
}
