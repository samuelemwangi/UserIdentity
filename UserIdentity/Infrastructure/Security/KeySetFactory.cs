using Microsoft.IdentityModel.Tokens;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Configuration;
using UserIdentity.Infrastructure.Security.Providers;

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

		public string GetAlgorithm() => _keySetConfigurationSection[nameof(KeySetOptions.Alg)] ?? EdDsaSecurityAlgorithmConstants.EdDsa;

		public string GetKeyType() => _keySetConfigurationSection[nameof(KeySetOptions.KeyType)] ?? EdDsaSecurityAlgorithmConstants.OKP;

		public string GetKeyId() => Base64UrlEncoder.Encode(_configuration.GetEnvironmentVariable("APP_KEY_ID"));

		public async Task<AsymmetricSecurityKey> GetSigningKeyAsync()
		{
			var privateKeyParamters = await ReadPemFileAsync("APP_PRIVATE_KEY_PATH", "APP_PRIVATE_KEY_PASS_PHRASE");
			return new EdDsaSecurityKey(privateKeyParamters)
			{
				KeyId = GetKeyId()
			};
		}

		public async Task<AsymmetricSecurityKey> GetVerificationKeyAsync()
		{
			var publicKeyParameters = await ReadPemFileAsync("APP_PUBLIC_KEY_PATH");

			return new EdDsaSecurityKey(publicKeyParameters)
			{
				KeyId = GetKeyId()
			};
		}

		public string GetCrvValue() => EdDsaSecurityAlgorithmConstants.Ed25519;

		public async Task<string> GetXValueAysnc()
		{
			var publicKey = (EdDsaSecurityKey)(await GetVerificationKeyAsync());

			return Base64UrlEncoder.Encode(publicKey.PublicKey.GetEncoded());
		}

		private async Task<Ed25519PublicKeyParameters> ReadPemFileAsync(string key)
		{
			var keyContent = await GetKeyContentAsync(key);

			using var keyTextReader = new StringReader(keyContent);

			return (Ed25519PublicKeyParameters)new PemReader(keyTextReader).ReadObject();
		}
		private async Task<Ed25519PrivateKeyParameters> ReadPemFileAsync(string key, string passPhraseKey)
		{
			var keyContent = await GetKeyContentAsync(key);

			using var keyTextReader = new StringReader(keyContent);

			var passPhrase = _configuration.GetEnvironmentVariable(passPhraseKey);

			return (Ed25519PrivateKeyParameters)new PemReader(keyTextReader, new PasswordFinder(passPhrase)).ReadObject();
		}

		private async Task<string> GetKeyContentAsync(string key) => await _keyProvider.GetKeyAsync(_configuration.GetEnvironmentVariable(key));
	}
}
