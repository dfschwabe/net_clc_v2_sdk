using System;
using System.Collections.Generic;
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
         Task Delete(string policyId, CancellationToken cancellationToken = default(CancellationToken));
         Task<AlertPolicyCollection> Get(CancellationToken cancellationToken = default(CancellationToken));
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
            
            cancellationToken.ThrowIfCancellationRequested();

            return await _httpClient.PostAsync<AlertPolicy>(String.Format("alertpolicies/{0}", alias), definition, cancellationToken);
        }

        public async Task Delete(string policyId, CancellationToken cancellationToken = new CancellationToken())
        {
            var alias = await _aliasProvider.GetAccountAlias();

            cancellationToken.ThrowIfCancellationRequested();

            await _httpClient.DeleteAsync(String.Format("alertpolicies/{0}/{1}", alias, policyId), cancellationToken);
        }

        public Task<AlertPolicyCollection> Get(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}