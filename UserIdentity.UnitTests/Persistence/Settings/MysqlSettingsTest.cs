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
            var mysqlSettings = _testSettings.Configuration.GetSection(nameof(MysqlSettings)).Get<MysqlSettings>();

            String connectionString = mysqlSettings.ConnectionString(_testSettings.Configuration);          

            Assert.Contains(Environment.GetEnvironmentVariable("DB_SERVER"), connectionString);
        }
    }
}

