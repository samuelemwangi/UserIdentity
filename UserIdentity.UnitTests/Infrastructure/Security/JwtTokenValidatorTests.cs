using System;
using System.Collections.Generic;
using System.Security.Claims;

using FakeItEasy;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Infrastructure.Security.Interfaces;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security
{
	public class JwtTokenValidatorTests
	{
		private readonly IJwtTokenHandler _jwtTokenHandler;
		private readonly IKeySetFactory _keySetFactory;

		public JwtTokenValidatorTests()
		{
			_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
			_keySetFactory = A.Fake<IKeySetFactory>();
		}

		[Fact]
		public void GetPrincipalFromToken_For_Valid_Token_Returns_Claims_Principal()
		{
			// Arrange
			var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.5mhBHqs5_DTLdINd9p5m7ZJ6XD0Xc55kIaCRY5r6HRA";
			var claimsPricipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("TestClaim", "TestClaimValue") }));

			A.CallTo(() => _jwtTokenHandler.ValidateToken(accessToken, A<TokenValidationParameters>.That.Matches(tvp => ResolveTVPMatch(tvp)))).Returns(claimsPricipal);

			// Act
			var jwtTokenValidator = new JwtTokenValidator(_jwtTokenHandler, _jwtIssuerOptions, _keySetFactory);
			var actualClaimsPrincipal = jwtTokenValidator.GetPrincipalFromToken(accessToken);

			// Assert
			Assert.IsType<ClaimsPrincipal>(actualClaimsPrincipal);
			Assert.Equal(claimsPricipal, actualClaimsPrincipal);
		}

		private bool ResolveTVPMatch(TokenValidationParameters tvp)
		{
			return tvp.ValidIssuer == _jwtIssuerOptions.Value.Issuer &&
				tvp.ValidAudience == _jwtIssuerOptions.Value.Audience &&
				tvp.ValidateIssuer &&
				tvp.ValidateAudience &&
				!tvp.ValidateLifetime;
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
