using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Configurations.Identity;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Configurations
{
  public class RefreshTokenConfigurationTests
  {
    [Fact]
    public void Configure_RefreshToken_Should_Configure_RefreshToken()
    {
      // Arrange
      var refreshTokenConfiguration = new RefreshTokenConfiguration();

      (var builder, var entityType) = EntityConfigurationTestsHelper<RefreshToken>.GetEntityTypeBuilder();

      // Act
      refreshTokenConfiguration.Configure(builder);

      // Assert
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(RefreshToken.Token), 200));
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(RefreshToken.UserId), 50));
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(RefreshToken.RemoteIpAddress), 20));

      Assert.True(entityType.ConfirmColumnHasIndex(nameof(RefreshToken.Token)));
      Assert.True(entityType.ConfirmColumnHasIndex(nameof(RefreshToken.UserId)));
    }
  }


}
