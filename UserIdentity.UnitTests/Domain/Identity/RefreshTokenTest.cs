using System;
using UserIdentity.Domain;
using UserIdentity.Domain.Identity;
using Xunit;

namespace UserIdentity.UnitTests.Domain.Identity
{
	public class RefreshTokenTest
	{
		[Fact]
		public void New_RefreshToken_is_a_Valid_RefreshToken_Instance()
		{
			RefreshToken refreshToken = new();

			Assert.IsAssignableFrom<BaseEntity>(refreshToken);
        }
	}
}

