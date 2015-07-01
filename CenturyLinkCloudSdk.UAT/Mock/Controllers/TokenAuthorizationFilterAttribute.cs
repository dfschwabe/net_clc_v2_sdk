using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    public class TokenAuthorizationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers;
            if (!headers.Contains("Authorization") || headers.GetValues("Authorization").First() != "Bearer " + AuthenticationController.Token)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized
                    , new ErrorResponse{ message = "Authorization has been denied for this request."});
                
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}