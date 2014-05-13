using System.Security.Principal;
using System.Web.Mvc;

namespace MvcStuff.Filters
{
    public class PermissionContext
    {
        public ControllerContext ControllerContext { get; set; }
        public IPrincipal User { get; set; }
    }
}