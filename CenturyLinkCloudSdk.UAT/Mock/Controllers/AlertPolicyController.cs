using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    [TokenAuthorizationFilter]
    public class AlertPolicyController : ApiController
    {
        [Route("alertpolicies/{alias}")]
        public HttpResponseMessage Post([FromUri] string alias, [FromBody] MockAlertPolicy policy)
        {
            policy.id = Guid.NewGuid().ToString();
            Users.ByAccountAlias[alias].AlertPolicies.Add(policy.id, policy);

            return Request.CreateResponse(HttpStatusCode.OK, policy);
        }

        [Route("alertpolicies/{alias}/{id}")]
        public HttpResponseMessage Delete([FromUri] string alias, [FromUri] string id)
        {
            Users.ByAccountAlias[alias].AlertPolicies.Remove(id);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }

    public class MockAlertPolicy
    {
        public string id { get; set; }

        public string name { get; set; }

        public IEnumerable<MockAlertAction> actions { get; set; }

        public IEnumerable<MockAlertTrigger> triggers { get; set; }
    }

    public class MockAlertAction
    {
        public string action { get; set; }

        public MockAlertActionSettings settings { get; set; }
    }

    public class MockAlertActionSettings
    {
        public IEnumerable<string> recipients { get; set; }
    }

    public class MockAlertTrigger
    {
        public string metric { get; set; }

        public TimeSpan duration { get; set; }

        public int threshold { get; set; }
    }
}