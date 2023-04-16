using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserIdentity.Domain;

using Xunit;

namespace UserIdentity.UnitTests.Domain
{
	public class TestEntity : BaseEntity
	{

	}

	public class EntityExtensionsTests
	{
		[Fact]
		public void Set_Audit_Fields_Sets_Audit_Fields()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var now = DateTime.UtcNow;

			var testEntity = new TestEntity();

			// Act
			testEntity.SetAuditFields(userId, now);

			// Assert
			Assert.Equal(userId, testEntity.CreatedBy);
			Assert.Equal(userId, testEntity.UpdatedBy);
			Assert.Equal(now, testEntity.CreatedAt);
			Assert.Equal(now, testEntity.UpdatedAt);
			Assert.False(testEntity.IsDeleted);
		}

		[Fact]
		public void Update_Audit_Fields_Updates_Audit_Fields()
		{
			// Arrange
			var createdByUserId = Guid.NewGuid().ToString();
			var createdDateTime = DateTime.UtcNow.AddDays(30);
			var userId = Guid.NewGuid().ToString();
			var now = DateTime.UtcNow;

			var testEntity = new TestEntity();
			testEntity.CreatedAt = createdDateTime;
			testEntity.CreatedBy = createdByUserId;

			var testEntity2 = new TestEntity();
			testEntity2.CreatedAt = createdDateTime;
			testEntity2.CreatedBy = createdByUserId;
			testEntity2.UpdatedAt = createdDateTime;

			// Act
			testEntity.UpdateAuditFields(userId, now);
			testEntity2.UpdateAuditFields(userId, now, true);

			// Assert
			Assert.Equal(createdByUserId, testEntity.CreatedBy);
			Assert.Equal(userId, testEntity.UpdatedBy);
			Assert.Equal(createdDateTime, testEntity.CreatedAt);
			Assert.Equal(now, testEntity.UpdatedAt);
			Assert.False(testEntity.IsDeleted);

			Assert.Equal(createdByUserId, testEntity2.CreatedBy);
			Assert.Equal(userId, testEntity2.UpdatedBy);
			Assert.Equal(createdDateTime, testEntity2.CreatedAt);
			Assert.Equal(createdDateTime, testEntity2.UpdatedAt);
			Assert.True(testEntity2.IsDeleted);

		}

	}
}
