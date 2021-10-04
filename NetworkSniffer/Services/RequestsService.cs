using Microsoft.Extensions.Caching.Memory;
using NetworkSniffer.Helpers;
using NetworkSniffer.Interfaces;
using System.Collections.Generic;
using _ = NetworkSniffer.Enums.Cache;

namespace NetworkSniffer.Services
{
    public class RequestsService : IRequestsService
    {
        private readonly IMemoryCache _memoryCache;

        public RequestsService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Adds a new url request to the NewRequest list and NewPageList.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method">Http method</param>
        public void AddRequest(string url, string method)
        {
            List<string> existingMethods;
            if (_memoryCache.TryGetValue(_.Methods, out existingMethods))
            {
                existingMethods.Add(method);
                _memoryCache.Set(_.Methods, existingMethods);
            }
            else
            {
                _memoryCache.Set(_.Methods, new List<string>() { method });

            }

            AddSection(url);
        }

        /// <summary>
        /// Adds a new section to the PageList Dictionary, or increases existing value if it is already present.
        /// </summary>
        /// <param name="url"></param>
        private void AddSection(string url)
        {
            var stringArray = url.Split('/');
            Dictionary<string, int> existingSections;

            if (stringArray.Length > 3)
            {
                if (_memoryCache.TryGetValue(_.Sections, out existingSections))
                {
                    if (!existingSections.TryAdd(stringArray[3], 1))
                    {
                        existingSections[stringArray[3]] += 1;
                    }
                }
                else
                {
                    existingSections = new Dictionary<string, int>();
                    existingSections.Add(stringArray[3], 1);
                }
                _memoryCache.Set(_.Sections, existingSections);
            }
        }

        /// <summary>
        /// Flushes Requests from New to Old Request Collections to maintain Collections from last x seconds and new collections simultaneously.
        /// </summary>
        public void ClearRequests()
        {
            _memoryCache.Remove(_.Sections);
            _memoryCache.Remove(_.Methods);
        }

        /// <summary>
        /// Creates summary or requests and prints it using the Console Utility.
        /// </summary>
        public void PrintRequests()
        {
            var topSection = FindTopSection();
            ConsoleUtilities.WriteHits(topSection.Item1, topSection.Item2, GetPrintSpacing());

            var numRequests = NumberOfRequests();
            var numHosts = NumberOfPages();
            var types = TypeOfRequests();
            ConsoleUtilities.WriteSummary(numRequests, numHosts, types);
        }

        /// <summary>
        /// Gets the required spacing between the top of the console and where requests will be written. Takes into account 
        /// the numbers of alarms and the header messages.
        /// </summary>
        private int GetPrintSpacing()
        {
            List<string> existingAlarms;
            if (_memoryCache.TryGetValue(_.Alarms, out existingAlarms))
            {
                return existingAlarms.Count + 3;
            }
            return 3; //Avoids writing over header
        }

        /// <summary>
        /// Finds the most visited section of any site and the number of times it was visited.
        /// </summary>
        /// <returns>Tuple of section and number of visits</returns>
        public (string, int) FindTopSection()
        {
            string url = "";
            int max = 0;
            Dictionary<string, int> existingSections;

            if (_memoryCache.TryGetValue(_.Sections, out existingSections))
            {
                foreach (var section in existingSections)
                {
                    if (section.Value > max)
                    {
                        max = section.Value;
                        url = section.Key;
                    }
                }
            }

            return (url, max);
        }

        /// <summary>
        /// Finds the total number of requests made.
        /// </summary>
        public int NumberOfRequests()
        {
            var totalRequests = 0;
            List<string> existingMethods;
            if (_memoryCache.TryGetValue(_.Methods, out existingMethods))
            {
                totalRequests = existingMethods.Count;
            }
            return totalRequests;
        }

        /// <summary>
        /// Finds the number of unique pages visited
        /// </summary>
        /// <returns></returns>
        public int NumberOfPages()
        {
            var totalSections = 0;
            Dictionary<string, int> existingSections;

            if (_memoryCache.TryGetValue(_.Sections, out existingSections))
            {
                totalSections = existingSections.Count;
            }
            return totalSections;
        }

        /// <summary>
        /// Calculates the number of get, post, put, and delete requests.
        /// </summary>
        /// <returns>A list of the above.</returns>
        public List<int> TypeOfRequests()
        {
            var types = new List<int> { 0, 0, 0, 0 };
            List<string> existingMethods;

            if (_memoryCache.TryGetValue(_.Methods, out existingMethods))
            {
                foreach (var method in existingMethods)
                {
                    switch (method)
                    {
                        case "GET":
                            types[0] += 1;
                            break;
                        case "POST":
                            types[1] += 1;
                            break;
                        case "PUT":
                            types[2] += 1;
                            break;
                        case "DELETE":
                            types[3] += 1;
                            break;
                    }
                }
            }

            return types;
        }

    }
}
