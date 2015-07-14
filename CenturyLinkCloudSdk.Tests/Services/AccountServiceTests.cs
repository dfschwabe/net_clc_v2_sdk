using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;
using CenturyLinkCloudSdk.Models.Internal;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;
using CenturyLinkCloudSdk.Services;
using Moq;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.Tests.Services
{
    [TestFixture]
    public class AccountServiceTests
    {
        private const string AccountAlias = "alias";
        private const string CenterId1 = "dc1";
        private const string CenterId2 = "dc2";
        private Mock<IHttpClient> _client;
        private Mock<IAliasProvider> _aliasProvider;
        private AccountService _testObject;

        [SetUp]
        public void Setup()
        {
            _client = new Mock<IHttpClient>();

            _aliasProvider = new Mock<IAliasProvider>();
            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Returns(Task.FromResult(AccountAlias));

            _testObject = new AccountService(_client.Object, _aliasProvider.Object);
        }

        [Test]
        public void GetAccountTotalAssets_PerformsCorrectRequest()
        {
            var requestUri = String.Format("datacenters/{0}/{1}?totals=true", AccountAlias, CenterId1);
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.GetAsync<DataCenter>(requestUri, expectedToken))
                   .Returns(Task.FromResult(new DataCenter{Totals = new TotalAssets()}));

            _testObject.GetAccountTotalAssets(new List<string> { CenterId1 }, expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void GetAccountTotalAssets_AggregatesAcrossDataCenters()
        {
            var dc1 = new TotalAssets { Servers = 1, Cpus = 1, MemoryGB = 1, StorageGB = 1, Queue = 1 };
            _client.Setup(x => x.GetAsync<DataCenter>(String.Format("datacenters/{0}/{1}?totals=true", AccountAlias, CenterId1), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(new DataCenter{ Totals = dc1 }));

            var dc2 = new TotalAssets { Servers = 2, Cpus = 2, MemoryGB = 2, StorageGB = 2, Queue = 2 };
            _client.Setup(x => x.GetAsync<DataCenter>(String.Format("datacenters/{0}/{1}?totals=true", AccountAlias, CenterId2), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(new DataCenter{ Totals = dc2 }));


            var result =_testObject.GetAccountTotalAssets(new List<string> {CenterId1, CenterId2}, CancellationToken.None).Result;

            Assert.AreEqual(3, result.Servers);
            Assert.AreEqual(3, result.Cpus);
            Assert.AreEqual(3, result.MemoryGB);
            Assert.AreEqual(3, result.StorageGB);
            Assert.AreEqual(3, result.Queue);
        }

        [Test]
        public void GetAccountTotalAssets_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _client.Setup(x => x.GetAsync<DataCenter>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback<string,CancellationToken>((uri,token) => tokenSource.Cancel(true))
                .Returns(Task.FromResult(new DataCenter { Totals = new TotalAssets() }));


            Assert.Throws<AggregateException>(() =>_testObject.GetAccountTotalAssets(new List<string> { CenterId1, CenterId2 }, tokenSource.Token).Await());

            _client.Verify(x => x.GetAsync<DataCenter>(It.IsAny<string>(), tokenSource.Token), Times.Once);
        }

        [Test]
        public void GetRecentActivity_RequestsActivity_ForAuthenticatedUser()
        {
            ActivityFilter expectedFilter = new ActivityFilter { Accounts = new[] {AccountAlias}, Limit = 5};
            ActivityFilter actualFilter = null;
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.PostAsync<IEnumerable<Activity>>("search/activities", It.IsAny<ActivityFilter>(), expectedToken))
                   .Callback<string, object, CancellationToken>((uri, filter, token) => actualFilter = filter as ActivityFilter)
                   .Returns(Task.FromResult(new List<Activity>().AsEnumerable()));

            _testObject.GetRecentActivity(expectedFilter.Limit, expectedToken).Wait();

            _client.VerifyAll();
            Assert.NotNull(actualFilter);
            CollectionAssert.AreEqual(expectedFilter.Accounts, actualFilter.Accounts);
            Assert.AreEqual(expectedFilter.Limit, actualFilter.Limit);
        }

        [Test]
        public void GetRecentActivity_ReturnsExpectedResult()
        {
            var expectedResult = new List<Activity>().AsEnumerable();

            _client.Setup(x => x.PostAsync<IEnumerable<Activity>>(It.IsAny<string>(), It.IsAny<ActivityFilter>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedResult));

            var actualResult = _testObject.GetRecentActivity(1, CancellationToken.None).Result;

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetRecentActivity_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Callback(() => tokenSource.Cancel(true))
                          .Returns(Task.FromResult(AccountAlias));

            Assert.Throws<TaskCanceledException>(() => _testObject.GetRecentActivity(5, tokenSource.Token).Await());

            _client.Verify(x => x.PostAsync<IEnumerable<Activity>>(It.IsAny<string>(), It.IsAny<ActivityFilter>(), It.IsAny<CancellationToken>())
                                 , Times.Never);
        }

        [Test]
        public void GetRecentActivityByAccountAlias_PerformsCorrectRequest()
        {
            ActivityFilter expectedFilter = new ActivityFilter { Accounts = new[] {"alias1", "alias2"}, Limit = 5};
            ActivityFilter actualFilter = null;
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.PostAsync<IEnumerable<Activity>>("search/activities", It.IsAny<ActivityFilter>(), expectedToken))
                   .Callback<string, object, CancellationToken>((uri, filter, token) => actualFilter = filter as ActivityFilter)
                   .Returns(Task.FromResult(new List<Activity>().AsEnumerable()));

            _testObject.GetRecentActivityByAccountAlias(expectedFilter.Accounts, expectedFilter.Limit, expectedToken).Wait();

            _client.VerifyAll();
            Assert.NotNull(actualFilter);
            CollectionAssert.AreEqual(expectedFilter.Accounts, actualFilter.Accounts);
            Assert.AreEqual(expectedFilter.Limit, actualFilter.Limit);
        }

        [Test]
        public void GetRecentActivityByAccountAlias_ReturnsExpectedResult()
        {
            var expectedResult = new List<Activity>().AsEnumerable();

            _client.Setup(x => x.PostAsync<IEnumerable<Activity>>(It.IsAny<string>(), It.IsAny<ActivityFilter>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedResult));

            var actualResult = _testObject.GetRecentActivityByAccountAlias(new List<string>(), 1, CancellationToken.None).Result;

            Assert.AreSame(expectedResult, actualResult);
        }
    }
}