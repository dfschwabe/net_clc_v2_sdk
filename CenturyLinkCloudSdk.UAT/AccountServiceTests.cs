using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.UAT.Mock;
using CenturyLinkCloudSdk.UAT.Mock.Controllers;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public class AccountServiceTests : FixtureBase
    {
        private List<string> _dataCenterIds;
        private TotalAssets _assetTotals;
        private IEnumerable<Activity> _activity;

        [SetUp]
        public void Setup()
        {
            _assetTotals = null;
            _activity = null;
        }

        [TestCase(Users.A)]
        [TestCase(Users.B)]
        public void GetAccountTotalAssets_CalculatesDataCenters_ForAccountAlias(string username)
        {
            Given_I_Am(username);
            
            When_I_Request_Asset_Totals();

            Then_I_Receive_Totals_For_My_Data_Centers();
        }

        [Test]
        public void GetRecentActivity_RequestsLimitedRecords_ForSpecifiedUsers()
        {
            Given_I_Am(Users.A);

            When_I_Request_Activity_For(Users.A, Users.B);
            
            Then_I_Recieve_Activity_For(Users.A, Users.B);
        }

        private void When_I_Request_Activity_For(params string[] usernames)
        {
            var accounts = usernames.Select(u => Users.ByUsername[u].AccountAlias).ToList();
            _activity = ServiceFactory.CreateAccountService()
                                      .GetRecentActivityByAccountAlias(accounts, SearchController.ExpectedLimit, CancellationToken.None).Result;
        }

        private void When_I_Request_Asset_Totals()
        {
            _dataCenterIds = CurrentUser.DataCentersById.Keys.ToList();
            _assetTotals = ServiceFactory.CreateAccountService()
                                         .GetAccountTotalAssets(_dataCenterIds, CancellationToken.None).Result;
        }

        private void Then_I_Receive_Totals_For_My_Data_Centers()
        {
            var expectedServers = CurrentUser.DataCentersById
                                             .Keys
                                             .Select(k => CurrentUser.DataCentersById[k])
                                             .Aggregate(0, (accum, dc) => accum + dc.Totals.Servers);

            Assert.AreEqual(expectedServers, _assetTotals.Servers);
        }

        private void Then_I_Recieve_Activity_For(params string[] usernames)
        {
            var expectedActivity = usernames.SelectMany(u => Users.ByUsername[u].RecentActivity).ToList();

            Assert.AreEqual(expectedActivity.Count(), _activity.Count());
            Assert.True(expectedActivity.All(e => _activity.Any(a => a.Body.Equals(e.Body))));
        }
    }
}