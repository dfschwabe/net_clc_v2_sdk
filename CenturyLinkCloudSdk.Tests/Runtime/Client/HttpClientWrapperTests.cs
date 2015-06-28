﻿using System;
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
            _innerClient = new HttpClient(_innerHandler.Object);
            _innerClient.BaseAddress = new Uri(BaseAddress);

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
    }
}