using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;

using Microsoft.AspNetCore.Identity;
using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Queries
{
    public class GetRolesQueryHandlerTest
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;


        public GetRolesQueryHandlerTest()
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
            var vm = await handler.GetRolesAsync();

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
			var vm = await handler.GetRolesAsync();

			// Assert
			Assert.IsType<RolesViewModel>(vm);
			Assert.Equal(0, vm.Roles.Count);
		}

        [Fact]
        public async Task Get_UserRoles_Returns_UserRoles()
        {
            // Arrange
            var query = new GetRolesQuery { UserId = "1" };
            var user = new IdentityUser { Id = "1", UserName = "test" };
            var userRoles = new List<string> { "Admin", "User" };

            A.CallTo(() => _userManager.FindByIdAsync(query.UserId)).Returns(user);
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(userRoles);

            var handler = new GetRolesQueryHandler(_roleManager, _userManager);

            // Act
            var vm = await handler.GetUserRolesAsync(query);

            // Assert
            Assert.IsType<UserRolesViewModel>(vm);
            Assert.Equal(userRoles.Count, vm.UserRoles.Count);
            Assert.Equal(userRoles, vm.UserRoles);

			foreach (var item in vm.UserRoles)			
				Assert.Contains(item, userRoles);
			
        }

        [Fact]
        public async Task Get_UserRoles_for_NonExisiting_UserRoles_Returns_Zero_UserRoles()
        {
            // Arrange
            var query = new GetRolesQuery { UserId = "1" };
            var user = new IdentityUser { Id = "1", UserName = "test" };
            var userRoles = default(List<string>);

            A.CallTo(() => _userManager.FindByIdAsync(query.UserId)).Returns(user);
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(userRoles);

            var handler = new GetRolesQueryHandler(_roleManager, _userManager);

            // Act
            var vm = await handler.GetUserRolesAsync(query);

            // Assert
            Assert.IsType<UserRolesViewModel>(vm);
            Assert.Equal(0, vm.UserRoles.Count);
			foreach (var item in vm.UserRoles)
				Assert.Contains(item, userRoles);
        }



        [Fact]
        public async Task Get_UserRoles_for_NonExisiting_User_Throws_NoRecordException()
        {
            // Arrange
            var query = new GetRolesQuery { UserId = "1" };
            var user = default(IdentityUser);

            A.CallTo(() => _userManager.FindByIdAsync(query.UserId)).Returns(user);

            var handler = new GetRolesQueryHandler(_roleManager, _userManager);

            // Act &  Assert
            await Assert.ThrowsAsync<NoRecordException>(() => handler.GetUserRolesAsync(query));
        }
    }
}
