using Microsoft.AspNetCore.Identity;
using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using Xunit;
using FakeItEasy;
using System.Threading.Tasks;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using System.Collections.Generic;
using System;

namespace UserIdentity.Application.Core.Roles.Commands.CreateRole.Application.Core.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandlerTest
    {
        // Tests for CreateRoleCommandHandler in UserIdentity.Application.Core.Roles.Commands.CreateRole

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GetRolesQueryHandler _getUserRolesQueryHandler;


        public CreateRoleCommandHandlerTest()
        {
            _roleManager = A.Fake<RoleManager<IdentityRole>>();
            _userManager = A.Fake<UserManager<IdentityUser>>();
            _getUserRolesQueryHandler = A.Fake<GetRolesQueryHandler>();
        }
		 [Fact]
        public async Task Create_Role_When_Role_Exists_Throws_RecordExistsException()
        {
            // Arrange
            var command = new CreateRoleCommand { RoleName = "Admin" };

            A.CallTo(() => _roleManager.FindByNameAsync(command.RoleName)).Returns(new IdentityRole { Id = "1", Name = command.RoleName });

            var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

            // Act & Assert
            await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateRoleAsync(command));
        }

        [Fact]
        public async Task Create_Role_When_Role_Creation_Fails_Throws_RecordCreationException()
        {
            // Arrange
            var command = new CreateRoleCommand { RoleName = "Admin" };


            A.CallTo(() => _roleManager.FindByNameAsync(command.RoleName)).Returns(default(IdentityRole));
            A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.That.Matches(r => r.Name == command.RoleName))).Returns(Task.FromResult(IdentityResult.Failed()));

            var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

            // Act & Assert
            await Assert.ThrowsAsync<RecordCreationException>(()=>handler.CreateRoleAsync(command));
        }

        [Fact]
        public async Task Create_Role_Returns_Role()
        {
            // Arrange
            var command = new CreateRoleCommand { RoleName = "Admin" };

            A.CallTo(() => _roleManager.FindByNameAsync(command.RoleName)).Returns(default(IdentityRole));
            A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.That.Matches(r => r.Name == command.RoleName))).Returns(Task.FromResult(IdentityResult.Success));

            var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

            // Act
            var vm = await handler.CreateRoleAsync(command);

            // Assert
            Assert.IsType<RoleViewModel>(vm);
            Assert.NotNull(vm.Role.Id);
            Assert.Equal(command.RoleName, vm.Role.Name);
        }

       


		[Fact]
		public async Task Create_UserRole_When_No_User_Exists_Throws_NoRecordException(){
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(default(IdentityUser));

			var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.CreateUserRoleAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_When_No_Role_Exists_Throws_NoRecordException(){
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(default(IdentityRole));

			var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.CreateUserRoleAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_When_UserRole_Creation_Fails_Throws_RecordCreationException(){
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(new IdentityRole { Id = command.RoleId });
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), A<string>.That.Matches(r => r == command.RoleId))).Returns(Task.FromResult(IdentityResult.Failed()));

			var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateUserRoleAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_Returns_UserRoles(){
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			var query = new GetRolesQuery { UserId = command.UserId };
			var userRolesVM = new UserRolesViewModel{ UserRoles = new List<String>{ "Admin" }};

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(new IdentityRole { Id = command.RoleId });
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), A<string>.That.Matches(r => r == command.RoleId))).Returns(Task.FromResult(IdentityResult.Success));
			A.CallTo(() => _getUserRolesQueryHandler.GetUserRolesAsync(query)).Returns(Task.FromResult(userRolesVM));


			var handler = new CreateRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act
			var vm = await handler.CreateUserRoleAsync(command);

			// Assert
			Assert.IsType<UserRolesViewModel>(vm);
		}
    }
}