using System;
using System.Net.Http;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;
using CenturyLinkCloudSdk.Services;

namespace CenturyLinkCloudSdk
{
    public class CenturyLinkCloudServiceFactory
    {
        private readonly IAliasProvider _authenticationProvider;
        private readonly IHttpClient _httpClient;
        
        public CenturyLinkCloudServiceFactory(string username, string password)
            : this(username, password, new Uri("https://api.ctl.io"))
        {
        }

        public CenturyLinkCloudServiceFactory(string username, string password, Uri proxyUri)
        {
            var innerClient = HttpClientFactory.Create(new JsonMediaTypeHandler());
            innerClient.BaseAddress = proxyUri;
            
            _httpClient = new HttpClientWrapper(innerClient);
            
            _authenticationProvider = new AuthenticationProvider(username, password, _httpClient);
        }

        public ICenturyLinkCloudAccountService CreateAccountService()
        {
            return new AccountService(_httpClient, _authenticationProvider);
        }
    }
}