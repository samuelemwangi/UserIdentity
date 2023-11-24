using System;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using UserIdentity.Infrastructure.Security;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security
{
	public class KeySetFactoryTests : IClassFixture<TestSettingsFixture>
	{
		private readonly TestSettingsFixture _testSettings;

		public KeySetFactoryTests(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;
		}

		[Fact]
		public void Get_Algorithm_Returns_Algorithm()
		{
			// Arrange
			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act & Assert
			Assert.Equal("HS256", keySetFactory.GetAlgorithm());
		}

		[Fact]
		public void Get_KeyType_Returns_KeyType()
		{
			// Arrange
			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act & Assert
			Assert.Equal("oct", keySetFactory.GetKeyType());
		}

		[Fact]
		public void Get_Env_KeyId_Returns_Encoded_KeyId()
		{
			// Arrange
			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act 
			var encodedKeyId = Base64UrlEncoder.Encode(_testSettings.Props["APP_KEY_ID"]);

			// Asert
			Assert.Equal(encodedKeyId, keySetFactory.GetKeyId());
		}

		[Fact]
		public void Get_Non_Env_KeyId_Returns_Encoded_KeyId()
		{
			// Arrange
			var key = "APP_KEY_ID";
			var appKeyId = Environment.GetEnvironmentVariable(key);

			// override env variables
			Environment.SetEnvironmentVariable(key, null);
			_testSettings.SetConfiguration();

			var keySetFactory = new KeySetFactory(_testSettings.Configuration);
			var expectedKeyId = Base64UrlEncoder.Encode(key);

			// Act
			var actualKeyId = keySetFactory.GetKeyId();

			// ensure env variables are same as before
			Environment.SetEnvironmentVariable(key, appKeyId);
			_testSettings.SetConfiguration();

			// Assert
			Assert.NotNull(actualKeyId);
		}

		[Fact]
		public void Get_Default_KeyId_Returns_KeyId()
		{
			// Arrange
			var key = "APP_KEY_ID";
			var defaultKeyId = "APPV1KEYID";
			var appKeyId = Environment.GetEnvironmentVariable(key);

			// override env variables
			Environment.SetEnvironmentVariable(key, null);
			_testSettings.SetConfiguration();
			_testSettings.Configuration.GetSection("KeySetOptions")["KeyId"] = null;

			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			var expectedKeyId = Base64UrlEncoder.Encode(defaultKeyId);

			// Act
			var actualKeyId = keySetFactory.GetKeyId();

			// ensure env variables are same as before
			Environment.SetEnvironmentVariable(key, appKeyId);
			_testSettings.SetConfiguration();

			// Assert
			Assert.NotNull(actualKeyId);
		}

		[Fact]
		public void Get_Env_SecretKey_Returns_SecretKey()
		{
			// Arrange
			var keySetFactory = new KeySetFactory(_testSettings.Configuration);
			var expectedSecretKey = _testSettings.Props["APP_SECRET_KEY"];

			// Act & Assert
			Assert.Equal(expectedSecretKey, keySetFactory.GetSecretKey());
		}

		[Fact]
		public void Get_Non_Env_SecretKey_Returns_SecretKey()
		{
			// Arrange
			var key = "APP_SECRET_KEY";
			var secretKey = Environment.GetEnvironmentVariable(key);

			// override env variables
			Environment.SetEnvironmentVariable(key, null);
			_testSettings.SetConfiguration();

			var keySetFactory = new KeySetFactory(_testSettings.Configuration);
			var expectedScretKey = _testSettings.Configuration.GetSection("KeySetOptions")["SecretKey"];

			// Act
			var actualSecretKey = keySetFactory.GetSecretKey();

			// ensure env variables are same as before
			Environment.SetEnvironmentVariable(key, secretKey);
			_testSettings.SetConfiguration();

			// Assert
			Assert.Equal(expectedScretKey, actualSecretKey);
		}

		[Fact]
		public void Get_Env_SecretKey_Less_Than_32_Chars_Throws_SecurityTokenInvalidSigningKeyException()
		{
			// Arrange
			var key = "APP_SECRET_KEY";
			var secretKey = Environment.GetEnvironmentVariable(key);

			// override env variables
			Environment.SetEnvironmentVariable(key, "123");
			_testSettings.SetConfiguration();

			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act & Assert
			var exception = Assert.Throws<SecurityTokenInvalidSigningKeyException>(keySetFactory.GetSecretKey);
			Assert.Equal("Invalid key provided. Security key should be at least 32 characters", exception.Message);

			// ensure env variables are same as before
			Environment.SetEnvironmentVariable(key, secretKey);
			_testSettings.SetConfiguration();

		}

		[Fact]
		public void Get_Default_SecretKey_Returns_SecretKey()
		{
			// Arrange
			var key = "APP_SECRET_KEY";
			var defaultSecretKey = "KEY198*£%&YEK+OP}L5H0ULD>32CH8Rz";
			var envSecretKey = Environment.GetEnvironmentVariable(key);

			// override env variables
			Environment.SetEnvironmentVariable(key, null);
			_testSettings.SetConfiguration();
			_testSettings.Configuration.GetSection("KeySetOptions")["SecretKey"] = null;

			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act
			var actualSecretKey = keySetFactory.GetSecretKey();

			// ensure env variables are same as before
			Environment.SetEnvironmentVariable(key, envSecretKey);
			_testSettings.SetConfiguration();

			// Assert
			Assert.Equal(defaultSecretKey, actualSecretKey);
		}

		[Fact]
		public void Get_SigningKey_Returns_SigningKey()
		{
			// Arrange
			var secretKey = _testSettings.Props["APP_SECRET_KEY"];
			var expectedKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act & Assert
			Assert.Equal(expectedKey.ToString(), keySetFactory.GetSigningKey().ToString());
		}

		[Fact]
		public void Get_Encoded_SecretKey_Returns_Encoded_SecretKey()
		{
			// Arrange
			var expectedSecretKey = Base64UrlEncoder.Encode(_testSettings.Props["APP_SECRET_KEY"]);
			var keySetFactory = new KeySetFactory(_testSettings.Configuration);

			// Act & Assert
			Assert.Equal(expectedSecretKey, keySetFactory.GetBase64URLEncodedSecretKey());
		}
	}

}