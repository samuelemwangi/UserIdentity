using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Utilities
{
	public class LogHelper<T> : ILogHelper<T>
	{
		private readonly ILoggerFactory _loggerFactory;

		public LogHelper(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;

		}
		public void LogEvent(string message, LogLevel logLevel)
		{
			ILogger<T> logger = _loggerFactory.CreateLogger<T>();

			string resolvedMessage = message + "";

			switch (logLevel)
			{
				case LogLevel.Trace:
					logger.LogTrace(resolvedMessage);
					break;
				case LogLevel.Debug:
					logger.LogDebug(resolvedMessage);
					break;
				case LogLevel.Warning:
					logger.LogWarning(resolvedMessage);
					break;
				case LogLevel.Error:
					logger.LogError(resolvedMessage);
					break;
				case LogLevel.Critical:
					logger.LogCritical(resolvedMessage);
					break;
				default:
					logger.LogInformation(resolvedMessage);
					break;
			}

		}
	}
}
