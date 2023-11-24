using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

namespace UserIdentity.Infrastructure.Security.Interfaces
{
	public interface IJwtTokenHandler
	{
		String WriteToken(JwtSecurityToken jwt);
		ClaimsPrincipal ValidateToken(String? token, TokenValidationParameters tokenValidationParameters);
	}
}
