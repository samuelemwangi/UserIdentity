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

		public string GetAlgorithm()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.Alg)] ?? EdDsaSecurityAlgorithmConstants.EdDsa;
		}

		public string GetKeyType()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.KeyType)] ?? EdDsaSecurityAlgorithmConstants.EdDsa;
		}

		public string GetKeyId()
		{
			var envKeyId = _configuration.GetEnvironmentVariable("APP_KEY_ID");
			return Base64UrlEncoder.Encode(envKeyId);
		}

		public async Task<AsymmetricSecurityKey> GetSigningKeyAsync()
		{
			var privateKeyParamters = await ReadPemFileAsync("APP_PRIVATE_KEY_PATH", "APP_PRIVATE_KEY_PASS_PHRASE");
			return new EdDsaSecurityKey(privateKeyParamters)
			{
				CryptoProviderFactory = new EdDsaCryptoProviderFactory()
			};
		}

		public async Task<AsymmetricSecurityKey> GetVerificationKeyAsync()
		{
			var publicKeyParameters = await ReadPemFileAsync("APP_PUBLIC_KEY_PATH");
			return new EdDsaSecurityKey(publicKeyParameters)
			{
				CryptoProviderFactory = new EdDsaCryptoProviderFactory()
			};
		}

		public string GetCrvValue()
		{
			return "Ed25519";
		}

		public async Task<string> GetXValueAysnc()
		{
			var publicKeyParameters = await ReadPemFileAsync("APP_PUBLIC_KEY_PATH");

			return Base64UrlEncoder.Encode(publicKeyParameters.GetEncoded());
		}

		private async Task<Ed25519PublicKeyParameters> ReadPemFileAsync(string key)
		{
			var keyContent = await GetKeyContentAsync(key);

			using var keyTextReader = new StringReader(keyContent);

			var publicKeyParameters = (Ed25519PublicKeyParameters)new PemReader(keyTextReader).ReadObject();

			return publicKeyParameters;
		}
		private async Task<Ed25519PrivateKeyParameters> ReadPemFileAsync(string key, string passPhraseKey)
		{
			var keyContent = await GetKeyContentAsync(key);

			using var keyTextReader = new StringReader(keyContent);

			var passPhrase = _configuration.GetEnvironmentVariable(passPhraseKey);

			var privateKeyParameters = (Ed25519PrivateKeyParameters)new PemReader(keyTextReader, new PasswordFinder(passPhrase)).ReadObject();

			return privateKeyParameters;
		}

		private async Task<string> GetKeyContentAsync(string key)
		{
			var keyPath = _configuration.GetEnvironmentVariable(key);
			return await _keyProvider.GetKeyAsync(keyPath);
		}
	}
}
