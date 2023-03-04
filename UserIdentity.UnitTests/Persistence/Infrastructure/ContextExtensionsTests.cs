using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using UserIdentity.Domain.Identity;
using UserIdentity.Persistence;
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
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "AppDbContextTests")
				.Options;

			var entityPrefix = "";

			// Act
			using var context = new AppDbContext(options);

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
		}
	}
}
