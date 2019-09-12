using Microsoft.AspNet.OData.Routing.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Routing;
using System.Web.Http.Controllers;
using DynamicDataService.Extensions;
using DynamicDataService.Controllers;
using Microsoft.OData.UriParser;

namespace DynamicDataService.RoutingConventions
{
    class KeyEntityRoutingConvention : EntityRoutingConvention
    {
        public override string SelectAction(Microsoft.AspNet.OData.Routing.ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath == null || controllerContext == null || actionMap == null)
                return null;

            if (odataPath.PathTemplate != "~/entityset/key")
                return base.SelectAction(odataPath, controllerContext, actionMap);

            if (!controllerContext.ControllerDescriptor.ControllerType.IsSubclassOfRawGeneric(typeof(GenericDataController<>)))
                return base.SelectAction(odataPath, controllerContext, actionMap);

            var key = odataPath.Segments[1] as KeySegment;
            controllerContext.RouteData.Values[ODataRouteConstants.Key] = key.GenerateKey();
            return "Get";
        }
    }
}
