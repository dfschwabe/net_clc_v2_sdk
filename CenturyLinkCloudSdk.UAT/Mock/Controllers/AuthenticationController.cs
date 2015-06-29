using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    public class AuthenticationController : ApiController
    {
        [Route("authentication/login")]
        public HttpResponseMessage Post(MockLogin login)
        {
            var authentication = new MockAuthentication  { AccountAlias = Users.UserA.AccountAlias };

            return Request.CreateResponse(HttpStatusCode.OK, authentication);
        }
    }

    public class MockAuthentication
    {
        public string AccountAlias { get; set; }
    }

    public class MockLogin
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}