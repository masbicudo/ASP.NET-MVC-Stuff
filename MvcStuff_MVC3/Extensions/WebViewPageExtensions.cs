using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;

namespace MvcStuff
{
    /// <summary>
    /// Extension methods to the view page that allows changin the view
    /// accordin to access control rules, and other route informations.
    /// </summary>
    public static class WebViewPageExtensions
    {
        private static object AnyAccessAllowedKey = new CustomKey("AnyAccessAllowedKey");

        class AnyAccessAllowedStackItem
        {
            public bool IsToRender { get; set; }
        }

        /// <summary>
        /// Renders the content inside of an using statement, only if any of the calls to 'CanAccessAction'
        /// inside the using returns true.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <returns> Returns a disposable object that is used to render the contents inside the using. </returns>
        public static AllowanceDisposer AnyAccessAllowed(this WebViewPage @this)
        {
            var stringBuilder = ((StringWriter)@this.ViewContext.Writer).GetStringBuilder();
            var oldLength = stringBuilder.Length;

            var itemsCollection = @this.Context.Items;

            Stack<AnyAccessAllowedStackItem> stack;
            if (!itemsCollection.Contains(AnyAccessAllowedKey))
                itemsCollection[AnyAccessAllowedKey] = stack = new Stack<AnyAccessAllowedStackItem>();
            else
                stack = (Stack<AnyAccessAllowedStackItem>)itemsCollection[AnyAccessAllowedKey];

            stack.Push(new AnyAccessAllowedStackItem());

            return new AllowanceDisposer(
                allowed =>
                {
                    // ReSharper disable once ConstantNullCoalescingCondition
                    // resharper lies about the following line of code... in fact `!(bool?)null` => `null`.
                    var result = !allowed ?? !stack.Pop().IsToRender;
                    SetIsToRenderIfNeeded(result, itemsCollection);
                    if (result)
                    {
                        stringBuilder.Length = oldLength;
                    }
                });
        }

        private static void SetIsToRenderIfNeeded(bool result, IDictionary itemsCollection)
        {
            if (result && itemsCollection.Contains(AnyAccessAllowedKey))
            {
                var stack = (Stack<AnyAccessAllowedStackItem>)itemsCollection[AnyAccessAllowedKey];
                if (stack != null && stack.Count > 0)
                    stack.Peek().IsToRender = true;
            }
        }

        /// <summary>
        /// Checks whether the current user can access the specified action.
        /// At this moment it looks only at PermissionAttribute attributes.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="routeValues">An object containing the route values for the action. </param>
        /// <param name="method">Http method to differentiate GET, HEAD, POST, PUT and DELETE actions.</param>
        /// <returns>Returns true if the current user has access to the given action; otherwise false. </returns>
        public static bool CanAccessAction(
            this WebViewPage @this,
            [AspMvcArea("area")] object routeValues = null,
            HttpVerbs method = HttpVerbs.Get)
        {
            return @this.CanAccessAction(null, null, routeValues, method);
        }

        /// <summary>
        /// Checks whether the current user can access the specified action.
        /// At this moment it looks only at PermissionAttribute attributes.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="action">Action name to test.</param>
        /// <param name="routeValues">An object containing the route values for the action. </param>
        /// <param name="method">Http method to differentiate GET, HEAD, POST, PUT and DELETE actions.</param>
        /// <returns>Returns true if the current user has access to the given action; otherwise false. </returns>
        public static bool CanAccessAction(
            this WebViewPage @this,
            [AspMvcAction] string action,
            [AspMvcArea("area")] object routeValues = null,
            HttpVerbs method = HttpVerbs.Get)
        {
            return @this.CanAccessAction(action, null, routeValues, method);
        }

        /// <summary>
        /// Checks whether the current user can access the specified action.
        /// At this moment it looks only at PermissionAttribute attributes.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="action">Action name to test.</param>
        /// <param name="controller">Controller name to test.</param>
        /// <param name="routeValues">An object containing the route values for the action. </param>
        /// <param name="method">Http method to differentiate GET, HEAD, POST, PUT and DELETE actions.</param>
        /// <returns>Returns true if the current user has access to the given action; otherwise false. </returns>
        public static bool CanAccessAction(
            this WebViewPage @this,
            [AspMvcAction] string action = null,
            [AspMvcController] string controller = null,
            [AspMvcArea("area")] object routeValues = null,
            HttpVerbs method = HttpVerbs.Get)
        {
            // TODO: must cache all of these informations

            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            var methodStr = method.ToString().ToUpperInvariant();
            var routeValuesDic = new RouteValueDictionary(routeValues);
            var mvcHelper = MvcActionHelper.Create(
                @this.ViewContext.Controller.ControllerContext,
                action,
                controller,
                methodStr,
                routeValuesDic);

            if (mvcHelper.ActionDescriptor == null)
            {
                // The view does not exist... this means that nobody can access it.
                return false;
            }

            if (routeValues != null)
            {
                // checking action parameters
                var actionParams = mvcHelper.ActionDescriptor.GetParameters();

                // todo: check routeValuesDic to see if the contained values fit the actionParams
                // todo: maybe we should try to bind values (it could be slow)
            }

            // Getting the current User... (the logged user).
            var user = @this.Context.User;

            // If there is a logged user, then use permission attributes to determine whether user has access or not.
            if (user != null)
            {
                var result = CheckUserAccess(mvcHelper);
                SetIsToRenderIfNeeded(result, @this.Context.Items);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the current user can access the specified URL.
        /// At this moment it looks only at PermissionAttribute attributes.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="url">Url to test.</param>
        /// <param name="queryValues">An object containing values to add to the query of the URL. </param>
        /// <param name="method">Http method to differentiate GET, HEAD, POST, PUT and DELETE actions.</param>
        /// <returns>Returns true if the current user has access to the given URL; otherwise false. </returns>
        public static bool CanAccessUrl(
            this WebViewPage @this,
            [NotNull] string url,
            object queryValues = null,
            HttpVerbs method = HttpVerbs.Get)
        {
            // TODO: must cache all of these informations

            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var methodStr = method.ToString().ToUpperInvariant();
            var routeValuesDic = new RouteValueDictionary(queryValues);
            var nameValueQuery = new NameValueCollection();
            foreach (var eachKeyValue in routeValuesDic)
                if (eachKeyValue.Value != null)
                    nameValueQuery.Add(eachKeyValue.Key, eachKeyValue.Value.ToString());

            var mvcHelper = MvcActionHelper.Create(
                @this.ViewContext.Controller.ControllerContext,
                url,
                methodStr,
                nameValueQuery);

            if (mvcHelper.ActionDescriptor == null)
            {
                // The view does not exist... this means that nobody can access it.
                return false;
            }

            if (mvcHelper.RouteData.Values != null)
            {
                // checking action parameters
                var actionParams = mvcHelper.ActionDescriptor.GetParameters();

                // todo: check routeValuesDic to see if the contained values fit the actionParams
                // todo: maybe we should try to bind values (it could be slow)
            }

            // Getting the current User... (the logged user).
            var user = @this.Context.User;

            // If there is a logged user, then use permission attributes to determine whether user has access or not.
            if (user != null)
            {
                var result = CheckUserAccess(mvcHelper);
                SetIsToRenderIfNeeded(result, @this.Context.Items);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the current user can access the specified action.
        /// At this moment it looks only at PermissionAttribute attributes.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="actions">Action names to test.</param>
        /// <param name="controller">Controller name to test.</param>
        /// <param name="method">Http method to differentiate GET, HEAD, POST, PUT and DELETE actions.</param>
        /// <param name="routeValues">An object containing the route values for the action. </param>
        /// <returns>Returns true if the current user has access to the given action; otherwise false. </returns>
        public static bool CanAccessAnyAction(
            this WebViewPage @this,
            [AspMvcAction("Index")]string[] actions = null,
            [AspMvcController]string controller = null,
            HttpVerbs method = HttpVerbs.Get,
            [AspMvcArea("area")]object routeValues = null)
        {
            // TODO: must cache all of these informations

            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            var methodStr = method.ToString().ToUpperInvariant();
            var routeValuesDic = new RouteValueDictionary(routeValues);
            var mvcHelpers = actions.Select(
                action => MvcActionHelper.Create(
                    @this.ViewContext.Controller.ControllerContext,
                    action,
                    controller,
                    methodStr,
                    routeValuesDic)).ToArray();

            if (mvcHelpers.All(helper => helper.ActionDescriptor != null))
            {
                // The view does not exist... this means that nobody can access it.
                return false;
            }

            if (routeValues != null)
            {
                // checking action parameters
                //var actionParams = mvcHelper.ActionDescriptor.GetParameters();

                // todo: check routeValuesDic to see if the contained values fit the actionParams
                // todo: maybe we should try to bind values (it could be slow)
            }

            // Getting the current User... (the logged user).
            var user = @this.Context.User;

            // If there is a logged user, then use permission attributes to determine whether user has access or not.
            if (user != null)
            {
                var result = mvcHelpers.Any(CheckUserAccess);
                SetIsToRenderIfNeeded(result, @this.Context.Items);
                return result;
            }

            return false;
        }

        private static bool CheckUserAccess(MvcActionHelper mvcHelper)
        {
            var attributes = mvcHelper
                .GetFilters()
                .Select(f => f.Instance)
                .OfType<IAuthorizationFilter>()
                .ToArray();

            var permissionContext = new AuthorizationContext
            {
                ActionDescriptor = mvcHelper.ActionDescriptor,
                Controller = mvcHelper.Controller,
                HttpContext = mvcHelper.HttpContext,
                RequestContext = mvcHelper.RequestContext,
                Result = null,
                RouteData = mvcHelper.RouteData,
            };

            // sets the controller context before calling the filter
            var oldContext = mvcHelper.Controller.ControllerContext;
            try
            {
                mvcHelper.Controller.ControllerContext = mvcHelper.ControllerContext;

                var result = attributes.All(
                    pa =>
                    {
                        pa.OnAuthorization(permissionContext);
                        return IsContextAuthorized(permissionContext);
                    });

                return result;
            }
            finally
            {
                mvcHelper.Controller.ControllerContext = oldContext;
            }
        }

        /// <summary>
        /// Checks whether the current view is the result of the specified action call.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="action">Action name to test.</param>
        /// <param name="controller">Controller name to test.</param>
        /// <returns>Returns true if the current view represents the result of the given action; otherwise false.</returns>
        public static bool IsAction(
            this WebViewPage @this,
            [AspMvcAction]string action,
            [AspMvcController]string controller = null)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            var routeData = @this.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");

            if (controller != null)
                return string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase)
                       && string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase);

            return string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether the current view is the result of the specified controller call.
        /// </summary>
        /// <param name="this">The current view page.</param>
        /// <param name="controllerNames">Names of the controllers to test.</param>
        /// <returns>Returns true if the current view represents the result of any of the given controller; otherwise false.</returns>
        public static bool IsController(
            this WebViewPage @this,
            [AspMvcController]params string[] controllerNames)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this));

            var routeData = @this.ViewContext.RouteData;
            var currentController = routeData.GetRequiredString("controller");

            return controllerNames.Any(cn => string.Equals(currentController, cn, StringComparison.OrdinalIgnoreCase));
        }

        static readonly List<Func<AuthorizationContext, bool?>> ContextAuthorizedPredicates
            = new List<Func<AuthorizationContext, bool?>>();

        /// <summary>
        /// Adds a predicate to indicate whether an authorization context is considered as being authorized or not.
        /// </summary>
        /// <param name="authorizedPredicate"></param>
        public static void AddContextAuthorizedPredicate(Func<AuthorizationContext, bool?> authorizedPredicate)
        {
            lock (ContextAuthorizedPredicates)
                ContextAuthorizedPredicates.Insert(0, authorizedPredicate);
        }

        static WebViewPageExtensions()
        {
            ContextAuthorizedPredicates.Add(AuthorizedWhenResultIsNull);
        }

        private static bool? AuthorizedWhenResultIsNull(AuthorizationContext arg)
        {
            if (arg.Result == null)
                return true;
            return null;
        }

        private static bool IsContextAuthorized(AuthorizationContext authContext)
        {
            Func<AuthorizationContext, bool?>[] list;
            lock (ContextAuthorizedPredicates)
                list = ContextAuthorizedPredicates.ToArray();

            return (from contextAuthorizedPredicate in list
                    select contextAuthorizedPredicate(authContext)
                        into result
                        where result.HasValue
                        select result.Value)
                .FirstOrDefault();
        }
    }
}
