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
        private const string PolicyId = "policyid";
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
            var expectedUri = String.Format("alertpolicies/{0}", AccountAlias);
            var expectedBody = new AlertPolicyDefniition();
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.PostAsync<AlertPolicy>(expectedUri, expectedBody, expectedToken))
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

        [Test]
        public void Delete_PerformsCorrectRequest()
        {
            var expectedUri = String.Format("alertpolicies/{0}/{1}", AccountAlias, PolicyId);
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.DeleteAsync(expectedUri, expectedToken))
                   .Returns(Task.Run(() => { }));


            _testObject.Delete(PolicyId, expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void Delete_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Callback(() => tokenSource.Cancel(true))
                          .Returns(Task.FromResult(AccountAlias));

            Assert.Throws<TaskCanceledException>(() => _testObject.Delete(PolicyId, tokenSource.Token).Await());

            _client.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void Get_PerformsCorrectRequest()
        {
            var expectedUri = String.Format("alertpolicies/{0}", AccountAlias);
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.GetAsync<AlertPolicyCollection>(expectedUri, expectedToken))
                   .Returns(Task.FromResult(new AlertPolicyCollection()));

            _testObject.Get(expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void Get_ReturnsExpectedResult()
        {
            var expectedResult = new AlertPolicyCollection();

            _client.Setup(x => x.GetAsync<AlertPolicyCollection>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedResult));

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

            _client.Verify(x => x.GetAsync<AlertPolicyCollection>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Test]
        public void GetById_PerformsCorrectRequest()
        {
            var expectedUri = String.Format("alertpolicies/{0}/{1}", AccountAlias, PolicyId);
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.GetAsync<AlertPolicy>(expectedUri, expectedToken))
                   .Returns(Task.FromResult(new AlertPolicy()));

            _testObject.Get(PolicyId, expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void GetById_ReturnsExpectedResult()
        {
            var expectedResult = new AlertPolicy();

            _client.Setup(x => x.GetAsync<AlertPolicy>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

            _client.Verify(x => x.GetAsync<AlertPolicy>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        }
        
        [Test]
        public void Update_PerformsCorrectRequest()
        {
            var expectedUri = String.Format("alertpolicies/{0}/{1}", AccountAlias, PolicyId);
            var expectedBody = new AlertPolicyDefniition();
            var expectedToken = new CancellationTokenSource().Token;

            _client.Setup(x => x.PutAsync<AlertPolicy>(expectedUri, expectedBody, expectedToken))
                   .Returns(Task.FromResult(new AlertPolicy()));

            _testObject.Update(PolicyId, expectedBody, expectedToken).Wait();

            _client.VerifyAll();
        }

        [Test]
        public void Update_ReturnsExpectedResult()
        {
            var expectedResult = new AlertPolicy();

            _client.Setup(x => x.PutAsync<AlertPolicy>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedResult));

            var actualResult = _testObject.Update(PolicyId, new AlertPolicy(), CancellationToken.None).Result;

            Assert.AreSame(expectedResult, actualResult);
        }

        [Test]
        public void Update_Aborts_OnTokenCancellation()
        {
            var tokenSource = new CancellationTokenSource();

            _aliasProvider.Setup(x => x.GetAccountAlias())
                          .Callback(() => tokenSource.Cancel(true))
                          .Returns(Task.FromResult(AccountAlias));

            Assert.Throws<TaskCanceledException>(() => _testObject.Update(PolicyId, new AlertPolicy(), tokenSource.Token).Await());

            _client.Verify(x => x.PutAsync<AlertPolicy>(It.IsAny<string>(), It.IsAny<AlertPolicyDefniition>(), It.IsAny<CancellationToken>())
                                 , Times.Never);

        }
    }
}