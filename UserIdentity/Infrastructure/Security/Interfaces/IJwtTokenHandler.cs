using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UserIdentity.Infrastructure.Security.Interfaces
{
	public interface IJwtTokenHandler
	{
		String WriteToken(JwtSecurityToken jwt);
		ClaimsPrincipal ValidateToken(String? token, TokenValidationParameters tokenValidationParameters);
	}
}
