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

            return dataCenterIds
                        .Select(async id => await _httpClient.GetAsync<DataCenter>(String.Format("datacenters/{0}/{1}?totals=true", alias, id), CancellationToken.None))
                        .Aggregate(new TotalAssets(), (accum, dc) =>
                        {
                            accum.Servers += dc.Result.Totals.Servers;
                            accum.Cpus += dc.Result.Totals.Cpus;
                            accum.MemoryGB += dc.Result.Totals.MemoryGB;
                            accum.StorageGB += dc.Result.Totals.StorageGB;
                            accum.Queue += dc.Result.Totals.Queue;

                            return accum;
                        });
        }
    }
}