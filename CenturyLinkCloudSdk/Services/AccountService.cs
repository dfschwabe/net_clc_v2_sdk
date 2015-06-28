using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;

namespace CenturyLinkCloudSdk.Services
{
    public class AccountService
    {
        private readonly IHttpClient _httpClient;
        private readonly IAliasProvider _aliasProvider;

        public AccountService(IHttpClient httpClient, IAliasProvider aliasProvider)
        {
            _httpClient = httpClient;
            _aliasProvider = aliasProvider;
        }

        public async Task<TotalAssets> GetAccountTotalAssets(IEnumerable<string> dataCenterIds, CancellationToken cancellationToken)
        {
            var alias = await _aliasProvider.GetAccountAlias();

            await _httpClient.GetAsync<DataCenter>(String.Format("datacenters/{0}/{1}?totals=true", alias, dataCenterIds.First()), CancellationToken.None);

            return new TotalAssets();
        }
    }
}