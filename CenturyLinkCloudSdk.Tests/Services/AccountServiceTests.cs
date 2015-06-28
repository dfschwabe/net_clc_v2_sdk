using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;
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
            
            _client.Setup(x => x.GetAsync<DataCenter>(requestUri, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new DataCenter()));
            
            _testObject.GetAccountTotalAssets(new List<string> {CenterId1}, CancellationToken.None).Wait();

            _client.VerifyAll();
        }
    }
}