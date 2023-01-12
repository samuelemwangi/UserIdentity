using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Application.Interfaces.Security
{
	public interface IKeySetFactory
	{
		string GetAlgorithm();
		string GetKeyType();
		string GetKeyId();
		string GetSecretKey();
		SymmetricSecurityKey GetSigningKey();
		string GetBase64URLEncodedSecretKey();
	}
}
