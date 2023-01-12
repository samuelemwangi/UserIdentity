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

		public String GetAlgorithm()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.Alg)] ?? SecurityAlgorithms.HmacSha256;
		}

		public String GetKeyType()
		{
			return _keySetConfigurationSection[nameof(KeySetOptions.KeyType)] ?? "oct";
		}

		public String GetKeyId()
		{
			String? envKeyId = _configuration.GetValue<String>("APP_KEY_ID");

			String keyId = String.IsNullOrEmpty(envKeyId)
				? _keySetConfigurationSection[nameof(KeySetOptions.KeyId)] ?? "APPV1KEYID"
				: envKeyId;

			return Base64UrlEncoder.Encode(keyId);
		}

		public String GetSecretKey()
		{
			String? envSecretKey = _configuration.GetValue<String?>("APP_SECRET_KEY");

			String? secretKey = String.IsNullOrEmpty(envSecretKey)
				? _keySetConfigurationSection[nameof(KeySetOptions.SecretKey)] ?? "KEY198*£%&YEK+OP}L"
				: envSecretKey;

			return secretKey;
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
