using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Queries
{
  public class GetRoleClaimsQueryHandlerTests
  {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtFactory _jwtFactory;
    public GetRoleClaimsQueryHandlerTests()
    {
      _roleManager = A.Fake<RoleManager<IdentityRole>>();
      _jwtFactory = A.Fake<IJwtFactory>();
    }

    [Fact]
    public async Task Get_RoleClaims_When_Role_Is_Not_Found_Throws_NoRecordException()
    {
      // Arrange
      var query = new GetRoleClaimsQuery
      {
        RoleId = "1"
      };

      A.CallTo(() => _roleManager.FindByIdAsync(query.RoleId)).Returns((default(IdentityRole)));

      var handler = new GetRoleClaimsQueryHandler(_roleManager, _jwtFactory);


      // Act & Assert
      await Assert.ThrowsAsync<NoRecordException>(() => handler.GetItemsAsync(query));
    }

    [Fact]
    public async Task Get_RoleClaims_Returns_RoleClaims()
    {
      // Arrange
      var query = new GetRoleClaimsQuery
      {
        RoleId = "1"
      };

      var role = new IdentityRole
      {
        Id = "1",
        Name = "Admin"
      };

      var resource = "resource";
      var action = "action";

      var roleClaims = new List<Claim>
      {
        new Claim("scope", $"{resource}:{action}")
      };



      A.CallTo(() => _roleManager.FindByIdAsync(query.RoleId)).Returns(role);

      A.CallTo(() => _roleManager.GetClaimsAsync(role)).Returns(roleClaims);

      A.CallTo(() => _jwtFactory.DecodeScopeClaim(roleClaims[0])).Returns((resource, action));

      var handler = new GetRoleClaimsQueryHandler(_roleManager, _jwtFactory);

      // Act
      var vm = await handler.GetItemsAsync(query);

      // Assert
      Assert.IsType<RoleClaimsViewModel>(vm);
      Assert.IsAssignableFrom<ICollection<RoleClaimDTO>>(vm.RoleClaims);
      Assert.Equal(roleClaims.Count, vm.RoleClaims.Count);
    }

    [Fact]
    public async Task Get_RoleClaims_Given_Roles_Returns_Unique_RoleClaims()
    {

      // Arrange
      var roles = new List<String>
      {
        "Admin"
      };

      var identityRole = new IdentityRole
      {
        Id = "1234Admin",
        Name = "Admin"
      };

      var resource = "resource";
      var action = "action";
      var scopedClaim = $"{resource}:{action}";

      var roleClaims = new List<Claim>
      {
        new Claim("scope",scopedClaim)
      };

      A.CallTo(() => _roleManager.FindByNameAsync(roles[0])).Returns(identityRole);
      A.CallTo(() => _roleManager.GetClaimsAsync(identityRole)).Returns(roleClaims);


      var handler = new GetRoleClaimsQueryHandler(_roleManager, _jwtFactory);

      // Act
      var result = await handler.GetItemsAsync(roles);

      // Assert
      Assert.IsType<HashSet<String>>(result);
      Assert.Single(result);
      Assert.Contains(scopedClaim, result);
    }

  }
}
