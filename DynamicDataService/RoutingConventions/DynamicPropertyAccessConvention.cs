using DynamicDataService.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace DynamicDataService.RoutingConventions
{
    public class PropertyAccessConvention : NavigationSourceRoutingConvention
    {
        private const string ActionName = "GetProperty";

        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath == null || controllerContext == null || actionMap == null)
                return null;
            if (odataPath.PathTemplate == "~/entityset/key/property" ||
                odataPath.PathTemplate == "~/entityset/key/cast/property" ||
                odataPath.PathTemplate == "~/singleton/property" ||
                odataPath.PathTemplate == "~/singleton/cast/property")
            {
                var segment = odataPath.Segments[odataPath.Segments.Count - 1] as Microsoft.OData.UriParser.PropertySegment;

                if (segment != null)
                {
                    string actionName = actionMap.FindMatchingAction(ActionName);

                    if (actionName != null)
                    {
                        if (odataPath.PathTemplate.StartsWith("~/entityset/key", StringComparison.Ordinal))
                        {
                            Microsoft.OData.UriParser.KeySegment keyValueSegment = odataPath.Segments[1] as Microsoft.OData.UriParser.KeySegment;
                            controllerContext.RouteData.Values[ODataRouteConstants.Key] = string.Join(",", keyValueSegment
                                .Keys.Select(k => $"{k.Key}={k.Value}"));
                        }
                        controllerContext.RouteData.Values["propertyName"] = segment.Property.Name;
                        return ActionName;
                    }
                }
            }
            return null;
        }

    }
}
