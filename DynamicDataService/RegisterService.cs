using DynamicDataService.RoutingConventions;
using DynamicDataService.Selectors;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace DynamicDataService
{
    public static class RegisterServiceExtension
    {
        public static void MapDynamicODataServiceRoute<TContext>(
            this HttpConfiguration config)
            where TContext : DbContext, new()
        {
            MapService<TContext>(config);
        }

        public static void MapDynamicODataServiceRoute<TContext>(
            this HttpConfiguration config,
            string routeName = "DynamicODataRoute",
            string routePrefix = "odata")
            where TContext : DbContext, new()
        {
            MapService<TContext>(config, routeName, routePrefix);
        }

        private static void MapService<TContext>(
            HttpConfiguration config,
            string routeName = "DynamicODataRoute",
            string routePrefix = "odata",
            IList<IODataRoutingConvention> conventions = null,
            ODataBatchHandler batchHandler = null)
            where TContext : DbContext, new()
        {
            config.EnableDependencyInjection();
            
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            //Register all properties of IComponentModel as Odata entities
            MethodInfo baseMethod = typeof(ODataModelBuilder).GetMethod(nameof(builder.EntitySet));
            //All of this reflection only affects the startup time and not during the actual running of the program
            var entityProperties = typeof(TContext).GetProperties(BindingFlags.Instance |
                BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(IQueryable)));
            foreach (var prop in entityProperties)
            {
                var entityType = prop.PropertyType.GetGenericArguments()[0];
                MethodInfo generic = baseMethod.MakeGenericMethod(entityType);

                generic.Invoke(builder, new[] { prop.Name });
            }
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.AddODataQueryFilter();

            if (conventions == null)
                conventions = ODataRoutingConventions.CreateDefault();

            conventions.Insert(0, new KeyEntityRoutingConvention());
            conventions.Insert(0, new DynamicNavigationPropertyConvention());
            conventions.Insert(0, new PropertyAccessConvention());


            var entityModels = builder.GetEdmModel();
            config.MapODataServiceRoute(
                routeName: routeName,
                routePrefix: routePrefix,
                model: entityModels,
                pathHandler: new DefaultODataPathHandler(),
                routingConventions: conventions,
                batchHandler: batchHandler);

            var controllerSelector = new GenericControllerSelector<TContext>(config, builder.EntitySets, config.Services.GetHttpControllerSelector() ?? new DefaultHttpControllerSelector(config));
            config.Services.Replace(typeof(IHttpControllerSelector), controllerSelector);
            foreach (var item in controllerSelector.GetEntityControllers())
            {
                var entitySetInfo = entityModels.EntityContainer.FindEntitySet(item.Key);
                item.Value.Configuration.Services.Replace(typeof(IHttpActionSelector), new GenericActionSelector(config.Services.GetActionSelector(), item, entitySetInfo));
            }
        }
    }
}
