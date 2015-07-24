using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CenturyLinkCloudSdk.UAT.Mock.Controllers
{
    public class AuthenticationController : ApiController
    {
        public const string Token = "eyJpc3MiOiJ1cm46YXBpLXRpZXIzIiwiYXVk.eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9IjoidXJuOnRpZXIzLXVzZXJzIiwibmJmIjoxNDM1NzE1MzYxLCJleHAiOjE0MzY5MjQ5NjEsInJvbGUiOiJBY2NvdW50QWRtaW4iLCJ1bmlxdWVfbmFtZSI6ImRzY2h3YWJlLnQzYmsiLCJ1cm46dGllcjM6YWNjb3VudC1hbGlhcyI6IlQzQksiLCJ4y3xTbwelPiPWwRkdjBgNVh4JsvAlFUVwBtLWPV7GOk.1cm46dGllcjM6bG9jYXRpb24tYWxpYXMiOiJWQTEifQ";
        public static ErrorResponse Error { get; set; }

        [Route("authentication/login")]
        public HttpResponseMessage Post(MockLogin login)
        {
            if (Error != null)
            {
                var response = Request.CreateResponse(Error.StatusCode, Error.Reason);
                Error = null;
                return response;
            }

            User user = Users.Login(login.UserName, login.Password);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new ErrorReason { message = "We didn't recognize the username or password you entered. Please try again" });
            }

            var authentication = new MockAuthentication
            {
                AccountAlias = user.AccountAlias,
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

    [JsonConverter(typeof(MockLoginConverter))]
    public class MockLogin
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    /// This will ensure we promptly fail in the event that the client is configured to serialize to pascal-case
    public class MockLoginConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();

            if (properties[0].Name != "userName")
            {
                throw new JsonSerializationException("camel-case please");
            }

            return new MockLogin
            {
                UserName = (string)properties[0].Value,
                Password = (string)properties[1].Value
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}