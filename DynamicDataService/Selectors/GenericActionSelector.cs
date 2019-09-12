using DynamicDataService.Controllers;
using DynamicDataService.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace DynamicDataService.Selectors
{
    internal class GenericActionSelector : ODataActionSelector, IHttpActionSelector
    {
        private ILookup<string, HttpActionDescriptor> _actionMappings { get; set; }
        public GenericActionSelector(
            IHttpActionSelector innnerSelector,
            KeyValuePair<string, HttpControllerDescriptor> controller,
            IEdmEntitySet entitySet) : base(innnerSelector)
        {
            _actionMappings = GenerateActionMappings(controller, entitySet);
        }

        private ILookup<string, HttpActionDescriptor> GenerateActionMappings(
            KeyValuePair<string, HttpControllerDescriptor> controller,
            IEdmEntitySet entitySet)
        {
            Dictionary<string, HttpActionDescriptor> actionMaps = new Dictionary<string, HttpActionDescriptor>();
            foreach (var relation in entitySet.NavigationPropertyBindings)
            {
                //Configure Navigation links
                string methodName = null;
                Type relatedPropertyType = null;
                if (relation.NavigationProperty.TargetMultiplicity() == EdmMultiplicity.Many)
                {
                    methodName = "GetRelatedEntities";
                    relatedPropertyType = controller.Value.ControllerType.GetGenericArguments()[0].GetProperty(relation.NavigationProperty.Name).PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    methodName = "GetRelatedEntity";
                    relatedPropertyType = controller.Value.ControllerType.GetGenericArguments()[0].GetProperty(relation.NavigationProperty.Name).PropertyType;
                }

                var genericRelatedEntityGetter = controller.Value.ControllerType.GetMethod(methodName);
                var genericMethod = genericRelatedEntityGetter.MakeGenericMethod(relatedPropertyType);
                //Relate an action link for Get{PropertyName}
                actionMaps.Add(relation.NavigationProperty.Name,
                    new ReflectedHttpActionDescriptor(controller.Value, genericMethod));
            }
            return actionMaps.ToLookup(
                a => a.Key,
                a => a.Value);
        }

        /// <summary>
        /// Returns a map, keyed by action string, of all System.Web.Http.Controllers.HttpActionDescriptor
        /// that the selector can select. This is primarily called by System.Web.Http.Description.IApiExplorer
        /// to discover all the possible actions in the controller.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <returns>
        /// A map of System.Web.Http.Controllers.HttpActionDescriptor that the selector can
        /// select, or null if the selector does not have a well-defined mapping of System.Web.Http.Controllers.HttpActionDescriptor.
        /// </returns>
        public new ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {
            var baseActionMaps = base.GetActionMapping(controllerDescriptor);
            var baseActionGroups = baseActionMaps.ToDictionary(b => b.Key,
                b => b);
            foreach (var mapping in _actionMappings)
                baseActionGroups.Add(mapping.Key, mapping);

            return baseActionGroups.SelectMany(b => b.Value.Select(v => new { Key = b.Key, Value = v }))
                .ToLookup(a => a.Key, a => a.Value);
        }

        public new HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            HttpActionDescriptor action = null;
            var path = controllerContext.Request.RequestUri.LocalPath.Split('/', '(').Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var segment in path)
            {
                if (_actionMappings.Contains(segment))
                {
                    controllerContext.RouteData.Values["navigationPropertyName"] = segment;
                    return _actionMappings[segment].FirstOrDefault();
                }
            }
            action = base.SelectAction(controllerContext);
            return action;
        }
    }
}
