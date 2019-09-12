using DynamicDataService.Controllers;
using DynamicDataService.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace DynamicDataService.RoutingConventions
{
    public class DynamicNavigationPropertyConvention : NavigationRoutingConvention
    {
        public override string SelectAction(Microsoft.AspNet.OData.Routing.ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath == null || controllerContext == null || actionMap == null)
                return null;

            if (odataPath.PathTemplate != "~/entityset/key/navigation" && odataPath.PathTemplate != "~/entityset/key/cast/navigation")
                return base.SelectAction(odataPath, controllerContext, actionMap);

            if (!controllerContext.ControllerDescriptor.ControllerType.IsSubclassOfRawGeneric(typeof(GenericDataController<>)))
                return base.SelectAction(odataPath, controllerContext, actionMap);

            var segment = odataPath.Segments[odataPath.Segments.Count - 1] as NavigationPropertySegment;

            if (segment != null)
            {
                if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
                {
                    KeySegment keyValueSegment = odataPath.Segments[1] as KeySegment;
                    controllerContext.RouteData.Values[ODataRouteConstants.Key] = keyValueSegment.GenerateKey();
                }
            }
            return null;
        }
    }
}
