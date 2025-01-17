using System.Security.Claims;

namespace UserIdentity.Application.Interfaces.Security
{
	public interface IJwtFactory
	{
		(string, int) GenerateEncodedToken(string id, string userName, IList<string> userRoles, HashSet<string> userRoleClaims);
		Claim GenerateScopeClaim(string resource, string action);
		(string, string) DecodeScopeClaim(Claim scopeClaim);
	}
}
