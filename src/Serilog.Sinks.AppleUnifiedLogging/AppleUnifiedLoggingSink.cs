using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

#nullable enable

namespace Serilog.Sinks.AppleUnifiedLogging
{
    public class AppleUnifiedLoggingSink : Serilog.Core.ILogEventSink
    {
        internal static class NativeMethods
        {
            [DllImport("libSystem.dylib", EntryPoint = "os_log_create")]
            public static extern IntPtr os_log_create(string subsystem, string category);

            [DllImport("libVorfreude.NativeLogging", EntryPoint = "LogDebug")]
            public static extern void LogDebug(IntPtr log, string message);

            [DllImport("libVorfreude.NativeLogging", EntryPoint = "LogInfo")]
            public static extern void LogInfo(IntPtr log, string message);

            [DllImport("libVorfreude.NativeLogging", EntryPoint = "LogError")]
            public static extern void LogError(IntPtr log, string message);

            [DllImport("libVorfreude.NativeLogging", EntryPoint = "LogFault")]
            public static extern void LogFault(IntPtr log, string message);
        }

        private readonly ITextFormatter _Formatter;

        private readonly string _Subsystem;
        private const string _DefaultCategory = "";

        private ConcurrentDictionary<string, IntPtr> _CreatedOSLogs =
            new ConcurrentDictionary<string, IntPtr>();

        public AppleUnifiedLoggingSink(ITextFormatter? formatter)
        {
            _Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));

            _Subsystem = Foundation.NSBundle.MainBundle.BundleIdentifier;
        }

        public void Emit(LogEvent? logEvent)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            string category = _DefaultCategory;

            if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContext))
                category = sourceContext.ToString();

            var log = _CreatedOSLogs.GetOrAdd(category, s =>
            {
                var newLog = NativeMethods.os_log_create(_Subsystem, category);

                if (newLog == IntPtr.Zero)
                    throw new Exception($"Failed to create log for subsystem {_Subsystem}, category {category}");

                return newLog;
            });

            string message;

            using (var sw = new StringWriter())
            {
                _Formatter.Format(logEvent, sw);

                message = sw.ToString();
            }

            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                    NativeMethods.LogDebug(log, message);
                    break;
                case LogEventLevel.Debug:
                    NativeMethods.LogDebug(log, message);
                    break;
                case LogEventLevel.Information:
                    NativeMethods.LogInfo(log, message);
                    break;
                case LogEventLevel.Warning:
                    NativeMethods.LogError(log, message);
                    break;
                case LogEventLevel.Error:
                    NativeMethods.LogError(log, message);
                    break;
                case LogEventLevel.Fatal:
                    NativeMethods.LogFault(log, message);
                    break;
            }
        }
    }

    public static class AppleUnifiedLoggingSinkExtensions
    {
        const string DefaultTemplate = "{Message:l}{NewLine:l}{Exception:l}";

        public static LoggerConfiguration AppleUnifiedLogging(
            this LoggerSinkConfiguration? sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null)
        {
            if (sinkConfiguration == null)
                throw new ArgumentNullException(nameof(sinkConfiguration));

            if (outputTemplate == null)
                throw new ArgumentNullException(nameof(outputTemplate));

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new AppleUnifiedLoggingSink(formatter), restrictedToMinimumLevel);
        }
    }
}
