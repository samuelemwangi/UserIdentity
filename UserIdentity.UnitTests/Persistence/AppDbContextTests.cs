using System;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PolyzenKit.Domain.AppEntities;
using PolyzenKit.Domain.RegisteredApps;

using UserIdentity.Domain.RefreshTokens;
using UserIdentity.Domain.UserRegisteredApps;
using UserIdentity.Domain.Users;

using Xunit;

namespace UserIdentity.UnitTests.Persistence;

public class AppDbContextTests
{
  [Fact]
  public void OnModelCreating_Should_Update_Model()
  {
    // Arrange
    var entityPrefix = "";
    var stringType = "<string>";

    // Act
    using var context = AppDbContextTestFactory.GetAppDbContext();

    var entityTypes = context.Model.GetEntityTypes();

    // Assert	
    Assert.Equal(entityPrefix + "app_entities", entityTypes.Where(e => e.DisplayName() == nameof(AppEntity)).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "user_details", entityTypes.Where(e => e.DisplayName() == nameof(UserEntity)).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "refresh_tokens", entityTypes.Where(e => e.DisplayName() == nameof(RefreshTokenEntity)).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "registered_apps", entityTypes.Where(e => e.DisplayName() == nameof(RegisteredAppEntity)).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "user_registered_apps", entityTypes.Where(e => e.DisplayName() == nameof(UserRegisteredAppEntity)).FirstOrDefault()?.GetTableName());

    Assert.Equal(entityPrefix + "users", entityTypes.Where(e => e.DisplayName() == nameof(IdentityUser)).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "roles", entityTypes.Where(e => e.DisplayName() == nameof(IdentityRole)).FirstOrDefault()?.GetTableName());

    Assert.Equal(entityPrefix + "user_claims", entityTypes.Where(e => e.DisplayName() == nameof(IdentityUserClaim<String>) + stringType).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "role_claims", entityTypes.Where(e => e.DisplayName() == nameof(IdentityRoleClaim<String>) + stringType).FirstOrDefault()?.GetTableName());

    Assert.Equal(entityPrefix + "user_logins", entityTypes.Where(e => e.DisplayName() == nameof(IdentityUserLogin<String>) + stringType).FirstOrDefault()?.GetTableName());
    Assert.Equal(entityPrefix + "user_roles", entityTypes.Where(e => e.DisplayName() == nameof(IdentityUserRole<String>) + stringType).FirstOrDefault()?.GetTableName());

    Assert.Equal(entityPrefix + "user_tokens", entityTypes.Where(e => e.DisplayName() == nameof(IdentityUserToken<String>) + stringType).FirstOrDefault()?.GetTableName());
  }
}
