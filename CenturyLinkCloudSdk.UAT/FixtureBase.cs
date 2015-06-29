using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Owin;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public abstract class FixtureBase
    {
        private const string MockProxyBaseUri = "http://localhost:9000";
        private IDisposable _mockApi;
        protected CenturyLinkCloudServiceFactory _serviceFactory;

        [TestFixtureSetUp]
        public void FixtureUp()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            _mockApi = WebApp.Start(new StartOptions(MockProxyBaseUri), builder => builder.UseWebApi(config));

            _serviceFactory = new CenturyLinkCloudServiceFactory(string.Empty, string.Empty, new Uri(MockProxyBaseUri));
        }

        [TestFixtureTearDown]
        public void FixtureDown()
        {
            _mockApi.Dispose();
        }
    }
}