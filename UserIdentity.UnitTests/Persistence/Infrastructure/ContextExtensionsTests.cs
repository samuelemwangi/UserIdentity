using Microsoft.AspNetCore.Identity;

using System;

using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Infrastructure;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Infrastructure
{
	public class ContextExtensionsTests
	{

		[Fact]
		public void Get_Table_Name_Returns_Actual_Table_Name()
		{
			// Arrange
			var entityPrefix = "";

			// Act
			using var context = AppDbContextTestFactory.GetAppDbContext();

			// Assert	
			Assert.Equal(entityPrefix + "user_details", context.GetTableName<User>());
			Assert.Equal(entityPrefix + "refresh_tokens", context.GetTableName<RefreshToken>());

			Assert.Equal(entityPrefix + "users", context.GetTableName<IdentityUser>());
			Assert.Equal(entityPrefix + "roles", context.GetTableName<IdentityRole>());

			Assert.Equal(entityPrefix + "user_claims", context.GetTableName<IdentityUserClaim<String>>());
			Assert.Equal(entityPrefix + "role_claims", context.GetTableName<IdentityRoleClaim<String>>());

			Assert.Equal(entityPrefix + "user_logins", context.GetTableName<IdentityUserLogin<String>>());
			Assert.Equal(entityPrefix + "user_roles", context.GetTableName<IdentityUserRole<String>>());

			Assert.Equal(entityPrefix + "user_tokens", context.GetTableName<IdentityUserToken<String>>());

			// For non-existent entity
			Assert.Null(context.GetTableName<Random>());
		}

		[Fact]
		public void Get_Schema_Name_Returns_Actual_Schema_Name()
		{
			// Act
			using var context = AppDbContextTestFactory.GetAppDbContext();

			// Assert	
			Assert.Null(context.GetSchemaName<User>());
			Assert.Null(context.GetSchemaName<RefreshToken>());

			Assert.Null(context.GetSchemaName<IdentityUser>());
			Assert.Null(context.GetSchemaName<IdentityRole>());

			Assert.Null(context.GetSchemaName<IdentityUserClaim<String>>());
			Assert.Null(context.GetSchemaName<IdentityRoleClaim<String>>());

			Assert.Null(context.GetSchemaName<IdentityUserLogin<String>>());
			Assert.Null(context.GetSchemaName<IdentityUserRole<String>>());

			Assert.Null(context.GetSchemaName<IdentityUserToken<String>>());

			// For non-existent entity
			Assert.Null(context.GetTableName<Random>());
		}
	}
}
