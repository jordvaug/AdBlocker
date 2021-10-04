using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSniffer.Services;
using NetworkSniffer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace TestProject
{
    [TestClass]
    public class RequestsServiceUnitTests
    {
        private readonly IRequestsService _requestsService;

        public RequestsServiceUnitTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            _requestsService = new RequestsService(memoryCache);
        }

        [TestMethod]
        public void TestAddRequest()
        {
            string url = "http://www.google.com/news/stories";
            string method = "GET";

            _requestsService.AddRequest(url, method);

            Assert.AreEqual(1, _requestsService.NumberOfPages());
        }

        [TestMethod]
        public void TestClearRequests()
        {
            string url = "http://www.google.com/news/stories";
            string method = "GET";
            string method2 = "PUT";

            _requestsService.AddRequest(url, method);
            _requestsService.AddRequest(url, method2);
            _requestsService.ClearRequests();

            var numRequests = _requestsService.NumberOfRequests();

            Assert.AreEqual(0, numRequests);
        }

        [TestMethod]
        public void TestFindTopSection()
        {
            string url = "http://www.google.com/news/stories";
            string method = "GET";
            string url2 = "http://www.google.com/news/events";
            string url3 = "http://www.google.com/mail/top";

            _requestsService.ClearRequests();
            _requestsService.AddRequest(url, method);
            _requestsService.AddRequest(url2, method);
            _requestsService.AddRequest(url3, method);

            string section;
            (section, _) = _requestsService.FindTopSection();

            Assert.AreEqual("news", section);
        }

        [TestMethod]
        public void TestTypeOfRequests()
        {
            string url = "http://www.google.com/news/stories";
            string method = "GET";
            string method2 = "PUT";

            _requestsService.AddRequest(url, method);
            _requestsService.AddRequest(url, method2);

            var results = _requestsService.TypeOfRequests();

            Assert.AreEqual(1, results[0]);
        }
        

        [TestMethod]
        public void TestNumberOfRequests()
        {
            string url = "http://www.google.com/news/stories";
            string method = "GET";
            string method2 = "PUT";

            _requestsService.ClearRequests();
            _requestsService.AddRequest(url, method);
            _requestsService.AddRequest(url, method2);


            var numRequests = _requestsService.NumberOfRequests();

            Assert.AreEqual(2, numRequests);
        }
    }
}
