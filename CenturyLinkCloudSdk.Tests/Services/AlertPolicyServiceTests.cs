using System;
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
    public class AlertPolicyServiceTests
    {
        private const string AccountAlias = "alias";
        private AlertPolicyService _testObject;
        private Mock<IHttpClient> _client;
        private Mock<IAliasProvider> _aliasProvider;

        [SetUp]
        public void Setup()
        {
            _client = new Mock<IHttpClient>();
            _aliasProvider = new Mock<IAliasProvider>();
            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Returns(Task.FromResult(AccountAlias));
            _testObject = new AlertPolicyService(_client.Object, _aliasProvider.Object);
        }

        [Test]
        public void Create_PerformsCorrectRequest()
        {
            var requestUri = String.Format("alertpolicies/{0}", AccountAlias);
            var expectedBody = new AlertPolicyDefniition();
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.PostAsync<AlertPolicy>(requestUri, expectedBody, expectedToken))
                   .Returns(Task.FromResult(new AlertPolicy()));

            _testObject.Create(expectedBody, expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void Create_ReturnsExpectedResult()
        {
            var expectedResult = new AlertPolicy();

            _client.Setup(x => x.PostAsync<AlertPolicy>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedResult));

            var actualResult = _testObject.Create(new AlertPolicy(), CancellationToken.None).Result;

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void Create_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Callback(() => tokenSource.Cancel(true))
                          .Returns(Task.FromResult(AccountAlias));

            Assert.Throws<TaskCanceledException>(() => _testObject.Create(new AlertPolicy(), tokenSource.Token).Await());

            _client.Verify(x => x.PostAsync<AlertPolicy>(It.IsAny<string>(), It.IsAny<AlertPolicyDefniition>(), It.IsAny<CancellationToken>())
                                 , Times.Never);

        }
    }
}