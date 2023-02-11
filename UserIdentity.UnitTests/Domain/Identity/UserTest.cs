using System;

using UserIdentity.Domain;
using UserIdentity.Domain.Identity;

using Xunit;

namespace UserIdentity.UnitTests.Domain.Identity
{
	public class UserTest
	{
		[Fact]
		public void New_User_is_a_Valid_User_Instance()
		{
			// Arrange
			User user = new();

			// Act & Assert
			Assert.IsAssignableFrom<BaseEntity>(user);
		}
	}
}

