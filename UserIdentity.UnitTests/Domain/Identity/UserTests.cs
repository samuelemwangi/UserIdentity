using PolyzenKit.Domain.Entity;

using UserIdentity.Domain;
using UserIdentity.Domain.Identity;

using Xunit;

namespace UserIdentity.UnitTests.Domain.Identity
{
	public class UserTests
	{
		[Fact]
		public void New_User_is_a_Valid_User_Instance()
		{
			// Arrange
			User user = new() { Id = "" };

			// Act & Assert
			Assert.IsType<BaseEntity<string>>(user, exactMatch: false);
		}
	}
}

