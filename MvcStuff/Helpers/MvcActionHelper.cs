using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;

namespace MvcStuff
{
    /// <summary>
    /// Class containing ASP.NET MVC utilities to help with actions.
    /// </summary>
    public class MvcActionHelper
    {
        public static MvcActionHelper Create(
            ControllerContext currentControllerContext,
            [AspMvcAction] string actionName = null,
            [AspMvcController] string controllerName = null,
            string httpMethod = "GET",
            object routeValues = null,
            string protocol = null,
            string hostName = null,
            bool useCache = true)
        {
            var result = Create(
                currentControllerContext,
                actionName,
                controllerName,
                httpMethod,
                new RouteValueDictionary(routeValues),
                protocol,
                hostName,
                useCache);

            return result;
        }

        public static MvcActionHelper Create(
            ControllerContext currentControllerContext,
            [AspMvcAction] string actionName = null,
            [AspMvcController] string controllerName = null,
            string httpMethod = "GET",
            RouteValueDictionary routeValues = null,
            string protocol = null,
            string hostName = null,
            bool useCache = true)
        {
            actionName = actionName ?? currentControllerContext.RouteData.GetRequiredString("action");
            controllerName = controllerName ?? currentControllerContext.RouteData.GetRequiredString("controller");

            var urlHelper = new UrlHelper(currentControllerContext.RequestContext);
            var currentUri = currentControllerContext.RequestContext.HttpContext.Request.Url;

            Debug.Assert(currentUri != null, "currentUri != null");
            Debug.Assert(routeValues != null, "routeValues != null");
            var urlAction = urlHelper.Action(
                actionName,
                controllerName,
                routeValues,
                protocol ?? currentUri.Scheme,
                hostName ?? currentUri.Host);

            if (urlAction == null)
                throw new Exception("Cannot generate an action link with the given parameters.");

            var uri = new Uri(urlAction);

            var httpContext = new MockHttpContext(currentControllerContext.HttpContext)
            {
                Request2 =
                    new MockHttpRequest(currentControllerContext.HttpContext.Request)
                    {
                        HttpMethod2 = httpMethod,
                        Url2 = uri,
                    }
            };

            // Building route data.
            var routeData = RouteTable.Routes.GetRouteData(httpContext);

            ControllerBase controller;
            Type controllerType;
            ReflectedControllerDescriptor controllerDescriptor;
            if (useCache)
            {
                var controllerHelper = MvcControllerHelper.Create(
                    currentControllerContext,
                    actionName,
                    controllerName,
                    routeValues);

                controller = controllerHelper.Controller;
                controllerType = controllerHelper.ControllerType;
                controllerDescriptor = controllerHelper.ControllerDescriptor;
            }
            else
            {
                // Creating controller.
                Debug.Assert(routeData != null, "routeData != null");
                controller = (ControllerBase)ControllerFactory
                    .CreateController(
                    // note: the area does not affect which controller is selected
                        new RequestContext(httpContext, routeData),
                        controllerName);

                controllerType = controller.GetType();
                controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            }

            // Creating fake controller context.
            var mockControllerContext = new ControllerContext(
                httpContext,
                routeData,
                controller);

            controller.ControllerContext = mockControllerContext;

            var actionDescriptor = controllerDescriptor
                .FindAction(mockControllerContext, actionName);

            var result = new MvcActionHelper
            {
                ActionDescriptor = actionDescriptor,
                ActionName = actionName,
                Controller = controller,
                ControllerDescriptor = controllerDescriptor,
                ControllerName = controllerName,
                ControllerType = controllerType,
                CurrentControllerContext = currentControllerContext,
                HttpMethod = httpMethod,
                ControllerContext = mockControllerContext,
                Uri = uri,
                RouteData = routeData,
                HttpContext = httpContext,
            };

            return result;
        }

        public static MvcActionHelper Create(
          ControllerContext currentControllerContext,
          [AspMvcAction] string url,
          string httpMethod = "GET",
          NameValueCollection query = null,
          bool useCache = true)
        {
            var currentUri = currentControllerContext.HttpContext.Request.Url;
            Debug.Assert(currentUri != null, "currentUri != null");
            var uriBuilder = new UriBuilder(new Uri(currentUri, url));

            if (query != null)
                uriBuilder.Query += string.Join(
                    "&",
                    query.Cast<string>().SelectMany(
                        key => query.GetValues(key).Select(
                            val => string.Format(
                                "{0}={1}",
                                HttpUtility.UrlEncode(key),
                                HttpUtility.UrlEncode(val)))));

            var httpContext = new MockHttpContext(currentControllerContext.HttpContext)
            {
                Request2 =
                    new MockHttpRequest(currentControllerContext.HttpContext.Request)
                    {
                        HttpMethod2 = httpMethod,
                        Url2 = uriBuilder.Uri,
                    }
            };

            // Building route data.
            var routeData = RouteTable.Routes.GetRouteData(httpContext);

            Debug.Assert(routeData != null, "routeData != null");
            var actionName = routeData.GetRequiredString("action");
            var controllerName = routeData.GetRequiredString("controller");

            ControllerBase controller;
            Type controllerType;
            ReflectedControllerDescriptor controllerDescriptor;
            if (useCache)
            {
                var controllerHelper = MvcControllerHelper.Create(
                    currentControllerContext,
                    actionName,
                    controllerName,
                    routeData.Values);

                controller = controllerHelper.Controller;
                controllerType = controllerHelper.ControllerType;
                controllerDescriptor = controllerHelper.ControllerDescriptor;
            }
            else
            {
                // Creating controller.
                Debug.Assert(routeData != null, "routeData != null");
                controller = (ControllerBase)ControllerFactory
                    .CreateController(
                    // note: the area does not affect which controller is selected
                        new RequestContext(httpContext, routeData),
                        controllerName);

                controllerType = controller.GetType();
                controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            }

            // Creating fake controller context.
            var mockControllerContext = new ControllerContext(
                httpContext,
                routeData,
                controller);

            controller.ControllerContext = mockControllerContext;

            var actionDescriptor = controllerDescriptor
                .FindAction(mockControllerContext, actionName);

            var result = new MvcActionHelper
            {
                ActionDescriptor = actionDescriptor,
                ActionName = actionName,
                Controller = controller,
                ControllerDescriptor = controllerDescriptor,
                ControllerName = controllerName,
                ControllerType = controllerType,
                CurrentControllerContext = currentControllerContext,
                HttpMethod = httpMethod,
                ControllerContext = mockControllerContext,
                Uri = uriBuilder.Uri,
                RouteData = routeData,
                HttpContext = httpContext,
            };

            return result;
        }

        /// <summary>
        /// Returns all the filters that are executed when calling an action.
        /// This uses the default Mvc classes used to get the filters,
        /// so the behavior is the same.
        /// This means that the filters are returned in order,
        /// according to Order and Scope values of the filters.
        /// </summary>
        /// <returns>Filters that are used when calling the action.</returns>
        public Filter[] GetFilters()
        {
            var actionDescriptor = ActionDescriptor;

            // The default Controller.ActionInvoker.GetFilters returns filters from FilterProviders.Providers.GetFilters method.
            // So this method may not be compatible with custom controller implementations that override the ActionInvoker,
            // or override the GetFilters method.
            var filters = FilterProviders.Providers
                .GetFilters(ControllerContext, actionDescriptor)
                .ToArray();

            return filters;
        }

        public static IControllerFactory ControllerFactory
        {
            get
            {
                // The default MvcHandler.ControllerBuilder returns ControllerBuilder.Current.
                // So this may not be compatible with other implementations of MvcHandler.
                return ControllerBuilder.Current.GetControllerFactory();
            }
        }

        public ControllerContext CurrentControllerContext { get; set; }

        public string ActionName { get; private set; }

        public string ControllerName { get; private set; }

        public string HttpMethod { get; set; }

        public ControllerBase Controller { get; private set; }

        public ActionDescriptor ActionDescriptor { get; private set; }

        public ControllerContext ControllerContext { get; private set; }

        public Type ControllerType { get; private set; }

        public ReflectedControllerDescriptor ControllerDescriptor { get; private set; }

        public Uri Uri { get; private set; }

        public RouteData RouteData { get; set; }

        public HttpContextBase HttpContext { get; set; }

        public RequestContext RequestContext
        {
            get { return this.ControllerContext.RequestContext; }
        }
    }
}
