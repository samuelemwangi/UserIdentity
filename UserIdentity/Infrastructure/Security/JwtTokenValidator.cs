using System.Security.Claims;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Infrastructure.Security
{
	public class JwtTokenValidator : IJwtTokenValidator
	{
		private readonly IJwtTokenHandler _jwtTokenHandler;
		private readonly JwtIssuerOptions _jwtOptions;
		private readonly IKeySetFactory _keySetFactory;
		public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions, IKeySetFactory keySetFactory)
		{
			_jwtTokenHandler = jwtTokenHandler;
			_jwtOptions = jwtOptions.Value;
			_keySetFactory = keySetFactory;
		}
		public ClaimsPrincipal GetPrincipalFromToken(string accesToken)
		{
			return _jwtTokenHandler.ValidateToken(accesToken, new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidAudience = _jwtOptions.Audience,

				ValidateIssuer = true,
				ValidIssuer = _jwtOptions.Issuer,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _keySetFactory.GetVerificationKeyAsync().Result,
				ValidateLifetime = false
			});
		}
	}
}
