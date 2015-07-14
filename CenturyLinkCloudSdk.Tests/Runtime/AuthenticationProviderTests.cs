using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;
using Moq;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.Tests.Runtime
{
    [TestFixture]
    public class AuthenticationProviderTests
    {
        private Mock<IHttpClient> _client;
        private AuthenticationProvider _testObject;
        private const string Username = "User";
        private const string Password = "Pass";

        [SetUp]
        public void Setup()
        {
            _client = new Mock<IHttpClient>();
            _testObject = new AuthenticationProvider(Username, Password, _client.Object);
        }

        [Test]
        public void GetAccountAlias_PerformsCorrectRequest()
        {
            var expectedUri = "authentication/login";
            LoginRequest actualContent = null;

            _client.Setup(x => x.PostAsync<Authentication>(expectedUri, It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
                   .Callback<string, object, CancellationToken>((uri, content, token) => actualContent = content as LoginRequest)
                   .Returns(Task.FromResult(new Authentication()));


            _testObject.GetAccountAlias().Wait();

            Assert.NotNull(actualContent);
            Assert.AreEqual(Username, actualContent.UserName);
            Assert.AreEqual(Password, actualContent.Password);
        }

        [Test]
        public void GetAccountAlias_ReturnsAccountAlias_FromResponseMessage()
        {
            var expectedAlias = "alias";

            _client.Setup(x => x.PostAsync<Authentication>(It.IsAny<string>(), It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(new Authentication{AccountAlias = expectedAlias}));


            var actualAlias = _testObject.GetAccountAlias().Result;

            Assert.AreEqual(expectedAlias, actualAlias);
        }

        [Test]
        public void GetBearerToken_ReturnsAccountAlias_FromResponseMessage()
        {
            var expectedToken = "token";

            _client.Setup(x => x.PostAsync<Authentication>(It.IsAny<string>(), It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(new Authentication { BearerToken = expectedToken }));


            var actualToken = _testObject.GetBearerToken().Result;

            Assert.AreEqual(expectedToken, actualToken);
        }

        [Test]
        public void Authentication_Is_Cached()
        {
            var expectedAuthentication = new Authentication {AccountAlias = "alias", BearerToken = "token"};
            _client.SetupSequence(x => x.PostAsync<Authentication>(It.IsAny<string>(), It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.FromResult(expectedAuthentication))
                   .Returns(Task.FromResult(new Authentication { AccountAlias = "other alias", BearerToken = "other token" }));


            _testObject.GetAccountAlias().Wait();
            _testObject.GetBearerToken().Wait();
            var actualAlias = _testObject.GetAccountAlias().Result;
            var actualToken = _testObject.GetBearerToken().Result;


            _client.Verify(x => x.PostAsync<Authentication>(It.IsAny<string>(), It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())
                , Times.Exactly(1));
            Assert.AreEqual(expectedAuthentication.AccountAlias, actualAlias);
            Assert.AreEqual(expectedAuthentication.BearerToken, actualToken);
        }
    }
}