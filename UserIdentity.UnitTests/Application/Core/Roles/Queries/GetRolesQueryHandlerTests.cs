using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Queries
{
	public class GetRolesQueryHandlerTests
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;


		public GetRolesQueryHandlerTests()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
			_userManager = A.Fake<UserManager<IdentityUser>>();
		}

		[Fact]
		public async Task Get_Roles_Returns_Roles()
		{
			// Arrange
			var roles = new List<IdentityRole> {
														new IdentityRole { Id = "1", Name = "Admin" },
														new IdentityRole { Id = "2", Name = "User" }
									};


			A.CallTo(() => _roleManager.Roles).Returns(roles.AsQueryable());

			var handler = new GetRolesQueryHandler(_roleManager, _userManager);

			// Act
			var vm = await handler.GetItemsAsync(new GetRolesQuery { });

			// Assert
			Assert.IsType<RolesViewModel>(vm);
			Assert.Equal(roles.Count, vm.Roles.Count);
			Assert.Equal(roles.Select(r => r.Id), vm.Roles.Select(r => r.Id));
			Assert.Equal(roles.Select(r => r.Name), vm.Roles.Select(r => r.Name));
		}

		[Fact]
		public async Task Get_Roles__When_No_Roles_Returns_Zero_Roles()
		{
			// Arrange
			var roles = new List<IdentityRole>();
			A.CallTo(() => _roleManager.Roles).Returns(roles.AsQueryable());

			var handler = new GetRolesQueryHandler(_roleManager, _userManager);

			// Act
			var vm = await handler.GetItemsAsync(new GetRolesQuery { });

			// Assert
			Assert.IsType<RolesViewModel>(vm);
			Assert.Equal(0, vm.Roles.Count);
		}
	}
}
