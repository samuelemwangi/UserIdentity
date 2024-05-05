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
			var (privateKey, privateKeyPassPhrase) = await GetPrivateKeyAsync();

			using var privateKeyTextReader = new StringReader(privateKey);

			var pemReader = new PemReader(privateKeyTextReader, new PasswordFinder(privateKeyPassPhrase));

			var privateKeyParameters = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();

			var privateKeyRsaParameters = DotNetUtilities.ToRSAParameters(privateKeyParameters);

			return new RsaSecurityKey(privateKeyRsaParameters);
		}

		public async Task<AsymmetricSecurityKey> GetVerificationKeyAsync()
		{
			var publicKey = await GetPublicKeyAsync();

			using var publicKeyTextReader = new StringReader(publicKey);

			var pemReader = new PemReader(publicKeyTextReader);

			var publicKeyParameters = (RsaKeyParameters)pemReader.ReadObject();

			var rsaPublicKeyParameters = DotNetUtilities.ToRSAParameters(publicKeyParameters);

			return new RsaSecurityKey(rsaPublicKeyParameters);
		}

		public async Task<string> GetPublicKeyAsync()
		{
			var publicKeyPath = _configuration.GetEnvironmentVariable("APP_PUBLIC_KEY_PATH");
			return await _keyProvider.GetKeyAsync(publicKeyPath);
		}

		public async Task<string> GetBase64URLEncodedPublicKeyAsync()
		{
			return Base64UrlEncoder.Encode(await GetPublicKeyAsync());
		}

		private async Task<(string, string)> GetPrivateKeyAsync()
		{
			var privateKeyPath = _configuration.GetEnvironmentVariable("APP_PRIVATE_KEY_PATH");
			var privateKeyPassPhrase = _configuration.GetEnvironmentVariable("APP_PRIVATE_KEY_PASS_PHRASE");

			var privateKeyContent = await _keyProvider.GetKeyAsync(privateKeyPath);

			return (privateKeyContent, privateKeyPassPhrase);
		}
	}
}
