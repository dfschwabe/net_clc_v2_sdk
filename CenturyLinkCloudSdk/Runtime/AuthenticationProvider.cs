using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Runtime.Client;

namespace CenturyLinkCloudSdk.Runtime
{
    public interface IAliasProvider
    {
        Task<string> GetAccountAlias();
    }

    public interface ITokenProvider
    {
        Task<string> GetBearerToken();
    }

    public class AuthenticationProvider : IAliasProvider, ITokenProvider
    {
        private readonly string _username;
        private readonly string _password;
        private readonly IHttpClient _client;

        public AuthenticationProvider(string username, string password, IHttpClient httpClient)
        {
            _username = username;
            _password = password;
            _client = httpClient;
        }

        public async Task<string> GetAccountAlias()
        {
            var authentication = await Login();

            return authentication.AccountAlias;
        }

        public async Task<string> GetBearerToken()
        {
            var authentication = await Login();

            return authentication.BearerToken;
        }

        private async Task<Authentication> Login()
        {
            var loginRequest = new LoginRequest {UserName = _username, Password = _password};

            return await _client.PostAsync<LoginRequest, Authentication>("authentication/login", loginRequest, CancellationToken.None);
        }
    }
}