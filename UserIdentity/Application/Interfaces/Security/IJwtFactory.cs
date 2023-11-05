using System.Security.Claims;

namespace UserIdentity.Application.Interfaces.Security
{
  public interface IJwtFactory
  {
    Task<(String, Int32)> GenerateEncodedTokenAsync(String id, String userName, IList<String> userRoles, HashSet<String> userRoleClaims);
    Claim GenerateScopeClaim(String resource, String action);
    (String, String) DecodeScopeClaim(Claim scopeClaim);
  }
}
