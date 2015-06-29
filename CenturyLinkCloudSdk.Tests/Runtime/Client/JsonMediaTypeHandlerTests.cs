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

        [Test]
        public void SendAsync_AddsContentTypeHeader_WhenContentNotNull()
        {
            var request = new HttpRequestMessage{Content = new StringContent(string.Empty)};
            var innerHandler = new Mock<HttpClientHandler>();

            innerHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                        .Returns(Task.FromResult(new HttpResponseMessage()));

            new HttpMessageInvoker(new JsonMediaTypeHandler() { InnerHandler = innerHandler.Object })
                .SendAsync(request, CancellationToken.None)
                .Wait();

            Assert.AreEqual(1, request.Content.Headers.Count());
            Assert.AreEqual("application/json; charset=utf-8", request.Content.Headers.GetValues("Content-Type").FirstOrDefault());
        }
    }
}