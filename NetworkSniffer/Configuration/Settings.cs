using System.Collections.Generic;

namespace NetworkSniffer.Configuration
{
    /// <summary>
    /// App settings class.
    /// </summary>
    public class Settings
    {
        public int Threshold { get; set; }
        public int Time { get; set; }
        public int TTL { get; set; }
        public int Port { get; set; }
    }
}
