using System.Collections.Generic;

namespace NetworkSniffer.Interfaces
{
    public interface IRequestsService
    {
        public void AddRequest(string url, string method);
        public void ClearRequests();
        public void PrintRequests();
        public (string, int) FindTopSection();
        public int NumberOfRequests();
        public int NumberOfPages();
        public List<int> TypeOfRequests();
    }
}
