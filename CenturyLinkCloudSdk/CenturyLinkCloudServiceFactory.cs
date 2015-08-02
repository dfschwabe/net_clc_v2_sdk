using System;
using System.Collections.Generic;
using System.Net.Http;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;
using CenturyLinkCloudSdk.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter{CamelCaseText = true}}
            };

            var authProviderClient = HttpClientFactory.Create(new JsonMediaTypeHandler());
            var authProviderWrapper = new HttpClientWrapper(authProviderClient, serializerSettings);
            _authenticationProvider = new AuthenticationProvider(username, password, authProviderWrapper);
            
            var authHandler = new AuthenticationHandler(_authenticationProvider);
            var authorizedClient = HttpClientFactory.Create(authHandler, new JsonMediaTypeHandler());

            authProviderClient.BaseAddress = authorizedClient.BaseAddress = proxyUri;

            _clientWrapper = new HttpClientWrapper(authorizedClient, serializerSettings);
        }

        public ICenturyLinkCloudAccountService CreateAccountService()
        {
            return new AccountService(_clientWrapper, _authenticationProvider);
        }

        public ICenturyLinkCloudAlertPolicyService CreateAlertPolicyService()
        {
            return new AlertPolicyService(_clientWrapper, _authenticationProvider);
        }

        public ICenturyLinkCloudAntiAffinityPolicyService CreateAntiAffinityPolicyService()
        {
            throw new NotImplementedException();
        }
    }
}