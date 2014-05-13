using MvcStuff.ActionResults;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MvcStuff.Filters
{
    /// <summary>
    /// Base authorization filter used to allow or deny a loged user to access a specific protected resource.
    /// This is intended to be used as as attribute, but it can be used as a generic IAuthorizationFilter as well.
    /// </summary>
    public abstract class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private static readonly object PermissionAttribute_FilterContext_Key = new object();

        private HttpStatusCode? _denyStatusCode;

        // reference:
        // if someday we have problems with caching restricted-access pages, the following could be useful:
        // http://farm-fresh-code.blogspot.com.br/2009/11/customizing-authorization-in-aspnet-mvc.html

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Items[PermissionAttribute_FilterContext_Key] = filterContext;
                base.OnAuthorization(filterContext);
            }
            finally
            {
                filterContext.HttpContext.Items.Remove(PermissionAttribute_FilterContext_Key);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var canAccess = base.AuthorizeCore(httpContext);

            if (canAccess)
            {
                var filterContext = (AuthorizationContext)httpContext.Items[PermissionAttribute_FilterContext_Key];

                if (filterContext.Result == null)
                {
                    Debug.Assert(httpContext.User != null, "httpContext.User must not be null");
                    canAccess = this.CanAccessResource(
                        new PermissionContext
                        {
                            User = httpContext.User,
                            ControllerContext = filterContext.Controller.ControllerContext
                        });
                }
            }

            return canAccess;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string message;
            if (this.StatusDescriptionResourceType != null)
            {
                message = this.GetResourceLookup<string>(
                    this.StatusDescriptionResourceType,
                    this.StatusDescriptionResourceName);
            }
            else
            {
                message = this.StatusDescription;
            }

            message = message ?? (this.DenyStatusCode == HttpStatusCode.NotFound
                ? null
                : "The current user is not authorized to access the resource because it hasn't got permission.");

            filterContext.Result = new StatusCodeResult(this.DenyStatusCode, message);
        }

        protected virtual T GetResourceLookup<T>(Type resourceType, string resourceName)
        {
            // source: http://stackoverflow.com/a/15193981/195417

            if ((resourceType != null) && (resourceName != null))
            {
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
                var property = resourceType.GetProperty(resourceName, bindingFlags);
                if (property != null)
                    return (T)property.GetValue(null, null);
            }

            return default(T);
        }

        public abstract bool CanAccessResource(PermissionContext permissionContext);

        public Type StatusDescriptionResourceType { get; set; }

        public string StatusDescriptionResourceName { get; set; }

        [Localizable(true)] // must be [Localizable(true)]
        [JetBrains.Annotations.LocalizationRequired(true)]
        public string StatusDescription { get; set; }

        /// <summary>
        /// Determina ou define o código de status HTTP,
        /// que é retornado quando o usuário não possui acesso ao recurso 
        /// protegido por este atributo.
        /// </summary>
        public HttpStatusCode DenyStatusCode
        {
            get { return _denyStatusCode ?? DefaultDenyStatusCode; }
            set { _denyStatusCode = value; }
        }

        static PermissionAttribute()
        {
            DefaultDenyStatusCode = HttpStatusCode.Unauthorized;
        }

        /// <summary>
        /// Determina ou define o código de status HTTP padrão,
        /// que é retornado quando o usuário não possui acesso ao recurso 
        /// protegido por um atributo PermissionAttribute.
        /// </summary>
        public static HttpStatusCode DefaultDenyStatusCode { get; set; }
    }
}