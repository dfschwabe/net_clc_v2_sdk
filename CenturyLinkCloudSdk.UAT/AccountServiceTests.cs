using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.UAT.Mock;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public class AccountServiceTests : FixtureBase
    {
        private List<string> _dataCenterIds;
        private TotalAssets _assetTotals;

        [TestCase(Users.A)]
        [TestCase(Users.B)]
        public void GetAccountTotalAssets_CalculatesDataCenters_ForAccountAlias(string username)
        {
            Given_I_Am(username);
            
            When_I_Request_Asset_Totals();

            Then_I_Receive_Totals_For_My_Data_Centers();
        }

        private void When_I_Request_Asset_Totals()
        {
            _dataCenterIds = CurrentUser.DataCentersById.Keys.ToList();
            _assetTotals = ServiceFactory.CreateAccountService()
                                          .GetAccountTotalAssets(_dataCenterIds, CancellationToken.None).Result;
        }

        private void Then_I_Receive_Totals_For_My_Data_Centers()
        {
            var expectedServers = CurrentUser.DataCentersById.Keys
                                              .Select(k => CurrentUser.DataCentersById[k])
                                              .Aggregate(0, (accum, dc) => accum + dc.Totals.Servers);

            Assert.AreEqual(expectedServers, _assetTotals.Servers);
        }
    }
}