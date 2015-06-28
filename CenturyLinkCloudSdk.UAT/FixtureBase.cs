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
        private IDisposable _mockApi;

        [TestFixtureSetUp]
        public void FixtureUp()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            _mockApi = WebApp.Start(new StartOptions("http://localhost:9000/"), builder => builder.UseWebApi(config));
        }

        [TestFixtureTearDown]
        public void FixtureDown()
        {
            _mockApi.Dispose();
        }
    }
}