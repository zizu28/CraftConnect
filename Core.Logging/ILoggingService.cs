using Microsoft.Extensions.Logging;

namespace Core.Logging
{
	public interface ILoggingService<T> where T : class
	{
		void LogTrace(string message, params object[] args);
		void LogDebug(string message, params object[] args);
		void LogInformation(string message, params object[] args);
		void LogWarning(string message, params object[] args);
		void LogError(Exception? exception, string message, params object[] args);
		void LogCritical(Exception? exception, string message, params object[] args);
		void LogWithContext(LogLevel level, string message, 
			Exception? exception = null, Dictionary<string, object>? context = null);
	}
}
