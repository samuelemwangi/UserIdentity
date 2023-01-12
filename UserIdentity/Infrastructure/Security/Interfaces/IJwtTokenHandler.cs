using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UserIdentity.Infrastructure.Security.Interfaces
{
	public interface IJwtTokenHandler
	{
		string WriteToken(JwtSecurityToken jwt);
		ClaimsPrincipal ValidateToken(string? token, TokenValidationParameters tokenValidationParameters);
	}
}
