using System;
using System.Threading.Tasks;

using MELT;

using Microsoft.Extensions.Logging;

using UserIdentity.Infrastructure.Utilities;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Utilities
{
	public class LogHelperTests
	{
		private readonly ITestLoggerFactory _loggerFactory;

		public LogHelperTests()
		{
			_loggerFactory = TestLoggerFactory.Create();
		}

		[Theory]
		[InlineData("Test Log Message Info", LogLevel.Information)]
		[InlineData("Test Log Message Warning", LogLevel.Warning)]
		[InlineData("Test Log Message Error", LogLevel.Error)]
		[InlineData("Test Log Message Critical", LogLevel.Critical)]
		[InlineData("Test Log Message Debug", LogLevel.Debug)]
		[InlineData("Test Log Message Trace", LogLevel.Trace)]
		public async Task Log_Helper_Log_Event_Logs_Event(string logMessage, LogLevel logLevel)
		{
			// Arrange
			LogHelper<object> logHelper = new(_loggerFactory);

			// Act
			await logHelper.LogEventAsync(logMessage, logLevel);

			// Assert
			var log = Assert.Single(_loggerFactory.Sink.LogEntries);
			Assert.Equal(logMessage, log.Message);
			Assert.Equal(logLevel, log.LogLevel);
		}

	}
}

