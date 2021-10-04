using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSniffer.Services;
using NetworkSniffer.Interfaces;
using Microsoft.Extensions.Options;
using NetworkSniffer.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace TestProject
{
    [TestClass]
    public class AlarmServiceUnitTests
    {
        private readonly IAlarmService _alarmService;

        public AlarmServiceUnitTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            IOptions<Settings> settings = Options.Create<Settings>(new Settings());

            _alarmService = new AlarmService(settings, memoryCache);
        }

        [TestMethod]
        public void TestAddAlarm()
        {

            string message = "Traffic exceeded threshold.";

            _alarmService.AddAlarm(message);

            Assert.AreEqual("Traffic exceeded threshold.", _alarmService.GetAlarms()[0]);
        }

        /// <summary>
        /// Test alarm trigger functionality.
        /// </summary>
        [TestMethod]
        public void TestTriggerAlarm()
        {
            _alarmService.Threshold = 1;
            _alarmService.Time = 1;

            _alarmService.AddRequest();

            Assert.AreEqual(true, _alarmService.GetAlarmState());
        }

        /// <summary>
        /// Test TTL functionality of requests
        /// </summary>
        [TestMethod]
        public void TestRequestTTL()
        {
            _alarmService.Time = 1;

            _alarmService.AddRequest();

            Thread.Sleep(2000);

            _alarmService.RemoveOldRequests();

            Assert.AreEqual(0, _alarmService.GetRequests().Count);
        }
    }
}
