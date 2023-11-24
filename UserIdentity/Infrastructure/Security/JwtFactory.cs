using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

using Microsoft.Extensions.Options;

using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Security.Helpers;
using UserIdentity.Infrastructure.Security.Interfaces;

namespace UserIdentity.Infrastructure.Security
{
	public class JwtFactory : IJwtFactory
	{
		private readonly IJwtTokenHandler _jwtTokenHandler;
		private readonly JwtIssuerOptions _jwtOptions;
		private readonly IMachineDateTime _machineDateTime;

		private Char scopeClaimSeparator => ':';

		public JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions, IMachineDateTime machineDateTime)
		{
			_jwtTokenHandler = jwtTokenHandler;
			_jwtOptions = jwtOptions.Value;
			_machineDateTime = machineDateTime;
			ThrowIfInvalidOptions(_jwtOptions);

		}
		public async Task<(String, Int32)> GenerateEncodedTokenAsync(String id, String userName, IList<String> userRoles, HashSet<String> userRoleClaims)
		{
			var identity = GenerateClaimsIdentity(id, userName);
			var roleClaims = userRoles.Select(x => new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, x)).ToArray();

			var claims = new[]{
				new Claim(JwtRegisteredClaimNames.Sub, userName),
				new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
				new Claim(JwtRegisteredClaimNames.Iat, _machineDateTime.ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
				identity.FindFirst(Constants.Strings.JwtClaimIdentifiers.Id),
			};


			var combinedClaims = new Claim[roleClaims.Length + claims.Length];

			// add first array
			Array.Copy(claims, combinedClaims, claims.Length);

			// add roleClaims array
			Array.Copy(roleClaims, 0, combinedClaims, claims.Length, roleClaims.Length);

			// Get Scopes
			String scopes = "";
			bool scopesExist = false;
			if (userRoleClaims.Count > 0)
			{
				scopes = String.Join(" ", userRoleClaims);
				scopesExist = true;
			}


			// Create the JWT security token and encode it.
			var jwt = new JwtSecurityToken(
							_jwtOptions.Issuer,
							_jwtOptions.Audience,
							//  Add role scopes if they exist
							scopesExist ? combinedClaims.Append(new Claim(Constants.Strings.JwtClaimIdentifiers.Scope, scopes)) : combinedClaims,
							_jwtOptions.NotBefore,
							_jwtOptions.Expiration,
							_jwtOptions.SigningCredentials);

			return (_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);

		}

		public Claim GenerateScopeClaim(String resource, String action)
		{
			var scope = $"{resource}{scopeClaimSeparator}{action}".ToLower();
			return new Claim(Constants.Strings.JwtClaimIdentifiers.Scope, scope);
		}

		public (String, String) DecodeScopeClaim(Claim scopeClaim)
		{
			var scopeValues = scopeClaim.Value.Split(scopeClaimSeparator);

			return (scopeValues[0], scopeValues[1]);
		}

		private static ClaimsIdentity GenerateClaimsIdentity(String id, String userName)
		{
			return new ClaimsIdentity(
				new GenericIdentity(userName, "Token"),
				new[] { new Claim(Constants.Strings.JwtClaimIdentifiers.Id, id), }
				);
		}

		private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
		{
			//check if null
			if (options == null) throw new ArgumentNullException(nameof(options));

			//check if valid for is set
			if (options.ValidFor <= TimeSpan.Zero)
			{
				throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
			}

			//check if Signing Credentials is set
			if (options.SigningCredentials == null)
			{
				throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
			}
			//check if Jti Generator i set
			if (options.JtiGenerator == null)
			{
				throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
			}

		}
	}
}
