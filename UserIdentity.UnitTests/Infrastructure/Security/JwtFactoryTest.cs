using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Infrastructure.Security.Helpers;
using UserIdentity.Infrastructure.Security.Interfaces;
using UserIdentity.Infrastructure.Utilities;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security
{
	public class JwtFactoryTest
	{
		private readonly IJwtTokenHandler _jwtTokenHandler;
		private readonly IMachineDateTime _machineDateTime;

		public JwtFactoryTest()
		{
			_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
			_machineDateTime = new MachineDateTime();
		}


		[Fact]
		public async Task GenerateEncodedToken_WhenCalled_ReturnsToken()
		{
			// Arrange
			var issuer = _jwtIssuerOptions.Value.Issuer ?? "TestIssuer";
			var audience = _jwtIssuerOptions.Value.Audience ?? "TestAudience";
			var userName = _jwtIssuerOptions.Value.Subject ?? "TestUser";

			var jti = await _jwtIssuerOptions.Value.JtiGenerator();

			var issuedAt = _jwtIssuerOptions.Value.IssuedAt;
			var validFor = _jwtIssuerOptions.Value.ValidFor;
			var expiration = _jwtIssuerOptions.Value.Expiration;
			var signingCredentials = _jwtIssuerOptions.Value.SigningCredentials;

			var userId = Guid.NewGuid().ToString();
			var issuedAtEpoch = _machineDateTime.ToUnixEpochDate(issuedAt).ToString();

			var userRoles = new List<String> { "TestRole" };
			var userRoleClaims = new HashSet<String> { "TestRoleClaim" };


			var combinedClaims = new List<Claim>
						{
								new Claim(JwtRegisteredClaimNames.Sub, userName),
								new Claim(JwtRegisteredClaimNames.Jti, jti),
								new Claim(JwtRegisteredClaimNames.Iat, issuedAtEpoch, ClaimValueTypes.Integer64),
								new Claim(Constants.Strings.JwtClaimIdentifiers.Id, userId),
								new Claim(Constants.Strings.JwtClaimIdentifiers.Rol, userRoles[0]),
								new Claim(Constants.Strings.JwtClaimIdentifiers.Scope, userRoleClaims.ToArray()[0])
						};

			var jwt = new JwtSecurityToken(issuer, audience, combinedClaims, issuedAt, expiration, signingCredentials);

			var generatedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

			A.CallTo(() => _jwtTokenHandler.WriteToken(A<JwtSecurityToken>.That.Matches(currJwt => ResolveMathcingJWT(currJwt, userName, issuedAtEpoch, userId)))).Returns(generatedToken);

			// Act
			var jwtFactory = new JwtFactory(_jwtTokenHandler, _jwtIssuerOptions, _machineDateTime);
			(String token, int expiresIn) = await jwtFactory.GenerateEncodedTokenAsync(userId, userName, userRoles, userRoleClaims);

			// Assert
			Assert.Equal(generatedToken, token);
			Assert.Equal((int)validFor.TotalSeconds, expiresIn);
		}

		private bool ResolveMathcingJWT(JwtSecurityToken currJWT, String userName, String issuedAtEpoch, String userId)
		{
			//check if subject claim matches
			if (currJWT.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userName) == null)
				return false;

			// check if iat claim matches
			if (currJWT.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat && c.Value == issuedAtEpoch) == null)
				return false;

			// check if id claim matches
			if (currJWT.Claims.FirstOrDefault(c => c.Type == Constants.Strings.JwtClaimIdentifiers.Id && c.Value == userId) == null)
				return false;

			return true;
		}




		private static IOptions<JwtIssuerOptions> _jwtIssuerOptions => new OptionsWrapper<JwtIssuerOptions>(new JwtIssuerOptions
		{
			Issuer = "TestIssuer",
			Audience = "TestAudience",
			Subject = "TestSubject",
			ValidFor = TimeSpan.FromMinutes(5),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(new byte[32]), SecurityAlgorithms.HmacSha256)
		});
	}
}

