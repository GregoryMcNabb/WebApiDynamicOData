using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DynamicDataServiceTests
{
    public class SelectedActionFilter : IAuthenticationFilter
    {
        public Task AuthenticateAsync(
             HttpAuthenticationContext context,
             CancellationToken cancellationToken)
        {
            context.ErrorResult = CreateResult(context.ActionContext);

            // short circuit the rest of the authentication filters
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var actionContext = context.ActionContext;

            actionContext.Request.Properties["selected_action"] =
                actionContext.ActionDescriptor;
            context.Result = CreateResult(actionContext);


            return Task.FromResult(0);
        }

        private static IHttpActionResult CreateResult(
            HttpActionContext actionContext)
        {
            var response = new HttpResponseMessage()
            { RequestMessage = actionContext.Request };

            actionContext.Response = response;

            return new ByPassActionResult(response);
        }

        public bool AllowMultiple { get { return true; } }
    }
}
