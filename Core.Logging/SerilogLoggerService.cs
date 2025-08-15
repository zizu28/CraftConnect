using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Core.Logging
{
	public class SerilogLoggerService<T>(ILogger<T> logger) : ILoggingService<T> where T : class
	{
		private readonly ILogger<T> _logger = logger;

		public void LogCritical(Exception? exception, string message, params object[] args)
		{
			_logger.LogCritical(exception, message, args);
		}

		public void LogDebug(string message, params object[] args)
		{
			_logger.LogDebug(message, args);
		}

		public void LogError(Exception? exception, string message, params object[] args)
		{
			_logger.LogError(exception, message, args);
		}

		public void LogInformation(string message, params object[] args)
		{
			_logger.LogInformation(message, args);
		}

		public void LogTrace(string message, params object[] args)
		{
			_logger.LogTrace(message, args);
		}

		public void LogWarning(string message, params object[] args)
		{
			_logger.LogWarning(message, args);
		}

		public void LogWithContext(LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? context = null)
		{
			using (LogContext.PushProperty("Context", context))
			{
				switch (level)
				{
					case LogLevel.Critical:
						LogCritical(exception, message);
						break;
					case LogLevel.Debug:
						LogDebug(message);
						break;
					case LogLevel.Error:
						LogError(exception, message);
						break;
					case LogLevel.Information:
						LogInformation(message);
						break;
					case LogLevel.Trace:
						LogTrace(message);
						break;
					case LogLevel.Warning:
						LogWarning(message);
						break;
					default:
						LogInformation(message);
						break;
				}
			}
		}
	}
}
