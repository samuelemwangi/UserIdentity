using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserIdentity.Application.Core.Roles.Commands.UpdateRole;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Commands
{
  public class UpdateRoleCommandHandlerTests
  {

    private readonly RoleManager<IdentityRole> _roleManager;
    public UpdateRoleCommandHandlerTests()
    {
      _roleManager = A.Fake<RoleManager<IdentityRole>>();
    }

    [Fact]
    public async Task Update_NonExisting_Role_Throws_NoRecordException()
    {
      // Arrange
      var command = new UpdateRoleCommand
      {
        RoleId = "1",
        RoleName = "Admin"
      };

      A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(default(IdentityRole));

      var handler = new UpdateRoleCommandHandler(_roleManager);

      // Act& Assert
      await Assert.ThrowsAsync<NoRecordException>(() => handler.UpdateItemAsync(command));
    }

    [Fact]
    public async Task Update_Existing_Role_Failure_Throws_RecordUpdateException()
    {
      // Arrange
      var command = new UpdateRoleCommand
      {
        RoleId = "1",
        RoleName = "Admin"
      };

      var identityRole = new IdentityRole
      {
        Id = command.RoleId,
        Name = "User"
      };

      A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(identityRole);

      A.CallTo(() => _roleManager.UpdateAsync(identityRole)).Returns(IdentityResult.Failed());

      var handler = new UpdateRoleCommandHandler(_roleManager);

      // Act& Assert
      await Assert.ThrowsAsync<RecordUpdateException>(() => handler.UpdateItemAsync(command));
    }

    [Fact]
    public async Task Update_Existing_Role_Updates_Successfully()
    {
      // Arrange
      var command = new UpdateRoleCommand
      {
        RoleId = "1",
        RoleName = "Admin"
      };

      var identityRole = new IdentityRole
      {
        Id = command.RoleId,
        Name = "User"
      };

      A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(identityRole);

      A.CallTo(() => _roleManager.UpdateAsync(identityRole)).Returns(IdentityResult.Success);

      var handler = new UpdateRoleCommandHandler(_roleManager);

      // Act
      var vm = await handler.UpdateItemAsync(command);

      // Assert
      Assert.IsType<RoleViewModel>(vm);
      Assert.IsType<RoleDTO>(vm.Role);
      Assert.Equal(command.RoleName, vm.Role.Name);
    }
  }
}
