using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Configuration;

namespace UserIdentity.Infrastructure.Security
{

	public class KeySetFactory : IKeySetFactory
	{
		private readonly IConfigurationSection _keySetConfigurationSection;
		private readonly IConfiguration _configuration;
		private readonly IKeyProvider _keyProvider;

		private class PasswordFinder(string password) : IPasswordFinder
		{
			private readonly string password = password;

			public char[] GetPassword() => password.ToCharArray();
		}

		public KeySetFactory(IConfiguration configuration, IKeyProvider keyProvider)
		{
			_keySetConfigurationSection = configuration.GetSection(nameof(KeySetOptions));
			_configuration = configuration;
			_keyProvider = keyProvider;
		}

		public string GetAlgorithm()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.Alg)] ?? SecurityAlgorithms.RsaSha256;
		}

		public string GetKeyType()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.KeyType)] ?? "RSA";
		}

		public string GetKeyId()
		{
			var envKeyId = _configuration.GetEnvironmentVariable("APP_KEY_ID");
			return Base64UrlEncoder.Encode(envKeyId);
		}

		public async Task<AsymmetricSecurityKey> GetSigningKeyAsync()
		{
			return new RsaSecurityKey(await ReadPemFileAsync("APP_PRIVATE_KEY_PATH", "APP_PRIVATE_KEY_PASS_PHRASE"));
		}

		public async Task<AsymmetricSecurityKey> GetVerificationKeyAsync()
		{
			return new RsaSecurityKey(await ReadPemFileAsync("APP_PUBLIC_KEY_PATH"));
		}

		public async Task<(string, string)> GetModulusAndExponentForPublicKeyAsync()
		{
			var publicKeyRsaParameters = await ReadPemFileAsync("APP_PUBLIC_KEY_PATH");

			var modulus = Base64UrlEncoder.Encode(publicKeyRsaParameters.Modulus);
			var exponent = Base64UrlEncoder.Encode(publicKeyRsaParameters.Exponent);

			return (modulus, exponent);
		}


		private async Task<RSAParameters> ReadPemFileAsync(string key, string? passPhraseKey = null)
		{
			var keyPath = _configuration.GetEnvironmentVariable(key);
			var keyContent = await _keyProvider.GetKeyAsync(keyPath);

			using var keyTextReader = new StringReader(keyContent);

			if (passPhraseKey == null)
			{
				var publicKeyParameters = (RsaKeyParameters)new PemReader(keyTextReader).ReadObject();
				return DotNetUtilities.ToRSAParameters(publicKeyParameters);
			}
			else
			{
				var passPhrase = _configuration.GetEnvironmentVariable(passPhraseKey);
				var privateKeyParameters = (RsaPrivateCrtKeyParameters)new PemReader(keyTextReader, new PasswordFinder(passPhrase)).ReadObject();
				return DotNetUtilities.ToRSAParameters(privateKeyParameters);
			}
		}
	}
}
