using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserIdentity.Infrastructure.Security;
using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security
{
    public class KeySetFactoryTest : IClassFixture<TestSettingsFixture>
    {
        private readonly TestSettingsFixture _testSettings;

        public KeySetFactoryTest(TestSettingsFixture testSettings)
        {
            _testSettings = testSettings;
        }

        [Fact]
        public void Get_Algorithm_Returns_Algorithm()
        {
            KeySetFactory keySetFactory = new(_testSettings.Configuration);

            Assert.Equal("HS256", keySetFactory.GetAlgorithm());
        }

        [Fact]
        public void Get_KeyType_Returns_KeyType()
        {
            KeySetFactory keySetFactory = new(_testSettings.Configuration);

            Assert.Equal("oct", keySetFactory.GetKeyType());
        }

        [Fact]
        public void Get_Env_KeyId_Returns_Encoded_KeyId()
        {
            KeySetFactory keySetFactory = new(_testSettings.Configuration);

            String encodedKeyId = Base64UrlEncoder.Encode(_testSettings.Props["APP_KEY_ID"]);

            Assert.Equal(encodedKeyId, keySetFactory.GetKeyId());
        }

        [Fact]
        public void Get_Non_Env_KeyId_Returns_Encoded_KeyId()
        {
           
            String key = "APP_KEY_ID";          
            var appKeyId = Environment.GetEnvironmentVariable(key);

            // override env variables
            Environment.SetEnvironmentVariable(key, null);
            _testSettings.SetConfiguration();

            KeySetFactory keySetFactory = new(_testSettings.Configuration);
            String expectedKeyId = Base64UrlEncoder.Encode(key);
            String actualKeyId = keySetFactory.GetKeyId();

            // ensure env variables are same as before
            Environment.SetEnvironmentVariable(key, appKeyId);
            _testSettings.SetConfiguration();

            Assert.Equal(expectedKeyId, actualKeyId);
        }

        [Fact]
        public void Get_Default_KeyId_Returns_KeyId()
        {
            String key = "APP_KEY_ID";
            String defaultKeyId = "APPV1KEYID";
            var appKeyId = Environment.GetEnvironmentVariable(key);

            // override env variables
            Environment.SetEnvironmentVariable(key, null);
            _testSettings.SetConfiguration();
            _testSettings.Configuration.GetSection("KeySetOptions")["KeyId"] = null;

            KeySetFactory keySetFactory = new(_testSettings.Configuration);

            String expectedKeyId = Base64UrlEncoder.Encode(defaultKeyId);
            String actualKeyId = keySetFactory.GetKeyId();

            // ensure env variables are same as before
            Environment.SetEnvironmentVariable(key, appKeyId);
            _testSettings.SetConfiguration();

            Assert.Equal(expectedKeyId, actualKeyId);
        }

        [Fact]
        public void Get_Env_SecretKey_Returns_SecretKey()
        {
            KeySetFactory keySetFactory = new KeySetFactory(_testSettings.Configuration);
            String expectedSecretKey = _testSettings.Props["APP_SECRET_KEY"];

            Assert.Equal(expectedSecretKey, keySetFactory.GetSecretKey());
        }

        [Fact]
        public void Get_Non_Env_SecretKey_Returns_SecretKey()
        {

            String key = "APP_SECRET_KEY";
            var secretKey = Environment.GetEnvironmentVariable(key);

            // override env variables
            Environment.SetEnvironmentVariable(key, null);
            _testSettings.SetConfiguration();

            KeySetFactory keySetFactory = new KeySetFactory(_testSettings.Configuration);
            String expectedScretKey = _testSettings.Configuration.GetSection("KeySetOptions")["SecretKey"];
            String actualSecretKey = keySetFactory.GetSecretKey();

            // ensure env variables are same as before
            Environment.SetEnvironmentVariable(key, secretKey);
            _testSettings.SetConfiguration();

            Assert.Equal(expectedScretKey, actualSecretKey);
        }

        [Fact]
        public void Get_Default_SecretKey_Returns_SecretKey()
        {
            String key = "APP_SECRET_KEY";
            String defaultSecretKey = "KEY198*£%&YEK+OP}L";
            var envSecretKey = Environment.GetEnvironmentVariable(key);

            // override env variables
            Environment.SetEnvironmentVariable(key, null);
            _testSettings.SetConfiguration();
            _testSettings.Configuration.GetSection("KeySetOptions")["SecretKey"] = null;

            KeySetFactory keySetFactory = new KeySetFactory(_testSettings.Configuration);
            String actualSecretKey = keySetFactory.GetSecretKey();

            // ensure env variables are same as before
            Environment.SetEnvironmentVariable(key, envSecretKey);
            _testSettings.SetConfiguration();

            Assert.Equal(defaultSecretKey, actualSecretKey);
        }

        [Fact]
        public void Get_SigningKey_Returns_SigningKey()
        {
            String secretKey = _testSettings.Props["APP_SECRET_KEY"];
            SymmetricSecurityKey expectedKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            KeySetFactory keySetFactory = new KeySetFactory(_testSettings.Configuration);

            Assert.Equal(expectedKey.ToString(), keySetFactory.GetSigningKey().ToString());
        }

        [Fact]
        public void Get_Encoded_SecretKey_Returns_Encoded_SecretKey()
        {
            String expectedSecretKey = Base64UrlEncoder.Encode(_testSettings.Props["APP_SECRET_KEY"]);
            KeySetFactory keySetFactory = new KeySetFactory(_testSettings.Configuration);
            Assert.Equal(expectedSecretKey, keySetFactory.GetBase64URLEncodedSecretKey());
        }
    }

}