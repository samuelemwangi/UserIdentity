using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Security
{
    public class JwtTokenHandler : IJwtTokenHandler
	{
		private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
		private readonly IKeySetFactory _keySetFactory;
		private readonly ILogHelper<JwtTokenHandler> _logHelper;

		public JwtTokenHandler(IKeySetFactory keySetFactory, ILogHelper<JwtTokenHandler> logHelper)
		{
			_jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
			_keySetFactory = keySetFactory;
			_logHelper = logHelper;
		}
		public ClaimsPrincipal ValidateToken(string? token, TokenValidationParameters tokenValidationParameters)
		{
			try
			{
				var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

				if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(_keySetFactory.GetAlgorithm(), StringComparison.InvariantCultureIgnoreCase))
					throw new SecurityTokenException("Invalid token provided");

				return principal;
			}
			catch (Exception e)
			{
				throw new SecurityTokenReadException(e.Message);
			}
		}

		public string WriteToken(JwtSecurityToken jwt)
		{
			return _jwtSecurityTokenHandler.WriteToken(jwt);
		}
	}
}
