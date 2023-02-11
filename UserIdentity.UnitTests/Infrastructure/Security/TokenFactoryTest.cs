using System;

using UserIdentity.Infrastructure.Security;

using Xunit;

namespace UserIdentity.UnitTests.Infrastructure.Security
{
	public class TokenFactoryTest : IClassFixture<TestSettingsFixture>
	{
		[Fact]
		public void Get_RefreshToken_Returns_RefreshToken()
		{
			// Arrange
			var tokenFactory = new TokenFactory();

			// Act & Assert
			Assert.False(String.IsNullOrEmpty(tokenFactory.GenerateRefreshToken()));
		}

		[Fact]
		public void Get_OTP_Token_Throws_Not_Implemented_Exception()
		{
			// Arrange
			var tokenFactory = new TokenFactory();

			// Act & Assert
			Assert.Throws<NotImplementedException>(() => tokenFactory.GenerateOTPToken());
		}
	}
}

