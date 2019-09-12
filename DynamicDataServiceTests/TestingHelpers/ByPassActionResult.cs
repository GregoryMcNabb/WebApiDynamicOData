using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace DynamicDataServiceTests
{
    internal class ByPassActionResult : IHttpActionResult
    {
        public HttpResponseMessage Message { get; set; }

        public ByPassActionResult(HttpResponseMessage message)
        {
            Message = message;
        }

        public Task<HttpResponseMessage>
           ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<HttpResponseMessage>(Message);
        }
    }
}
