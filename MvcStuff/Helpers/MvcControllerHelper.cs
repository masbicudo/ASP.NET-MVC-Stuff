using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;

namespace MvcStuff
{
    /// <summary>
    /// Class containing ASP.NET MVC utilities to help with actions.
    /// </summary>
    public class MvcControllerHelper
    {
        private static readonly object MvcControllerHelperKey = new object();

        class RequestData
        {
            public RequestData()
            {
                this.Controllers = new Dictionary<object, ControllerBase>();
            }

            public Dictionary<object, ControllerBase> Controllers { get; private set; }
        }

        public static MvcControllerHelper Create(
            ControllerContext currentControllerContext,
            [AspMvcAction] string actionName = null,
            [AspMvcController] string controllerName = null,
            object routeValues = null)
        {
            var result = Create(
                currentControllerContext,
                actionName,
                controllerName,
                new RouteValueDictionary(routeValues));

            return result;
        }

        public static MvcControllerHelper Create(
            ControllerContext currentControllerContext,
            [AspMvcAction] string actionName = null,
            [AspMvcController] string controllerName = null,
            RouteValueDictionary routeValues = null)
        {
            controllerName = controllerName ?? currentControllerContext.RouteData.GetRequiredString("controller");

            actionName = actionName ?? currentControllerContext.RouteData.GetRequiredString("action");

            // Getting virtual path data, ensuring that the passed parameters correspond to a route.
            // This is needed only for route validation purposes.
            var mergedRouteValues = new RouteValueDictionary(routeValues ?? new RouteValueDictionary());
            mergedRouteValues["controller"] = controllerName;
            mergedRouteValues["action"] = actionName;

            var virtualPathData = RouteTable.Routes.GetVirtualPathForArea(
                currentControllerContext.RequestContext,
                mergedRouteValues);

            if (virtualPathData == null)
                throw new Exception("Cannot get VirtualPathData with the given parameters.");

            // Getting the dictionary that caches the controllers created in the current request.
            RequestData requestCachedData;
            if (currentControllerContext.HttpContext.Items.Contains(MvcControllerHelperKey))
                requestCachedData = (RequestData)currentControllerContext.HttpContext.Items[MvcControllerHelperKey];
            else
                currentControllerContext.HttpContext.Items[MvcControllerHelperKey] = requestCachedData = new RequestData();

            // If controller is already cached, we use the cached one.
            var controllerKey =
                new
                {
                    area = mergedRouteValues["area"],
                    controller = mergedRouteValues["controller"],
                };
            ControllerBase controller;
            if (!requestCachedData.Controllers.TryGetValue(controllerKey, out controller))
            {
                // Now that we have a virtual path data, that corresponds to an existing route,
                // we can create the controller instance.
                var httpContextMock = new MockHttpContext(currentControllerContext.HttpContext)
                {
                    Request2 =
                        new MockHttpRequest(currentControllerContext.HttpContext.Request)
                };

                // Building route data.
                var routeData = new RouteData(virtualPathData.Route, null);
                foreach (var eachKeyValue in mergedRouteValues)
                    routeData.Values.Add(eachKeyValue.Key, eachKeyValue.Value);

                // Creating controller.
                Debug.Assert(routeData != null, "routeData != null");
                controller = (ControllerBase)ControllerFactory
                    .CreateController(
                    // note: the area does not affect which controller is selected
                        new RequestContext(httpContextMock, routeData),
                        controllerName);

                requestCachedData.Controllers[controllerKey] = controller;
            }

            var controllerType = controller.GetType();

            // todo: cache the controller descriptor as this uses a lot of reflection
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);

            var result = new MvcControllerHelper
            {
                Controller = controller,
                ControllerDescriptor = controllerDescriptor,
                ControllerName = controllerName,
                ControllerType = controllerType,
                CurrentControllerContext = currentControllerContext,
            };

            return result;
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

        public ControllerContext CurrentControllerContext { get; private set; }

        public string ControllerName { get; private set; }

        public ControllerBase Controller { get; private set; }

        public Type ControllerType { get; private set; }

        public ReflectedControllerDescriptor ControllerDescriptor { get; private set; }
    }
}
