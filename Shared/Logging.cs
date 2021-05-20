using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Web;

namespace MfePoc.Shared
{
    public static class Logging
    {
        public static void SetupNLog<T>(Action run)
        {
            var loggerFullName = typeof(T).FullName;
            var moduleName = loggerFullName.Replace("MfePoc.", "").Replace(".Program", "");

            var config = new LoggingConfiguration();

            var rootFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ".."));

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == Environments.Development)
                while (!File.Exists(Path.Combine(rootFolder, "MfePoc.sln")))
                    rootFolder = Directory.GetParent(rootFolder).FullName;

            var logFolder = Path.Combine(rootFolder, "logs");

            InternalLogger.LogFile = Path.Combine(logFolder, $"{moduleName}.internal.log");
            LogManager.ThrowConfigExceptions = true;

            var fileTarget = new FileTarget("file")
            {
                FileName = Path.Combine(logFolder, "log-${shortdate}.log"),
                ConcurrentWrites = true,
                ConcurrentWriteAttempts = 10,
                ConcurrentWriteAttemptDelay = 43,
                ForceMutexConcurrentWrites = true,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 7,
                Layout = "${longdate}|${level:uppercase=true}|" + moduleName + "|${logger}|${message} ${exception:format=tostring}",
            };

            var wrapper = new BufferingTargetWrapper(fileTarget)
            {
                BufferSize = 1000,
                FlushTimeout = 100,
                OverflowAction = BufferingTargetWrapperOverflowAction.Flush,
            };

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, wrapper);
            LogManager.Configuration = config;

            var logger = LogManager.GetLogger(loggerFullName);
            logger.Info("Process start");

            try
            {
                run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Process exception");
            }
            finally
            {
                logger.Info("Process end");
            }

            LogManager.Shutdown();
        }

        public static IWebHostBuilder UseMfePocNLog(this IWebHostBuilder builder)
        {
            builder.UseNLog();
            return builder;
        }
    }
}
