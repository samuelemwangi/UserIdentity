using System;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Domain.RefreshTokens;
using UserIdentity.Domain.Users;
using UserIdentity.Persistence.Infrastructure;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Infrastructure;

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
    Assert.Equal(entityPrefix + "user_details", context.GetTableName<UserEntity>());
    Assert.Equal(entityPrefix + "refresh_tokens", context.GetTableName<RefreshTokenEntity>());

    Assert.Equal(entityPrefix + "users", context.GetTableName<IdentityUser>());
    Assert.Equal(entityPrefix + "roles", context.GetTableName<IdentityRole>());

    Assert.Equal(entityPrefix + "user_claims", context.GetTableName<IdentityUserClaim<string>>());
    Assert.Equal(entityPrefix + "role_claims", context.GetTableName<IdentityRoleClaim<string>>());

    Assert.Equal(entityPrefix + "user_logins", context.GetTableName<IdentityUserLogin<string>>());
    Assert.Equal(entityPrefix + "user_roles", context.GetTableName<IdentityUserRole<string>>());

    Assert.Equal(entityPrefix + "user_tokens", context.GetTableName<IdentityUserToken<string>>());

    // For non-existent entity
    Assert.Null(context.GetTableName<Random>());
  }
}
