using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    [TokenAuthorizationFilter]
    public class SearchController : ApiController
    {
        public const int ExpectedLimit = 2;

        [Route("search/activities")]
        public HttpResponseMessage Post(MockActivityFilter filter)
        {
            if (filter.Limit != ExpectedLimit)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "error");
            }

            var result = filter.Accounts
                               .SelectMany(a => Users.ByAccountAlias[a].RecentActivity);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }

    public class MockActivityFilter
    {
        public List<string> Accounts { get; set; }

        public int Limit { get; set; }
    }
}