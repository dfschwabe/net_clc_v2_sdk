using System;
using System.Collections.Generic;
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
    public class AntiAffinityPolicyServiceTests
    {
        private const string AccountAlias = "alias";
        private const string PolicyId = "policyid";
        private AntiAffinityPolicyService _testObject;
        private Mock<IHttpClient> _client;
        private Mock<IAliasProvider> _aliasProvider;

        [SetUp]
        public void Setup()
        {
            _client = new Mock<IHttpClient>();
            _aliasProvider = new Mock<IAliasProvider>();
            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Returns(Task.FromResult(AccountAlias));
            _testObject = new AntiAffinityPolicyService(_client.Object, _aliasProvider.Object);
        }

        [Test]
        public void Get_PerformsCorrectRequest()
        {
            var expectedUri = String.Format("antiaffinitypolicies/{0}", AccountAlias);
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.GetAsync<ModelCollection<AntiAffinityPolicy>>(expectedUri, expectedToken))
                   .Returns(Task.FromResult(new ModelCollection<AntiAffinityPolicy>()));

            _testObject.Get(expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void Get_ReturnsExpectedResult()
        {
            var expectedResult = new List<AntiAffinityPolicy>();

            _client.Setup(x => x.GetAsync<ModelCollection<AntiAffinityPolicy>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(new ModelCollection<AntiAffinityPolicy>{ Items = expectedResult }));

            var actualResult = _testObject.Get(CancellationToken.None).Result;

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void Get_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Callback(() => tokenSource.Cancel(true))
                          .Returns(Task.FromResult(AccountAlias));

            Assert.Throws<TaskCanceledException>(() => _testObject.Get(tokenSource.Token).Await());

            _client.Verify(x => x.GetAsync<ModelCollection<AntiAffinityPolicy>>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Test]
        public void GetById_PerformsCorrectRequest()
        {
            var expectedUri = String.Format("antiaffinitypolicies/{0}/{1}", AccountAlias, PolicyId);
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.GetAsync<AntiAffinityPolicy>(expectedUri, expectedToken))
                   .Returns(Task.FromResult(new AntiAffinityPolicy()));

            _testObject.Get(PolicyId, expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void GetById_ReturnsExpectedResult()
        {
            var expectedResult = new AntiAffinityPolicy();

            _client.Setup(x => x.GetAsync<AntiAffinityPolicy>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedResult));

            var actualResult = _testObject.Get(PolicyId, CancellationToken.None).Result;

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void GetById_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Callback(() => tokenSource.Cancel(true))
                          .Returns(Task.FromResult(AccountAlias));

            Assert.Throws<TaskCanceledException>(() => _testObject.Get(PolicyId, tokenSource.Token).Await());

            _client.Verify(x => x.GetAsync<AntiAffinityPolicy>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}