using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;

namespace NetworkSniffer.Interfaces
{
    public interface IProxyService
    {
        public ProxyServer GetProxyServer();
        public Task OnBeforeRequest(object sender, SessionEventArgs ev);
        public Task OnBeforeResponse(object sender, SessionEventArgs ev);
        public bool ValidateUrl(string url);
    }
}
