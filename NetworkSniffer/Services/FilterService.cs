using Microsoft.Extensions.Options;
using NetworkSniffer.Configuration;
using NetworkSniffer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace NetworkSniffer.Services
{
    public class FilterService: IFilterService
    {
        private readonly AdSites _settings;
        public HashSet<string> AdList { get; set; }
        public FilterService(IOptions<AdSites> settings)
        {
            _settings = settings.Value;
            AdList = _settings.Sites;
        }

        public bool ValidateRequest(string url)
        {
            var urlArray = url.Split('.');
            StringBuilder domain = new StringBuilder(urlArray[1]);
            domain.Append(".");
            domain.Append(urlArray[2].Split('/')[0]);

            if(AdList == null)
            {
                return false;
            }
            var result = AdList.Contains(domain.ToString());
            if(result)
            {
                Log.Information($"Blocked URL: {url}");
            }
            return result;
        }
    }
}
