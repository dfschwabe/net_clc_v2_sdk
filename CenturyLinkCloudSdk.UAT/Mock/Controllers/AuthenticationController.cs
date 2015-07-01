using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    public class AuthenticationController : ApiController
    {
        public const string Token = "bearerToken=eyJpc3MiOiJ1cm46YXBpLXRpZXIzIiwiYXVk.eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9IjoidXJuOnRpZXIzLXVzZXJzIiwibmJmIjoxNDM1NzE1MzYxLCJleHAiOjE0MzY5MjQ5NjEsInJvbGUiOiJBY2NvdW50QWRtaW4iLCJ1bmlxdWVfbmFtZSI6ImRzY2h3YWJlLnQzYmsiLCJ1cm46dGllcjM6YWNjb3VudC1hbGlhcyI6IlQzQksiLCJ4y3xTbwelPiPWwRkdjBgNVh4JsvAlFUVwBtLWPV7GOk.1cm46dGllcjM6bG9jYXRpb24tYWxpYXMiOiJWQTEifQ";

        [Route("authentication/login")]
        public HttpResponseMessage Post(MockLogin login)
        {
            var authentication = new MockAuthentication
            {
                AccountAlias = Users.UserA.AccountAlias,
                BearerToken = Token
            };

            return Request.CreateResponse(HttpStatusCode.OK, authentication);
        }
    }

    public class MockAuthentication
    {
        public string AccountAlias { get; set; }
        public string BearerToken { get; set; }
    }

    public class MockLogin
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}