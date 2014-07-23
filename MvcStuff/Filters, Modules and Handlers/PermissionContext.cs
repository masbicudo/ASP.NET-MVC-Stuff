using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace MvcStuff
{
    public abstract class UserDataPermissionAttribute : PermissionAttribute
    {
        public UserDataPermissionAttribute(object data, params Type[] conditions)
        {

        }

        public override bool CanAccessResource(PermissionContext permissionContext)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Context associated with permissions.
    /// </summary>
    public class PermissionContext
    {
        public PermissionContext(ControllerContext controllerContext, IPrincipal user)
        {
            this.ControllerContext = controllerContext;
        }

        /// <summary>
        /// Gets the controller context.
        /// </summary>
        public ControllerContext ControllerContext { get; private set; }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public IPrincipal User
        {
            get { return this.ControllerContext.HttpContext.User; }
        }

        /// <summary>
        /// Gets the HttpContext of the request.
        /// </summary>
        public HttpContextBase HttpContext
        {
            get { return this.ControllerContext.HttpContext; }
        }
    }
}