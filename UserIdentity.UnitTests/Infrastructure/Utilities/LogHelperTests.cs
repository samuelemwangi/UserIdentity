using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using UserIdentity.Infrastructure.Utilities;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
	public class LogHelperTests
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<LogHelperTests> _logger;

		public LogHelperTests()
		{
			_loggerFactory = new NullLoggerFactory();
			_logger = _loggerFactory.CreateLogger<LogHelperTests>();
		}

		[Fact]
		public void LogEvent_Logs_Event()
		{
			LogHelper<LogHelperTests> logHelper = new(_loggerFactory);
			String logMessage = "Log Message";

			logHelper.LogEvent(logMessage, LogLevel.Information);


		}

	}
}

