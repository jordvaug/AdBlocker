using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetworkSniffer.Configuration;
using NetworkSniffer.Interfaces;
using NetworkSniffer.Services;
using Serilog;
using System.IO;

namespace NetworkSniffer
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            IConfiguration configuration = builder.Build();

            var alarmSettingsSection =
                    configuration.GetSection("AppAlarmSettings");

            var AdListSection =
                    configuration.GetSection("AdList");

            services.Configure<Settings>(alarmSettingsSection);

            services.Configure<AdSites>(AdListSection);

            services.AddSingleton(configuration);
            services.AddTransient<EntryPoint>();
            services.AddTransient<IAlarmService, AlarmService>();
            services.AddTransient<IProxyService, ProxyService>();
            services.AddTransient<IRequestsService, RequestsService>();
            services.AddTransient<IScheduleService, ScheduleService>();
            services.AddTransient<IFilterService, FilterService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true);
            });

            Log.Information($"Starting Network Sniffer");

            return services;
        }

    }
}
