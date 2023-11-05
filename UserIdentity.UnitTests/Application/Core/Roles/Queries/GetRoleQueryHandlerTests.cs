using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UserIdentity.Application.Core.Roles.Queries.GetRole;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Queries
{
  public class GetRoleQueryHandlerTests
  {
    private readonly RoleManager<IdentityRole> _roleManager;
    public GetRoleQueryHandlerTests()
    {
      _roleManager = A.Fake<RoleManager<IdentityRole>>();

    }

    [Fact]
    public async Task Get_Role_Returns_Role()
    {
      // Arrange
      var query = new GetRoleQuery
      {
        RoleId = "1"
      };

      var role = new IdentityRole
      {
        Id = "1",
        Name = "Admin"
      };

      A.CallTo(() => _roleManager.FindByIdAsync(query.RoleId)).Returns(role);

      var handler = new GetRoleQueryHandler(_roleManager);

      // Act
      var vm = await handler.GetItemAsync(query);

      // Assert
      Assert.IsType<RoleViewModel>(vm);
      Assert.Equal(role.Id, vm.Role.Id);
      Assert.Equal(role.Name, vm.Role.Name);
    }

    [Fact]
    public async Task Get_Role_Throws_NoRecordException()
    {
      // Arrange
      var query = new GetRoleQuery
      {
        RoleId = "1"
      };

      A.CallTo(() => _roleManager.FindByIdAsync(query.RoleId)).Returns(default(IdentityRole));

      var handler = new GetRoleQueryHandler(_roleManager);

      // Act & Assert
      await Assert.ThrowsAsync<NoRecordException>(() => handler.GetItemAsync(query));

    }

  }
}
