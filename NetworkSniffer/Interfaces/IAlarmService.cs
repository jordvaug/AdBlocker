using System;
using System.Collections.Generic;

namespace NetworkSniffer.Interfaces
{
    public interface IAlarmService
    {
        public int TotalRequests { get; set; }
        public int Time { get; set; }
        public int Threshold { get; set; }
        public List<DateTime> Requests { get; set; }
        public bool GetAlarmState();
        public List<string> GetAlarms();
        public List<DateTime> GetRequests();
        public void PrintAlarms();
        public void AddAlarm(string alarm);
        public void AddRequest();
        public void RemoveOldRequests();
    }
}
