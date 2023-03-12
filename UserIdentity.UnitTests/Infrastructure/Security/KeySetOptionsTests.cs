using UserIdentity.Infrastructure.Security;
using UserIdentity.UnitTests.Utils;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security
{
	public class KeySetOptionsTests : IClassFixture<TestSettingsFixture>
	{
		private readonly TestSettingsFixture _testSettings;

		public KeySetOptionsTests(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;
		}

		[Fact]
		public void Configure_KeySetOptions_Configures_KeySetOptions()
		{
			// Arrange
			var alg = "HS256";
			var keyType = "oct";
			var keyId = "APP_KEY_ID";
			var secretKey = "$E+ $E(RET K#Y IN 3NV1R0MEN+";

			// Act & Assert
			var section = _testSettings.Configuration.GetSection(nameof(KeySetOptions));

			Assert.Equal(alg, section[nameof(KeySetOptions.Alg)]);
			Assert.Equal(keyType, section[nameof(KeySetOptions.KeyType)]);
			Assert.Equal(keyId, section[nameof(KeySetOptions.KeyId)]);
			Assert.Equal(secretKey, section[nameof(KeySetOptions.SecretKey)]);
		}

		[Fact]
		public void New_Configure_KeySetOptions_Creates_New_KeySetOptions_Instance()
		{
			// Arrange
			var keySetOptions = new KeySetOptions();

			// Act & Assert
			Assert.NotNull(keySetOptions);
		}
	}
}
