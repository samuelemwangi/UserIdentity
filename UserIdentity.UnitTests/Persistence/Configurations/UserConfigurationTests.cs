using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Configurations.Identity;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Configurations
{
  public class UserConfigurationTests
  {
    [Fact]
    public void Configure_User_Should_Configure_User()
    {
      // Arrange
      var userConfiguration = new UserConfiguration();
      (var builder, var entityType) = EntityConfigurationTestsHelper<User>.GetEntityTypeBuilder();

      // Act
      userConfiguration.Configure(builder);

      // Assert
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(User.EmailConfirmationToken), 600));
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(User.FirstName), 20));
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(User.LastName), 20));
      Assert.True(entityType.ConfirmMaxColumnLength(nameof(User.ForgotPasswordToken), 600));

      Assert.True(entityType.ConfirmColumnHasKey(nameof(User.Id)));
    }
  }
}
