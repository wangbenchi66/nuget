using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Core
{
    public class LoggerWrapper
    {
        private static ILogger _staticLogger;

        /// <summary>
        /// 初始化静态Logger
        /// </summary>
        /// <param name="logger">ILogger实例</param>
        public static void InitializeStaticLogger(ILogger<LoggerWrapper> logger)
        {
            _staticLogger = logger;
        }

        /// <summary>
        /// 记录信息日志（静态方法）
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="args">日志参数</param>
        public static void LogInformation(string message, params object[] args)
        {
            _staticLogger?.LogInformation(message, args);
        }

        /// <summary>
        /// 记录调试日志（静态方法）
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="args">日志参数</param>
        public static void LogDebug(string message, params object[] args)
        {
            _staticLogger?.LogDebug(message, args);
        }

        /// <summary>
        /// 记录警告日志（静态方法）
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="args">日志参数</param>
        public static void LogWarning(string message, params object[] args)
        {
            _staticLogger?.LogWarning(message, args);
        }

        /// <summary>
        /// 记录错误日志（静态方法）
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="args">日志参数</param>
        public static void LogError(string message, params object[] args)
        {
            _staticLogger?.LogError(message, args);
        }

        /// <summary>
        /// 记录严重错误日志（静态方法）
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="args">日志参数</param>
        public static void LogCritical(string message, params object[] args)
        {
            _staticLogger?.LogCritical(message, args);
        }
    }
}