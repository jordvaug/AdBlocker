using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSniffer.Services;
using NetworkSniffer.Interfaces;
using Microsoft.Extensions.Options;
using NetworkSniffer.Configuration;

namespace TestProject
{
    [TestClass]
    public class ProxyServiceUnitTests
    {
        private readonly IProxyService _proxyService;
        private readonly IRequestsService _requestsService;
        private readonly IAlarmService _alarmService;
        private readonly IFilterService _filterService;

        public ProxyServiceUnitTests()
        {
            IOptions<Settings> settings = Options.Create<Settings>(new Settings());
            _proxyService = new ProxyService(settings, _requestsService, _alarmService, _filterService);
        }

        [TestMethod]
        public void TestValidateUrl()
        {
            string url = "https://www.google.com";

            var result = _proxyService.ValidateUrl(url);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestValidateUrlFail()
        {
            string url = "google.com";

            var result = _proxyService.ValidateUrl(url);

            Assert.AreEqual(false, result);
        }

    }
}
