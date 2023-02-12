using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Roles.Commands.CreateRole;
using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Commands
{
	public class CreateUserRoleCommandHandlerTest
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly GetUserRolesQueryHandler _getUserRolesQueryHandler;


		public CreateUserRoleCommandHandlerTest()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
			_userManager = A.Fake<UserManager<IdentityUser>>();
			_getUserRolesQueryHandler = A.Fake<GetUserRolesQueryHandler>();
		}

		[Fact]
		public async Task Create_UserRole_When_No_User_Exists_Throws_NoRecordException()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(default(IdentityUser));

			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_When_No_Role_Exists_Throws_NoRecordException()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(default(IdentityRole));

			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_When_UserRole_Creation_Fails_Throws_RecordCreationException()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(new IdentityRole { Id = command.RoleId });
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), A<string>.That.Matches(r => r == command.RoleId))).Returns(Task.FromResult(IdentityResult.Failed()));

			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_Returns_UserRoles()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			var query = new GetUserRolesQuery { UserId = command.UserId };
			var userRolesVM = new UserRolesViewModel { UserRoles = new List<String> { "Admin" } };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(new IdentityRole { Id = command.RoleId });
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), A<string>.That.Matches(r => r == command.RoleId))).Returns(Task.FromResult(IdentityResult.Success));
			A.CallTo(() => _getUserRolesQueryHandler.GetItemsAsync(query)).Returns(Task.FromResult(userRolesVM));


			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act
			var vm = await handler.CreateItemAsync(command);

			// Assert
			Assert.IsType<UserRolesViewModel>(vm);
		}
	}
}
