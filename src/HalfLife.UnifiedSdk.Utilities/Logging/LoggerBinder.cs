using Serilog;
using Serilog.Events;
using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Reflection.Metadata.Ecma335;

namespace HalfLife.UnifiedSdk.Utilities.Logging
{
    /// <summary>
    /// Provides a <see cref="ILogger"/> in a <see cref="System.CommandLine.Command"/> handler.
    /// The logger is configured to log to the console and to the Visual Studio debug output window (when running with Visual Studio).
    /// </summary>
    public sealed class LoggerBinder : BinderBase<ILogger>
    {
        /// <summary>Singleton binder instance.</summary>
        public static LoggerBinder Instance { get; } = new();

        private readonly Func<BindingContext, LogEventLevel> _getLogEventLevel;

        /// <summary>Creates a binder that uses the <see cref="LogEventLevel.Information"/> log level.</summary>
        public LoggerBinder()
        {
            _getLogEventLevel = _ => LogEventLevel.Information;
        }

        /// <summary>Creates a binder that uses the given option to indicate verbose logging.</summary>
        public LoggerBinder(Option<bool> verboseOption)
        {
            _getLogEventLevel = bindingContext =>
                bindingContext.ParseResult.GetValueForOption(verboseOption) ? LogEventLevel.Verbose : LogEventLevel.Information;
        }

        /// <summary>Creates a binder that uses the given delegate to get the log event level.</summary>
        public LoggerBinder(Func<BindingContext, LogEventLevel> getLogEventLevel)
        {
            _getLogEventLevel = getLogEventLevel;
        }

        /// <inheritdoc/>
        protected override ILogger GetBoundValue(BindingContext bindingContext)
        {
            return CreateLogger(_getLogEventLevel(bindingContext));
        }

        /// <summary>Creates a logger with the given minimum level.</summary>
        public static ILogger CreateLogger(LogEventLevel minimumLevel)
        {
            return new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Debug()
                .MinimumLevel.Is(minimumLevel)
                .CreateLogger();
        }
    }
}
