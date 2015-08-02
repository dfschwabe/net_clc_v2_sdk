using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.Models.Internal;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;

namespace CenturyLinkCloudSdk.Services
{
    public interface ICenturyLinkCloudAlertPolicyService
    {
         Task<AlertPolicy> Create(AlertPolicyDefniition definition, CancellationToken cancellationToken = default(CancellationToken));
         Task Delete(string policyId, CancellationToken cancellationToken = default(CancellationToken));
         Task<AlertPolicy> Get(string policyId, CancellationToken cancellationToken = default(CancellationToken));
         Task<List<AlertPolicy>> Get(CancellationToken cancellationToken = default(CancellationToken));
         Task<AlertPolicy> Update(string policyId, AlertPolicyDefniition policyDefinition, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AlertPolicyService : ICenturyLinkCloudAlertPolicyService
    {
        private const string Api = "alertpolicies";
        private readonly IHttpClient _httpClient;
        private readonly IAliasProvider _aliasProvider;

        public AlertPolicyService(IHttpClient httpClient, IAliasProvider aliasProvider)
        {
            _httpClient = httpClient;
            _aliasProvider = aliasProvider;
        }

        public async Task<AlertPolicy> Create(AlertPolicyDefniition definition, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(cancellationToken);

            return await _httpClient.PostAsync<AlertPolicy>(requestUri, definition, cancellationToken);
        }

        public async Task Delete(string policyId, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(policyId, cancellationToken);

            await _httpClient.DeleteAsync(requestUri, cancellationToken);
        }

        public async Task<AlertPolicy> Get(string policyId, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(policyId, cancellationToken);

            return await _httpClient.GetAsync<AlertPolicy>(requestUri, cancellationToken);
        }

        public async Task<List<AlertPolicy>> Get(CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(cancellationToken);

            var result = await _httpClient.GetAsync<ModelCollection<AlertPolicy>>(requestUri, cancellationToken);

            return result.Items;
        }

        public async Task<AlertPolicy> Update(string policyId, AlertPolicyDefniition definition, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(policyId, cancellationToken);

            return await _httpClient.PutAsync<AlertPolicy>(requestUri, definition, cancellationToken);
        }

        private async Task<string> GetUri(CancellationToken cancellationToken)
        {
            var alias = await GetAlias(cancellationToken);

            return String.Format("{0}/{1}", Api, alias);
        }

        private async Task<string> GetUri(string policyId, CancellationToken cancellationToken)
        {
            var alias = await GetAlias(cancellationToken);

            return String.Format("{0}/{1}/{2}", Api, alias, policyId);
        }

        private async Task<string> GetAlias(CancellationToken cancellationToken)
        {
            var result = await _aliasProvider.GetAccountAlias();

            cancellationToken.ThrowIfCancellationRequested();

            return result;
        }
    }
}