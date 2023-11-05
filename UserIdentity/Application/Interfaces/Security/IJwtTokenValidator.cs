using System.Security.Claims;

namespace UserIdentity.Application.Interfaces.Security
{
  public interface IJwtTokenValidator
  {
    ClaimsPrincipal GetPrincipalFromToken(String accesToken);
  }
}
