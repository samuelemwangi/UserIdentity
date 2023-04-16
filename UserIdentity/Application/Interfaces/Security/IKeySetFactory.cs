using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Application.Interfaces.Security
{
	public interface IKeySetFactory
	{
		String GetAlgorithm();
		String GetKeyType();
		String GetKeyId();
		String GetSecretKey();
		SymmetricSecurityKey GetSigningKey();
		String GetBase64URLEncodedSecretKey();
	}
}
