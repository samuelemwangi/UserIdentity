using System;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Extensions;
using UserIdentity.Domain;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Extensions
{
	public record TestBaseEntityDTO : BaseEntityDTO
	{
	}

	public class TesBaseEntity : BaseEntity
	{
	}

	public class DTOExtensionsTests
	{
		[Fact]
		public void Resolve_Owned_By_Logged_In_User_Resolves()
		{
			// Arrange
			var id = Guid.NewGuid();
			var userId = Guid.NewGuid().ToString();
			var otherUserId = Guid.NewGuid().ToString();

			var testDTO = null as TestBaseEntityDTO;
			var testDTO1 = new TestBaseEntityDTO();
			var testDTO2 = new TestBaseEntityDTO { Id = id, CreatedBy = userId, UpdatedBy = userId };
			var testDTO3 = new TestBaseEntityDTO { Id = id, CreatedBy = userId, UpdatedBy = otherUserId };
			var testDTO4 = new TestBaseEntityDTO { Id = id, CreatedBy = otherUserId, UpdatedBy = userId };


			// Act & Assert
			Assert.False(testDTO.OwnedByLoggedInUser(userId));

			Assert.False(testDTO1.OwnedByLoggedInUser(userId));

			Assert.True(testDTO2.OwnedByLoggedInUser(userId));
			Assert.False(testDTO2.OwnedByLoggedInUser(otherUserId));
			Assert.Equal(id, testDTO2.Id);

			Assert.True(testDTO3.OwnedByLoggedInUser(userId));
			Assert.False(testDTO3.OwnedByLoggedInUser(null));
			Assert.Equal(id, testDTO3.Id);

			Assert.True(testDTO4.OwnedByLoggedInUser(otherUserId));
			Assert.False(testDTO4.OwnedByLoggedInUser(null));
			Assert.Equal(id, testDTO4.Id);
		}

		[Fact]
		public void Set_DTO_Audit_Fields_Sets_Audit_Fields()
		{
			// Arrange
			var id = Guid.NewGuid();
			var userId = Guid.NewGuid().ToString();
			var otherUserId = Guid.NewGuid().ToString();
			var now = DateTime.Now;
			var nowString = now.ToString();

			var testDTO = new TestBaseEntityDTO();
			var testEntity = new TesBaseEntity { Id = id, CreatedBy = userId, UpdatedBy = otherUserId, CreatedAt = now, UpdatedAt = now };

			// Act
			testDTO.SetDTOAuditFields(testEntity, (dt) => dt?.ToString());

			// Assert
			Assert.Equal(userId, testDTO.CreatedBy);
			Assert.Equal(nowString, testDTO.CreatedAt);
			Assert.Equal(otherUserId, testDTO.UpdatedBy);
			Assert.Equal(nowString, testDTO.UpdatedAt);
		}
	}
}
