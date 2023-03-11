using System;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Extensions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Extensions
{
	public record TestBaseEntityDTO : BaseEntityDTO
	{
	}

	public class DTOExtensionsTests
	{
		[Fact]
		public void Resolve_Owned_By_Logged_In_User_Resolves()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var otherUserId = Guid.NewGuid().ToString();

			var testDTO = null as TestBaseEntityDTO;
			var testDTO1 = new TestBaseEntityDTO();
			var testDTO2 = new TestBaseEntityDTO { CreatedBy = userId, LastModifiedBy = userId };
			var testDTO3 = new TestBaseEntityDTO { CreatedBy = userId, LastModifiedBy = otherUserId };
			var testDTO4 = new TestBaseEntityDTO { CreatedBy = otherUserId, LastModifiedBy = userId };


			// Act & Assert
			Assert.False(testDTO.OwnedByLoggedInUser(userId));

			Assert.False(testDTO1.OwnedByLoggedInUser(userId));

			Assert.True(testDTO2.OwnedByLoggedInUser(userId));
			Assert.False(testDTO2.OwnedByLoggedInUser(otherUserId));

			Assert.True(testDTO3.OwnedByLoggedInUser(userId));
			Assert.False(testDTO3.OwnedByLoggedInUser(null));

			Assert.True(testDTO4.OwnedByLoggedInUser(otherUserId));
			Assert.False(testDTO4.OwnedByLoggedInUser(null));
		}
	}
}
