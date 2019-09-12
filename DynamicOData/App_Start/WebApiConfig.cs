using DynamicOData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DynamicDataService;
using Unity;
using Unity.Lifetime;
using DynamicOData.ServiceProvider;
using System.Data.Entity;

namespace DynamicOData
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            var container = new UnityContainer();
            container.RegisterType<DbContext, TestingModel>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
            config.MapDynamicODataServiceRoute<TestingModel>();
        }
    }
}
