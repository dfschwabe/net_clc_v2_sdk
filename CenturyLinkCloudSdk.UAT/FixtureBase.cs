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
        protected const string MockProxyBaseUri = "http://localhost:9000";
        private IDisposable _mockApi;

        [TestFixtureSetUp]
        public void FixtureUp()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            _mockApi = WebApp.Start(new StartOptions(MockProxyBaseUri), builder => builder.UseWebApi(config));

        }

        [TestFixtureTearDown]
        public void FixtureDown()
        {
            _mockApi.Dispose();
        }
    }
}