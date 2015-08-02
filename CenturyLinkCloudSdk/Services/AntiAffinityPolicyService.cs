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
    public interface ICenturyLinkCloudAntiAffinityPolicyService
    {
        Task<AntiAffinityPolicy> Create(AntiAffinityPolicyDefinition definition, CancellationToken cancellationToken = default(CancellationToken));
        Task Delete(string policyId, CancellationToken cancellationToken = default(CancellationToken));
        Task<AntiAffinityPolicy> Get(string policyId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<AntiAffinityPolicy>> Get(CancellationToken cancellationToken = default(CancellationToken));
        Task<AntiAffinityPolicy> Update(string policyId, string newName, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AntiAffinityPolicyService : ICenturyLinkCloudAntiAffinityPolicyService
    {
        private const string Api = "antiaffinitypolicies";
        private readonly IHttpClient _httpClient;
        private readonly IAliasProvider _aliasProvider;

        public AntiAffinityPolicyService(IHttpClient httpClient, IAliasProvider aliasProvider)
        {
            _httpClient = httpClient;
            _aliasProvider = aliasProvider;
        }

        public async Task<AntiAffinityPolicy> Create(AntiAffinityPolicyDefinition definition, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(cancellationToken);

            return await _httpClient.PostAsync<AntiAffinityPolicy>(requestUri, definition, cancellationToken);
        }

        public async Task Delete(string policyId, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(policyId, cancellationToken);

            await _httpClient.DeleteAsync(requestUri, cancellationToken);
        }

        public async Task<AntiAffinityPolicy> Get(string policyId, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(policyId, cancellationToken);

            return await _httpClient.GetAsync<AntiAffinityPolicy>(requestUri, cancellationToken);
        }

        public async Task<List<AntiAffinityPolicy>> Get(CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(cancellationToken);

            var result = await _httpClient.GetAsync<ModelCollection<AntiAffinityPolicy>>(requestUri, cancellationToken);

            return result.Items;
        }

        public async Task<AntiAffinityPolicy> Update(string policyId, string newName, CancellationToken cancellationToken = new CancellationToken())
        {
            var requestUri = await GetUri(policyId, cancellationToken);

            var body = new AntiAffinityPolicyUpdate{Name = newName};

            return await _httpClient.PutAsync<AntiAffinityPolicy>(requestUri, body, cancellationToken);
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