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
            TokenFactory tokenFactory = new TokenFactory();
            Assert.False(String.IsNullOrEmpty(tokenFactory.GenerateRefreshToken()));
        }

        [Fact]
        public void Get_OTP_Token_Throws_Not_Implemented_Exception()
        {
            TokenFactory tokenFactory = new TokenFactory();
            Assert.Throws<NotImplementedException>(() => tokenFactory.GenerateOTPToken());
        }
    }
}

