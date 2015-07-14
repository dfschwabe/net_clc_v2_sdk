using System;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;

namespace CenturyLinkCloudSdk.Services
{
    public interface ICenturyLinkCloudAlertPolicyService
    {
         Task<AlertPolicy> Create(AlertPolicyDefniition definition, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AlertPolicyService : ICenturyLinkCloudAlertPolicyService
    {
        private readonly IHttpClient _httpClient;
        private readonly IAliasProvider _aliasProvider;

        public AlertPolicyService(IHttpClient httpClient, IAliasProvider aliasProvider)
        {
            _httpClient = httpClient;
            _aliasProvider = aliasProvider;
        }

        public async Task<AlertPolicy> Create(AlertPolicyDefniition definition, CancellationToken cancellationToken = new CancellationToken())
        {
            var alias = await _aliasProvider.GetAccountAlias();
            var requestUri = String.Format("alertpolicies/{0}", alias);

            return await _httpClient.PostAsync<AlertPolicy>(requestUri, definition, cancellationToken);
        }
    }
}