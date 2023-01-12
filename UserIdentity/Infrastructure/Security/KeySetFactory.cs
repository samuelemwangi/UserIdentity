using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Infrastructure.Security
{
	public class KeySetFactory : IKeySetFactory
	{
		private readonly IConfigurationSection _keySetConfigurationSection;
		private readonly IConfiguration _configuration;

		public KeySetFactory(IConfiguration configuration)
		{
			_keySetConfigurationSection = configuration.GetSection(nameof(KeySetOptions));
			_configuration = configuration;
		}

		public string GetAlgorithm()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.Alg)] ?? SecurityAlgorithms.HmacSha256;
		}

		public string GetKeyType()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.KeyType)] ?? "oct";
		}

		public string GetKeyId()
		{
			string keyId =  _keySetConfigurationSection[nameof(KeySetOptions.KeyId)] ?? "NYAMASHOPV1";
			return Base64UrlEncoder.Encode(keyId);
		}

		public string GetSecretKey()
		{
			string? envSecretKey = _configuration.GetValue<string?>("SECRET_KEY");

			string? secretKey = string.IsNullOrEmpty(envSecretKey) ? _keySetConfigurationSection[nameof(KeySetOptions.SecretKey)] : envSecretKey;

			return secretKey ?? "KEY198*£%&YEK+OP}L";
		}

		public SymmetricSecurityKey GetSigningKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(GetSecretKey()));
		}
		public string GetBase64URLEncodedSecretKey()
		{

			string secretKey = GetSecretKey();

			return Base64UrlEncoder.Encode(secretKey);
		}
	}
}
