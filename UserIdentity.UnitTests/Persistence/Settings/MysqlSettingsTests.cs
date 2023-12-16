using System;

using Microsoft.Extensions.Configuration;

using UserIdentity.Persistence.Settings.Mysql;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Settings
{
	public class MysqlSettingsTests : IClassFixture<TestSettingsFixture>
	{
		private readonly TestSettingsFixture _testSettings;

		public MysqlSettingsTests(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;
		}

		[Fact]
		public void ConnectionString_Should_Return_Well_Formed_ConnectionString()
		{
			// Arrange
			var props = _testSettings.Props;
			var mysqlSettings = _testSettings.Configuration.GetSection(nameof(MysqlSettings)).Get<MysqlSettings>();
			string expectedString = $"Server={props["DB_SERVER"]};Port={props["DB_PORT"]};Database={props["DB_NAME"]};User={props["DB_USER"]};Password={props["DB_PASSWORD"]};";

			// Act
			var connectionString = mysqlSettings.ConnectionString(_testSettings.Configuration);

			// Assert
			Assert.Equal(expectedString, connectionString);
		}
	}
}

