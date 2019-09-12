using DynamicDataService;
using DynamicDataServiceTests.TestingHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Unity;
using Unity.Lifetime;

namespace DynamicDataServiceTests
{
    public class ActionSelectorValidator
    {
        public static HttpActionDescriptor GetTargetAction(
            HttpMethod method,
            string relativeUri)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var container = new UnityContainer();
            container.RegisterType<DbContext, MockContext>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
            config.MapDynamicODataServiceRoute<MockContext>();

            config.Filters.Add(new SelectedActionFilter());
            var server = new HttpServer(config);
            var client = new HttpClient(server);
            var request = new HttpRequestMessage(method, "http://localhost/api/" + relativeUri);
            var response = client.SendAsync(request).Result;
            var actionDescriptor = (HttpActionDescriptor)response.RequestMessage.Properties["selected_action"];

            return actionDescriptor;
        }
    }
}
