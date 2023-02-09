using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UserIdentity.Infrastructure.Utilities;
using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
	public class LogHelperTest
	{
		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger<LogHelperTest> _logger;

        public LogHelperTest()
		{
			_loggerFactory = new NullLoggerFactory();
			_logger = _loggerFactory.CreateLogger<LogHelperTest>();
		}

		[Fact]
		public void LogEvent_Logs_Event()
		{
			LogHelper<LogHelperTest> logHelper = new(_loggerFactory);
			String logMessage = "Log Message";

			logHelper.LogEvent(logMessage, LogLevel.Information);
			

        }
	
	}
}

