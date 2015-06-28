using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Runtime.Client;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.Tests.Runtime.Client
{
    [TestFixture]
    public class JsonMediaTypeHandlerTests
    {
        [Test]
        public void SendAsync_AddsAcceptHeader()
        {
            var request = new HttpRequestMessage();
            var innerHandler = new Mock<HttpClientHandler>();

            innerHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                        .Returns(Task.FromResult(new HttpResponseMessage()));

            new HttpMessageInvoker(new JsonMediaTypeHandler() { InnerHandler = innerHandler.Object })
                .SendAsync(request, CancellationToken.None)
                .Wait();

            Assert.Greater(request.Headers.Accept.Count, 0);
            Assert.AreEqual("application/json", request.Headers.Accept.First().MediaType);
        }
    }
}