using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Application.Interfaces.Security
{
	public interface IKeySetFactory
	{
		string GetAlgorithm();
		string GetKeyType();
		string GetKeyId();
		Task<AsymmetricSecurityKey> GetSigningKeyAsync();
		Task<AsymmetricSecurityKey> GetVerificationKeyAsync();
		string GetCrvValue();
		Task<string> GetXValueAysnc();
	}
}
