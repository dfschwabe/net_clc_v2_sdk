using System;
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

        [Route("antiaffinitypolicies/{alias}/{id}")]
        public HttpResponseMessage Put([FromUri] string alias, [FromUri] string id, [FromBody] MockAntiAffinityPolicy policy)
        {
            Users.ByAccountAlias[alias].AntiAffinityPolicies[id].name = policy.name;

            return Request.CreateResponse(HttpStatusCode.OK, policy);
        }

        [Route("antiaffinitypolicies/{alias}")]
        public HttpResponseMessage Post([FromUri] string alias, [FromBody] MockAntiAffinityPolicy policy)
        {
            policy.id = Guid.NewGuid().ToString();
            Users.ByAccountAlias[alias].AntiAffinityPolicies.Add(policy.id, policy);

            return Request.CreateResponse(HttpStatusCode.OK, policy);
        }

        [Route("antiaffinitypolicies/{alias}/{id}")]
        public HttpResponseMessage Delete([FromUri] string alias, [FromUri] string id)
        {
            Users.ByAccountAlias[alias].AntiAffinityPolicies.Remove(id);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
   
    public class MockAntiAffinityPolicy
    {
        public string id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }

    public class MockAntiAffinityPolicyCollection
    {
        public List<MockAntiAffinityPolicy> items { get; set; }
    }


}