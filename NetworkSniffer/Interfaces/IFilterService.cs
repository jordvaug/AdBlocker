using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkSniffer.Interfaces
{
    public interface IFilterService
    {
        public bool ValidateRequest(string url);
        public HashSet<string> AdList { get; set; }

    }
}
