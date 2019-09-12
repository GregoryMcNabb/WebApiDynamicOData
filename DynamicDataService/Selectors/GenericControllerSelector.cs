using DynamicDataService.Controllers;
using Microsoft.AspNet.OData.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace DynamicDataService.Selectors
{
    internal class GenericControllerSelector<TContext> : IHttpControllerSelector
    {
        private IDictionary<string, HttpControllerDescriptor> _controllerMappings;
        private IHttpControllerSelector _InnerSelector { get; set; }

        public GenericControllerSelector(HttpConfiguration config, IEnumerable<EntitySetConfiguration> entities, IHttpControllerSelector innerSelector)
        {
            _controllerMappings = GenerateMappings(config, entities);
            _InnerSelector = innerSelector;
        }

        public IDictionary<string, HttpControllerDescriptor> GetEntityControllers()
        {
            return _controllerMappings;
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _InnerSelector.GetControllerMapping()
                .Concat(_controllerMappings)
                .ToLookup(c => c.Key, c => c.Value)
                .ToDictionary(c => c.Key, c => c.First());
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            try
            {
                return _InnerSelector.SelectController(request);
            }
            catch(HttpResponseException ex)
            {
                if(ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return SelectEntityController(request);
                throw ex;
            }
        }

        private HttpControllerDescriptor SelectEntityController(HttpRequestMessage request)
        {
            string path = null;
            try
            {
                path = request.RequestUri.LocalPath;
            }
            catch(InvalidOperationException)
            {
                path = request.RequestUri.OriginalString;
            }
            var segments = path.Split('/', '(').Where(p => !string.IsNullOrWhiteSpace(p));
            foreach (var segment in segments)
            {
                if (_controllerMappings.ContainsKey(segment))
                    return _controllerMappings[segment];
            }
            throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
        }

        private IDictionary<string, HttpControllerDescriptor> GenerateMappings(HttpConfiguration config, IEnumerable<EntitySetConfiguration> entitySets)
        {
            IDictionary<string, HttpControllerDescriptor> dictionary = new Dictionary<string, HttpControllerDescriptor>();

            foreach (EntitySetConfiguration set in entitySets)
            {
                Type genericControllerType = typeof(GenericDataController<>).MakeGenericType(set.ClrType);


                //Creates an instance of the generic data controller for this type
                var genericControllerDescription = new HttpControllerDescriptor(config,
                    set.Name, genericControllerType);
                dictionary.Add(set.Name, genericControllerDescription);
            }

            return dictionary;
        }
    }
}
