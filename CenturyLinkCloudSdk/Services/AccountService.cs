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

            return dataCenterIds.Select(async id => await GetDatacenter(alias, id))
                                .Aggregate(new TotalAssets(), SumDatacenters);
        }

        private async Task<DataCenter> GetDatacenter(string alias, string id)
        {
            return await _httpClient.GetAsync<DataCenter>(String.Format("datacenters/{0}/{1}?totals=true", alias, id), CancellationToken.None);
        }

        private TotalAssets SumDatacenters(TotalAssets accumulator, Task<DataCenter> dc)
        {
            accumulator.Servers += dc.Result.Totals.Servers;
            accumulator.Cpus += dc.Result.Totals.Cpus;
            accumulator.MemoryGB += dc.Result.Totals.MemoryGB;
            accumulator.StorageGB += dc.Result.Totals.StorageGB;
            accumulator.Queue += dc.Result.Totals.Queue;

            return accumulator;
        }
    }
}