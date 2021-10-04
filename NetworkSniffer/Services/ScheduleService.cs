using Microsoft.Extensions.Options;
using NetworkSniffer.Configuration;
using NetworkSniffer.Interfaces;
using Serilog;
using System;

namespace NetworkSniffer.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly Settings _settings;
        private readonly IRequestsService _requestsService;
        private readonly IAlarmService _alarmService;
        private System.Timers.Timer _timer;
        private const int INTERVAL = 1000;

        public int timer { get; set; }
        private int TTL { get; set; }

        public ScheduleService(IRequestsService requestsService, IAlarmService alarmService, IOptions<Settings> settings)
        {
            _requestsService = requestsService;
            _alarmService = alarmService;
            _settings = settings.Value;
            timer = 0;
            TTL = _settings.TTL;
        }

        /// <summary>
        /// Starts the scheduler to run the printer.
        /// </summary>
        public void BeginSchedule()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = INTERVAL;

            try
            {
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(SchedulePrint);
                _timer.Start();
            }
            catch(ArgumentOutOfRangeException ex)
            {
                _timer.Interval = 1000;
                _timer.Start();
                Log.Error(ex.Message);
            }
            catch(ArgumentException e)
            {
                _timer.Interval = 1000;
                _timer.Start();
                Log.Error(e.Message);
            }

            Log.Debug($"Schedule set to interval of {INTERVAL}");
        }

        /// <summary>
        /// Delegate function used to print alarms and request summaries.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SchedulePrint(object sender, System.Timers.ElapsedEventArgs e)
        {
            _alarmService.PrintAlarms();

            timer++;
            if (timer == TTL)
            {
                _requestsService.PrintRequests();
                _requestsService.ClearRequests();
                timer = 0;
            }
        }
    }
}
