using NetworkSniffer.Interfaces;
using System;


namespace NetworkSniffer
{
    class EntryPoint
    {
        private readonly IProxyService _proxyService;
        private readonly IScheduleService _scheduleService;
        public EntryPoint(IProxyService proxyService, IScheduleService scheduleService)
        {
            _proxyService = proxyService;
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Runner method to drive the application.
        /// </summary>
        /// <param name="args"></param>
        public void Run(String[] args)
        {
            Console.Title = "Network Sniffer";
            Console.WriteLine($"[{DateTime.Now}] Starting Network Sniffer");
            Console.WriteLine("Type 'quit' at any point to stop the program.");

            var proxy = _proxyService.GetProxyServer();
            _scheduleService.BeginSchedule();

            string command;
            while ((command = Console.ReadLine()) != "quit")
            {

            }

            proxy.Stop();
        }

    }
}
