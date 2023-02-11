using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Infrastructure.Security;
using UserIdentity.Infrastructure.Utilities;

using Xunit;


namespace UserIdentity.UnitTests.Infrastructure.Security
{
	public class JwtTokenHandlerTest : IClassFixture<TestSettingsFixture>
	{
		private readonly TestSettingsFixture _testSettings;
		private readonly KeySetFactory _keySetFactory;
		private readonly ILogHelper<JwtTokenHandler> _logHelper;

		public JwtTokenHandlerTest(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;
			_keySetFactory = new KeySetFactory(_testSettings.Configuration);

			ILoggerFactory loggerFactory = new NullLoggerFactory();
			_logHelper = new LogHelper<JwtTokenHandler>(loggerFactory);
		}

		[Fact]
		public void ValidateToken_Thows_Exception_For_InValid_Token()
		{
			// Arrange
			var tokenHandler = new JwtTokenHandler(_keySetFactory, _logHelper);

			// Act & Assert
			Assert.Throws<SecurityTokenReadException>(() => tokenHandler.ValidateToken(null, new TokenValidationParameters()));
		}

		[Fact]
		public void ValidateToken_Validates_Token_For_Valid_Token()
		{
			// Arrange
			var tokenHandler = new JwtTokenHandler(_keySetFactory, _logHelper);

			var fullToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.5mhBHqs5_DTLdINd9p5m7ZJ6XD0Xc55kIaCRY5r6HRA";

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = false,
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false,
				SignatureValidator = delegate (String token, TokenValidationParameters parameters)
				{
					return new JwtSecurityToken(token);
				}

			};

			// Act & Assert
			Assert.IsType<ClaimsPrincipal>(tokenHandler.ValidateToken(fullToken, tokenValidationParameters));
		}

		[Fact]
		public void ValidateToken_Thows_Exception_For_InValid_Token_Issuer()
		{
			// Arrange
			var tokenHandler = new JwtTokenHandler(_keySetFactory, _logHelper);

			var fullToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.5mhBHqs5_DTLdINd9p5m7ZJ6XD0Xc55kIaCRY5r6HRA";

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = false,
				ValidateIssuer = true,
				ValidIssuer = "testIssuer",
				ValidateAudience = false,
				ValidateLifetime = false,
				SignatureValidator = delegate (String token, TokenValidationParameters parameters)
				{
					return new JwtSecurityToken(token);
				}

			};

			// Act & Assert
			Assert.Throws<SecurityTokenReadException>(() => tokenHandler.ValidateToken(fullToken, tokenValidationParameters));
		}

		[Fact]
		public void WriteToken_Writes_Token()
		{
			// Arrange
			var tokenHandler = new JwtTokenHandler(_keySetFactory, _logHelper);

			// Act & Assert
			Assert.IsType<String>(tokenHandler.WriteToken(new JwtSecurityToken()));
		}
	}
}

