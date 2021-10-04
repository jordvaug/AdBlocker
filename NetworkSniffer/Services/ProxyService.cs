using Microsoft.Extensions.Options;
using NetworkSniffer.Configuration;
using NetworkSniffer.Interfaces;
using Serilog;
using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace NetworkSniffer.Services
{
    public class ProxyService : IProxyService
    {
        private readonly Settings _settings;
        private readonly IRequestsService _requestsService;
        private readonly IAlarmService _alarmService;
        private readonly IFilterService _filterService;
        private int _port;
        public ProxyService(IOptions<Settings> settings, IRequestsService requestsService, IAlarmService alarmService,
                                IFilterService filterService)
        {
            _settings = settings.Value;
            _port = _settings.Port;
            _requestsService = requestsService;
            _alarmService = alarmService;
            _filterService = filterService;
        }

        public ProxyServer GetProxyServer()
        {
            var proxyServer = new ProxyServer(userTrustRootCertificate: true);

            try
            {
                var httpProxy = new ExplicitProxyEndPoint(IPAddress.Any, _port, decryptSsl: true);

                proxyServer.AddEndPoint(httpProxy);
                proxyServer.Start();

                proxyServer.BeforeRequest += OnBeforeRequest;
                proxyServer.BeforeResponse += OnBeforeResponse;
            }
            catch (System.NullReferenceException e)
            {
                Log.Error(e.Message);
                _port = 8080;
            }
            catch (ConfigurationException ex)
            {
                Log.Error(ex.Message);
                _port = 8080;
            }
            Console.WriteLine($"Please turn on proxy on your machine and set it to: http://localhost:{_port}");
            Log.Information($"Starting Proxy server on port {_port}");

            return proxyServer;
        }

        /// <summary>
        /// Validates the url before the request is proxied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        /// <returns></returns>
        public async Task OnBeforeRequest(object sender, SessionEventArgs ev)
        {
            var request = ev.HttpClient.Request;
            var token = new CancellationTokenSource();
            try
            {
                if (!ValidateUrl(request.Url) || _filterService.ValidateRequest(request.Url))
                {
                    token.Cancel();
                    CancellationToken ct = token.Token;
                    ct.ThrowIfCancellationRequested();
                }
                await Task.CompletedTask;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"Invalid URL was queried: {request.Url}");
                Log.Error($"Invalid URL was queried: {request.Url}");
            }
            finally
            {
                token.Dispose();
            }

        }

        /// <summary>
        /// Records summary of request before returning the response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        /// <returns></returns>
        public async Task OnBeforeResponse(object sender, SessionEventArgs ev)
        {
            var request = ev.HttpClient.Request;
            _requestsService.AddRequest(request.Url, request.Method);
            _alarmService.AddRequest();

            Log.Information($"Proxying for: {request.Url}");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Validates that the Url is valid.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool ValidateUrl(string url)
        {
            try
            {
                return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult);
            }
            catch(ArgumentNullException ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}
