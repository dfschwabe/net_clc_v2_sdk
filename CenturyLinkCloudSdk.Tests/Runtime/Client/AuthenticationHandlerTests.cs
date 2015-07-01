using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Runtime;
using CenturyLinkCloudSdk.Runtime.Client;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.Tests.Runtime.Client
{
    [TestFixture]
    public class AuthenticationHandlerTests
    {
        [Test]
        public void SendAsync_AddsAcceptHeader()
        {
            var innerHandler = new Mock<HttpClientHandler>();
            var authProvider = new Mock<ITokenProvider>();
            var testObject = new AuthenticationHandler(authProvider.Object) {InnerHandler = innerHandler.Object};
            var request = new HttpRequestMessage();
            

            innerHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                        .Returns(Task.FromResult(new HttpResponseMessage()));
            authProvider.Setup(x=>x.GetBearerToken())
                        .Returns(Task.FromResult("token value"));


            new HttpMessageInvoker(testObject)
                .SendAsync(request, CancellationToken.None)
                .Wait();


            Assert.NotNull(request.Headers.Authorization);
            Assert.AreEqual("Bearer", request.Headers.Authorization.Scheme);
            Assert.AreEqual("token value", request.Headers.Authorization.Parameter);
        } 
    }
}