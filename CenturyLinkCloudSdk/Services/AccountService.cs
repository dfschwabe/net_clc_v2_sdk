﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.Models.Internal;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;

namespace CenturyLinkCloudSdk.Services
{
    public interface ICenturyLinkCloudAccountService
    {
        Task<TotalAssets> GetAccountTotalAssets(IEnumerable<string> dataCenterIds, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Activity>> GetRecentActivity(int recordCountLimit = 10, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Activity>> GetRecentActivityByAccountAlias(IEnumerable<string> aliases, int recordCountLimit = 10, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AccountService : ICenturyLinkCloudAccountService
    {
        private readonly IHttpClient _httpClient;
        private readonly IAliasProvider _aliasProvider;

        public AccountService(IHttpClient httpClient, IAliasProvider aliasProvider)
        {
            _httpClient = httpClient;
            _aliasProvider = aliasProvider;
        }

        public async Task<TotalAssets> GetAccountTotalAssets(IEnumerable<string> dataCenterIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var alias = await _aliasProvider.GetAccountAlias();

            return dataCenterIds.Select(async id => await GetDatacenter(alias, id, cancellationToken))
                                .Aggregate(new TotalAssets(), SumDatacenters);
        }

        public async Task<IEnumerable<Activity>> GetRecentActivity(int recordCountLimit = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var alias = await _aliasProvider.GetAccountAlias();

            cancellationToken.ThrowIfCancellationRequested();

            return await GetRecentActivityByAccountAlias(new []{alias}, recordCountLimit, cancellationToken);
        }

        public async Task<IEnumerable<Activity>> GetRecentActivityByAccountAlias(IEnumerable<string> aliases, int recordCountLimit = 10, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filter = new ActivityFilter {Accounts = aliases, Limit = recordCountLimit};
         
            return await _httpClient.PostAsync<IEnumerable<Activity>>("search/activities", filter, cancellationToken);
        }

        private async Task<DataCenter> GetDatacenter(string alias, string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await _httpClient.GetAsync<DataCenter>(String.Format("datacenters/{0}/{1}?totals=true", alias, id), cancellationToken);
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