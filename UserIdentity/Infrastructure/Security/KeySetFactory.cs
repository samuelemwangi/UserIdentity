using System.Text;

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
			string? envKeyId = _configuration.GetValue<string>("APP_KEY_ID");

			string keyId = string.IsNullOrEmpty(envKeyId)
				? _keySetConfigurationSection[nameof(KeySetOptions.KeyId)] ?? "APPV1KEYID"
				: envKeyId;

			return Base64UrlEncoder.Encode(keyId);
		}

		public string GetSecretKey()
		{
			string? envSecretKey = _configuration.GetValue<string?>("APP_SECRET_KEY");

			string? secretKey = string.IsNullOrEmpty(envSecretKey)
				? _keySetConfigurationSection[nameof(KeySetOptions.SecretKey)] ?? "KEY198*£%&YEK+OP}L5H0ULD>32CH8Rz"
				: envSecretKey;

			return secretKey.Length < 32
				? throw new SecurityTokenInvalidSigningKeyException("Invalid key provided. Security key should be at least 32 characters")
				: secretKey;
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
