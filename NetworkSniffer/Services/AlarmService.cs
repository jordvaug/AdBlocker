using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NetworkSniffer.Configuration;
using NetworkSniffer.Helpers;
using NetworkSniffer.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using _ = NetworkSniffer.Enums.Cache;

namespace NetworkSniffer.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Settings _settings;
        private volatile int _totalRequests;
        private volatile bool _inAlarm;

        public int TotalRequests
        {
            get => _totalRequests;
            set => _totalRequests = value;
        }
        public int Time { get; set; }
        public int Threshold { get; set; }
        public List<DateTime> Requests { get; set; }

        public int invokeCount = 0;
        public int maxCount = 1;


        public AlarmService(IOptions<Settings> settings, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            try
            {
                _settings = settings.Value;
                Time = _settings.Time;
                Threshold = _settings.Threshold;
            }
            catch (System.InvalidOperationException ex)
            {
                Log.Error(ex.Message);
            }
            catch (System.NullReferenceException e)
            {
                Time = 120;
                Threshold = 30;
                Log.Error(e.Message);
            }

            Requests = new List<DateTime>();
            TotalRequests = 0;

            Log.Debug($"Alarm service started with {Time} seconds intervals and a threshold of: {Threshold}");
        }


        /// <summary>
        /// Checks Requests list to remove any requests older than the time specified in Settings.
        /// </summary>
        public void RemoveOldRequests()
        {
            var reqs = GetRequests();

            Debug.Assert(reqs != null,
                            "Values array cannot be null");

            if (reqs.Count > 0)
            {
                for (var i = 0; i < reqs.Count; i++)
                {
                    if (reqs[i] < DateTime.Now.AddSeconds(Time * -1))
                    {
                        reqs.RemoveRange(i, 1);
                    }
                    else if (reqs[i] > DateTime.Now.AddSeconds(Time * -1))
                    {
                        break;
                    }
                }
            }
            SetRequests(reqs);
            IsInAlarmState();
        }

        /// <summary>
        /// Get Requests List from cache and set Requests property.
        /// </summary>
        public List<DateTime> GetRequests()
        {
            List<DateTime> existingRequests;
            if (_memoryCache.TryGetValue(_.Requests, out existingRequests))
            {
                Requests = existingRequests;
            }
            return Requests;
        }

        /// <summary>
        /// Increments number of requests and calls IsInAlarmState.
        /// </summary>
        public void AddRequest()
        {
            var req = DateTime.Now;
            List<DateTime> existingRequests;
            if (_memoryCache.TryGetValue(_.Requests, out existingRequests))
            {
                existingRequests.Add(req);
                SetRequests(existingRequests);
            }
            else
            {
                SetRequests(new List<DateTime> { req });
            }

            IsInAlarmState();
        }

        /// <summary>
        /// Set Requests in cache.
        /// </summary>
        /// <param name="reqs"></param>
        private void SetRequests(List<DateTime> reqs)
        {
            TotalRequests = reqs.Count;
            _memoryCache.Set(_.Requests, reqs);
        }

        /// <summary>
        /// Checks current total requests against the threshold and calls AddAlarm if an alarm has been triggered.
        /// </summary>
        public void IsInAlarmState()
        {
            if (!GetAlarmState())
            {
                if (TotalRequests >= Threshold)
                {
                    string alarm = $"High traffic generated an alert - hits = {TotalRequests}, triggered at {DateTime.Now}";
                    AddAlarm(alarm);
                    SetAlarmState(true);
                }
            }
            else
            {
                if (TotalRequests < Threshold)
                {
                    string alarm = $"Traffic returned to acceptable levels at: {DateTime.Now}";
                    AddAlarm(alarm);
                    SetAlarmState(false);
                }
            }
        }

        /// <summary>
        /// Gets Alarm State from cache.
        /// </summary>
        public bool GetAlarmState()
        {
            bool state;
            if (_memoryCache.TryGetValue(_.AlarmState, out state))
            {
                _inAlarm = state;
            }
            else
            {
                _inAlarm = false;
            }
            return _inAlarm;
        }

        /// <summary>
        /// Set the current state of the alarm.
        /// </summary>
        /// <param name="value"></param>
        private void SetAlarmState(bool value)
        {
            _memoryCache.Set(_.AlarmState, value);
        }

        /// <summary>
        /// Adds Alarm message to the list of alarms.
        /// </summary>
        /// <param name="alarm"></param>
        public void AddAlarm(string alarm)
        {
            List<string> existingAlarms;
            if (_memoryCache.TryGetValue(_.Alarms, out existingAlarms))
            {
                existingAlarms.Add(alarm);
                _memoryCache.Set(_.Alarms, existingAlarms);
            }
            else
            {
                _memoryCache.Set(_.Alarms, new List<string> { alarm });
            }

            Log.Information(alarm);
        }

        /// <summary>
        /// Gets the list of Alarm messages from cache.
        /// </summary>
        public List<string> GetAlarms()
        {
            List<string> existingAlarms;
            if (_memoryCache.TryGetValue(_.Alarms, out existingAlarms))
            {
                return existingAlarms;
            }
            return new List<string>();

        }


        /// <summary>
        /// Prints all of the alarms triggered by the traffic threshold being met.
        /// </summary>
        public void PrintAlarms()
        {
            var existingAlarms = GetAlarms();

            if (existingAlarms.Count > 0)
            {
                ConsoleUtilities.Reset();
                foreach (var message in existingAlarms)
                {
                    ConsoleUtilities.WriteAlarm(message);
                }
            }

            RemoveOldRequests();
        }
    }
}
