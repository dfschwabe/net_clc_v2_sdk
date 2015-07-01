using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.Services;
using CenturyLinkCloudSdk.UAT.Mock;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public class AccountServiceTests : FixtureBase
    {
        private ICenturyLinkCloudAccountService _accountService;
        private List<string> _dataCenterIds;
        private TotalAssets _assetTotals;
        private User _currentUser;

        [TestCase(Users.A)]
        [TestCase(Users.B)]
        public void GetAccountTotalAssets_CalculatesDataCenters_ForAccountAlias(string username)
        {
            Given_I_Am(username);
            
            When_I_Request_Asset_Totals();

            Then_I_Receive_Totals_For_My_Data_Centers();
        }

        private void Given_I_Am(string username)
        {
            _currentUser = Users.ByUsername[username];

            var serviceFactory = new CenturyLinkCloudServiceFactory(_currentUser.Username, _currentUser.Password, new Uri(MockProxyBaseUri));
            _accountService = serviceFactory.CreateAccountService();
            _dataCenterIds = _currentUser.DataCentersById.Keys.ToList();
        }

        private void When_I_Request_Asset_Totals()
        {
            _assetTotals = _accountService.GetAccountTotalAssets(_dataCenterIds, CancellationToken.None).Result;
        }

        private void Then_I_Receive_Totals_For_My_Data_Centers()
        {
            var expectedServers = _currentUser.DataCentersById.Keys
                                              .Select(k => _currentUser.DataCentersById[k])
                                              .Aggregate(0, (accum, dc) => accum += dc.Totals.Servers);

            Assert.AreEqual(expectedServers, _assetTotals.Servers);
        }
    }
}