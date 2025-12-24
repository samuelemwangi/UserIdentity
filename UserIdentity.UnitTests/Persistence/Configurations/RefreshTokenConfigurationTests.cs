using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Configurations.Identity;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Configurations;

public class RefreshTokenConfigurationTests
{
  [Fact]
  public void Configure_RefreshToken_Should_Configure_RefreshToken()
  {
    // Arrange
    var refreshTokenConfiguration = new RefreshTokenConfiguration();

    (var builder, var entityType) = EntityConfigurationTestsHelper<RefreshTokenEntity>.GetEntityTypeBuilder();

    // Act
    refreshTokenConfiguration.Configure(builder);

    // Assert
    Assert.True(entityType.ConfirmMaxColumnLength(nameof(RefreshTokenEntity.Token), 200));
    Assert.True(entityType.ConfirmMaxColumnLength(nameof(RefreshTokenEntity.UserId), 50));
    Assert.True(entityType.ConfirmMaxColumnLength(nameof(RefreshTokenEntity.RemoteIpAddress), 20));

    Assert.True(entityType.ConfirmColumnHasIndex(nameof(RefreshTokenEntity.Token)));
    Assert.True(entityType.ConfirmColumnHasIndex(nameof(RefreshTokenEntity.UserId)));
  }
}
