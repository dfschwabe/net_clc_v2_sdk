using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    public class DatacenterController : ApiController
    {
        [Route("datacenters/{alias}/{id}")]
        public HttpResponseMessage Get(string alias, string id)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var includeTotals = queryString.AllKeys.Contains("totals") &&
                                queryString["totals"] == "true";

            if (!includeTotals)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "error");
            }

            var dataCenter = Users.UsersByAccountAlias[alias].DataCentersById[id];
            
            return Request.CreateResponse(HttpStatusCode.OK, dataCenter);
        } 
    }
}