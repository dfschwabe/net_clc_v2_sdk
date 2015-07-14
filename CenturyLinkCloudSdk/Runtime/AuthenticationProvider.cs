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
        private Authentication _authentication;

        public AuthenticationProvider(string username, string password, IHttpClient httpClient)
        {
            _username = username;
            _password = password;
            _client = httpClient;
        }

        public async Task<string> GetAccountAlias()
        {
            var authentication = await GetAuthentication();

            return authentication.AccountAlias;
        }

        public async Task<string> GetBearerToken()
        {
            var authentication = await GetAuthentication();

            return authentication.BearerToken;
        }

        private async Task<Authentication> GetAuthentication()
        {
            var loginRequest = new LoginRequest {UserName = _username, Password = _password};

            if (_authentication == null)
            {
                _authentication = await _client.PostAsync<Authentication>("authentication/login", loginRequest, CancellationToken.None);
            }

            return _authentication;
        }
    }
}