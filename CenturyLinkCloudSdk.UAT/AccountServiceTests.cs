using System;
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
            Given_I_Have_Data_Centers();

            When_I_Request_Asset_Totals();

            Then_I_Receive_Totals_For_My_Data_Centers();
        }

        [Test]
        public void GetRecentActivity_RequestsLimitedRecords_ForSpecifiedUsers()
        {
            Given_I_Am(Users.A);
            Given_There_Is_Recent_Activity();

            When_I_Request_Activity_For(Users.A, Users.B);
            
            Then_I_Recieve_Activity_For(Users.A, Users.B);
        }

        private void Given_I_Have_Data_Centers()
        {
            Users.UserA.DataCentersById = new Dictionary<string, MockDataCenter>
            {
                {DataCenters.DCA.Id, DataCenters.DCA},
                {DataCenters.DCB.Id, DataCenters.DCB},
            };

            Users.UserB.DataCentersById = new Dictionary<string, MockDataCenter>
            {
                {DataCenters.DCB.Id, DataCenters.DCB},
            };
        }

        private void Given_There_Is_Recent_Activity()
        {
            Users.UserA.RecentActivity = new List<MockActivity>
            {
                new MockActivity
                {
                    AccountAlias = "aliasA",
                    AccountDescription = "CLC Virtual Block Storage",
                    Body = "Roles updated to: AccountAdmin",
                    CreatedBy = "admin",
                    CreatedDate = DateTime.Now,
                    EntityId = 1,
                    EntityType = "User",
                    LocationAlias = "VA1",
                    ReferenceId = "VA1aliasACI01",
                    Subject = "Server VA1T3BKCI01 Configuration Updated"
                }
            };

            Users.UserB.RecentActivity = new List<MockActivity>
            {
                new MockActivity
                {
                    AccountAlias = "aliasB",
                    AccountDescription = "CLC Virtual Block Storage",
                    Body = "Server X Deleted by admin",
                    CreatedBy = "admin",
                    CreatedDate = DateTime.Now,
                    EntityId = 2,
                    EntityType = "Server",
                    LocationAlias = "VA1",
                    ReferenceId = "VA1aliasBCI01",
                    Subject = "Server VA1T3BKCI01 Configuration Updated"
                }
            };
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