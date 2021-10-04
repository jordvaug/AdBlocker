using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSniffer.Services;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace TestProject
{
    [TestClass]
    public class FilterServiceUnitTests
    {
        private readonly IFilterService _filterService;

        public FilterServiceUnitTests()
        {
            var settings = new AdSites()
            {
                Sites = new HashSet<string>()
            };
            settings.Sites.Add("bounceexchange.com");
            IOptions<AdSites> settingsOptions = Options.Create(settings);
            _filterService = new FilterService(settingsOptions);
        }

        [TestMethod]
        public void TestAddRequest()
        {
            string url = "https://images.outbrainimg.com/transform/v3/eyJpdSI6IjBlNTBlMzFhNWFhOTEyY2VlYWVkNWEzMzhmNzEwNzBmN2E5NWQzNTdkZTJmZWJjNDM2Y2IyN2ZhMjliMzZkZGEiLCJ3IjozODEsImgiOjI1NCwiZCI6MS4wLCJjcyI6MCwiZiI6NH0.webp";

            var result = _filterService.ValidateRequest(url);

            Assert.AreEqual(true, result);
        }

    }
}
