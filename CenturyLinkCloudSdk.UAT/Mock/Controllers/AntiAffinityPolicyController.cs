using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    [TokenAuthorizationFilter]
    public class AntiAffinityPolicyController : ApiController
    {
        [Route("antiaffinitypolicies/{alias}")]
        public HttpResponseMessage GetPolicies([FromUri] string alias)
        {
            var policies = new MockAntiAffinityPolicyCollection
            {
                items = Users.ByAccountAlias[alias].AntiAffinityPolicies.Values.ToList()
            };

            return Request.CreateResponse(HttpStatusCode.OK, policies);

        }

        [Route("antiaffinitypolicies/{alias}/{id}")]
        public HttpResponseMessage GetPolicy([FromUri] string alias, [FromUri] string id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, Users.ByAccountAlias[alias].AntiAffinityPolicies[id]);
        }
    }

    public class MockAntiAffinityPolicyDefinition
    {
        public string name { get; set; }
        public string location { get; set; }
    }    
    
    public class MockAntiAffinityPolicy : MockAntiAffinityPolicyDefinition
    {
        public string id { get; set; }
    }

    public class MockAntiAffinityPolicyCollection
    {
        public List<MockAntiAffinityPolicy> items { get; set; }
    }


}