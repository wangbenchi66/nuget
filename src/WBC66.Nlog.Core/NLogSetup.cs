using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using NLog;

namespace WBC66.NLog.Core
{
    /// <summary>
    /// NLog ���÷�װ
    /// </summary>
    public static class NLogSetup
    {
        /// <summary>
        /// ʹ��NLog����(Ĭ�������ļ�nlog.config)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configPath"></param>
        public static void AddNLogSteup(this WebApplicationBuilder builder, string configPath = "nlog.config")
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Host.UseNLog();
        }

        /// <summary>
        /// ʹ��NLog����(Ĭ�������ļ�jsonģʽ)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationManager"></param>
        public static void AddNLogSteup(this WebApplicationBuilder builder, ConfigurationManager configurationManager)
        {
            var nlogConfig = builder.Configuration.GetSection("NLog");
            if (nlogConfig == null)
                return;
            LogManager.Configuration = new NLogLoggingConfiguration(nlogConfig);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            var options = new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false };
            builder.Host.UseNLog(options);
        }

    }
}