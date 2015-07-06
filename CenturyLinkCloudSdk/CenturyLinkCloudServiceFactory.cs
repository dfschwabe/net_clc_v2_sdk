using System;
using System.Net.Http;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;
using CenturyLinkCloudSdk.Services;

namespace CenturyLinkCloudSdk
{
    public class CenturyLinkCloudServiceFactory
    {
        private readonly AuthenticationProvider _authenticationProvider;
        private readonly IHttpClient _clientWrapper;
        
        public CenturyLinkCloudServiceFactory(string username, string password)
            : this(username, password, new Uri("https://api.ctl.io/v2/"))
        {
        }

        public CenturyLinkCloudServiceFactory(string username, string password, Uri proxyUri)
        {
            var authProviderClient = HttpClientFactory.Create(new JsonMediaTypeHandler());
            var authProviderWrapper = new HttpClientWrapper(authProviderClient);
            _authenticationProvider = new AuthenticationProvider(username, password, authProviderWrapper);
            
            var authHandler = new AuthenticationHandler(_authenticationProvider);
            var authorizedClient = HttpClientFactory.Create(authHandler, new JsonMediaTypeHandler());

            authProviderClient.BaseAddress = authorizedClient.BaseAddress = proxyUri;
            
            _clientWrapper = new HttpClientWrapper(authorizedClient);
        }

        public ICenturyLinkCloudAccountService CreateAccountService()
        {
            return new AccountService(_clientWrapper, _authenticationProvider);
        }
    }
}