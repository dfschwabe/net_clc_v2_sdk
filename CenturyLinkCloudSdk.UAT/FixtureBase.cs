using System;
using System.Web.Http;
using CenturyLinkCloudSdk.UAT.Mock;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using Owin;

namespace CenturyLinkCloudSdk.UAT
{
    [TestFixture]
    public abstract class FixtureBase
    {
        private IDisposable _mockApi;
        private const string MockProxyBaseUri = "http://localhost:9000";
        protected User CurrentUser { get; set; }
        protected CenturyLinkCloudServiceFactory ServiceFactory { get; set; }

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

        protected void Given_I_Am(string username)
        {
            CurrentUser = Users.ByUsername[username];

            ServiceFactory = new CenturyLinkCloudServiceFactory(CurrentUser.Username, CurrentUser.Password, new Uri(MockProxyBaseUri));
        }
    }
}