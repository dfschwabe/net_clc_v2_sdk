﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Runtime.Client;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CenturyLinkCloudSdk.Tests.Runtime.Client
{
    [TestFixture]
    public class HttpClientWrapperTests
    {
        private const string BaseAddress = "https://uri.com";
        private Mock<HttpClientHandler> _innerHandler;
        private HttpClient _innerClient;
        private HttpClientWrapper _testObject;

        [SetUp]
        public void Setup()
        {
            _innerHandler = new Mock<HttpClientHandler>();
            _innerClient = new HttpClient(_innerHandler.Object) {BaseAddress = new Uri(BaseAddress)};

            _testObject = new HttpClientWrapper(_innerClient);
        }

        [Test]
        public void GetAsync_PerformsCorrectRequest()
        {
            HttpRequestMessage actualRequest = null;

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(new HttpResponseMessage { Content = new StringContent(string.Empty) }))
                         .Callback<HttpRequestMessage, CancellationToken>((request, _) => actualRequest = request);

            _testObject.GetAsync<string>("path/id", CancellationToken.None).Wait();

            Assert.NotNull(actualRequest);
            Assert.AreEqual(HttpMethod.Get, actualRequest.Method);
            Assert.AreEqual(BaseAddress + "/path/id", actualRequest.RequestUri.ToString());
        }

        [Test]
        public void GetAsync_DeserializesResponse()
        {
            var expected = new Poco {P1 = "value1"};

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(expected)) }));


            var actual = _testObject.GetAsync<Poco>("path/id", CancellationToken.None).Result;

            Assert.AreEqual(expected.P1, actual.P1);
        }

        [Test]
        public void GetAsync_Throws_OnFailedRequest()
        {
            var responseMessage = new HttpRequestMessage(HttpMethod.Get, "path/id")
                                    .CreateResponse(HttpStatusCode.NotFound);

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(responseMessage));

            try
            {
                _testObject.GetAsync<Poco>(string.Empty, CancellationToken.None).Await();
                
                Assert.Fail();
            }
            catch (CloudServiceException ex)
            {
                Assert.AreEqual(responseMessage.StatusCode, ex.StatusCode);
                Assert.AreEqual(responseMessage.ReasonPhrase, ex.ReasonPhrase);
                Assert.AreEqual("GET:" + "path/id", ex.Request);
            }
        }

        [Test]
        public void PostAsync_PerformsCorrectRequest()
        {
            HttpRequestMessage actualRequest = null;

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(new HttpResponseMessage { Content = new StringContent(string.Empty) }))
                         .Callback<HttpRequestMessage, CancellationToken>((request, _) => actualRequest = request);

            _testObject.PostAsync<string, string>("path/id", string.Empty, CancellationToken.None).Wait();

            Assert.NotNull(actualRequest);
            Assert.AreEqual(HttpMethod.Post, actualRequest.Method);
            Assert.AreEqual(BaseAddress + "/path/id", actualRequest.RequestUri.ToString());
        }

        [Test]
        public void PostAsync_PostsCorrectContent()
        {
            Poco actualBody = null;
            var expectedBody = new Poco {P1 = "request"};
            

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(new HttpResponseMessage { Content = new StringContent(string.Empty) }))
                         .Callback<HttpRequestMessage, CancellationToken>((request, _) => actualBody = request.ReadContentAs<Poco>());

            _testObject.PostAsync<Poco, string>("path/id", expectedBody, CancellationToken.None).Wait();

            Assert.NotNull(actualBody);
            Assert.AreEqual(expectedBody.P1, actualBody.P1);
        }

        [Test]
        public void PostAsync_DeserializesResponse()
        {
            var expectedResponseBody = new Poco { P1 = "value1" };

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(expectedResponseBody)) }));


            var actual = _testObject.PostAsync<Poco, Poco>("path/id", new Poco(), CancellationToken.None).Result;

            Assert.AreEqual(expectedResponseBody.P1, actual.P1);

        }

        [Test]
        public void PostAsync_OnFailedRequest()
        {
            var responseMessage = new HttpRequestMessage(HttpMethod.Post, "path/id")
                                    .CreateResponse(HttpStatusCode.NotFound);

            _innerHandler.Protected()
                         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                         .Returns(Task.FromResult(responseMessage));

            try
            {
                _testObject.PostAsync<Poco, Poco>(string.Empty, new Poco(),  CancellationToken.None).Await();

                Assert.Fail();
            }
            catch (CloudServiceException ex)
            {
                Assert.AreEqual(responseMessage.StatusCode, ex.StatusCode);
                Assert.AreEqual(responseMessage.ReasonPhrase, ex.ReasonPhrase);
                Assert.AreEqual("POST:" + "path/id", ex.Request);
            }
        }
    }

    public class Poco
    {
        public string P1 { get; set; }
    }
}