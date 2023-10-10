using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Common.Extensions
{
    public static class SerilogExtensions
    {
        const string Tempale = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// 添加Serilog日志
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultSerilog(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                // .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt",
                    outputTemplate: Tempale,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: false);
            });
            
            return services;
        }
    }
}