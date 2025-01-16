using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.Extensions.Options;

using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Security
{
	public class JwtFactory : IJwtFactory
	{
		private readonly JwtIssuerOptions _jwtOptions;
		private readonly IMachineDateTime _machineDateTime;

		private static readonly char _scopeClaimSeparator = ':';

		public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions, IMachineDateTime machineDateTime)
		{
			_jwtOptions = jwtOptions.Value;
			_machineDateTime = machineDateTime;
			ThrowIfInvalidOptions(_jwtOptions);
		}
		public async Task<(string, int)> GenerateEncodedTokenAsync(string id, string userName, IList<string> userRoles, HashSet<string> userRoleClaims)
		{
			var claims = new List<Claim>{
				new(JwtRegisteredClaimNames.Sub, userName),
				new(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
				new(JwtRegisteredClaimNames.Iat, _machineDateTime.ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
				new(JwtCustomClaimNames.Id, id)
			};

			claims.AddRange(userRoles.Select(role => new Claim(JwtCustomClaimNames.Rol, role)));

			if (userRoleClaims.Count > 0)
			{
				var scopes = string.Join(" ", userRoleClaims);
				claims.Add(new Claim(JwtCustomClaimNames.Scope, scopes));
			}

			var jwt = new JwtSecurityToken(
				_jwtOptions.Issuer,
				_jwtOptions.Audience,
				claims,
				_jwtOptions.NotBefore,
				_jwtOptions.Expiration,
				_jwtOptions.SigningCredentials
				);

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenString = tokenHandler.WriteToken(jwt);

			return (tokenString, (int)_jwtOptions.ValidFor.TotalSeconds);
		}

		public Claim GenerateScopeClaim(string resource, string action)
		{
			var scope = $"{resource}{_scopeClaimSeparator}{action}".ToLower();
			return new Claim(JwtCustomClaimNames.Scope, scope);
		}

		public (string, string) DecodeScopeClaim(Claim scopeClaim)
		{
			var scopeValues = scopeClaim.Value.Split(_scopeClaimSeparator);

			return (scopeValues[0], scopeValues[1]);
		}

		private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
		{
			ArgumentNullException.ThrowIfNull(options, nameof(options));

			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.ValidFor, TimeSpan.Zero, nameof(options.ValidFor));

			ArgumentNullException.ThrowIfNull(options.SigningCredentials, nameof(options.SigningCredentials));

			ArgumentNullException.ThrowIfNull(options.JtiGenerator, nameof(options.JtiGenerator));
		}
	}
}
