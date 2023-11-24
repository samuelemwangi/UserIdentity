using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.Commands.CreateRole;
using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Commands
{
	public class CreateUserRoleCommandHandlerTests
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> _getUserRolesQueryHandler;


		public CreateUserRoleCommandHandlerTests()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
			_userManager = A.Fake<UserManager<IdentityUser>>();
			_getUserRolesQueryHandler = A.Fake<IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel>>();
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
		public async Task Create_UserRole_When_User_Role_Exists_Throws_RecordExistsException()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			var existingRole = new IdentityRole { Id = command.RoleId, Name = "Admin" };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(existingRole);
			A.CallTo(() => _userManager.IsInRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), existingRole.Name)).Returns(Task.FromResult(true));

			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_When_UserRole_Creation_Fails_Throws_RecordCreationException()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			var existingRole = new IdentityRole { Id = command.RoleId, Name = "Admin" };
			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(existingRole);
			A.CallTo(() => _userManager.IsInRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), existingRole.Name)).Returns(Task.FromResult(false));
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), existingRole.Name)).Returns(Task.FromResult(IdentityResult.Failed()));

			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_UserRole_Returns_UserRoles()
		{
			// Arrange
			var command = new CreateUserRoleCommand { UserId = "123", RoleId = "Admin123" };
			var existingRole = new IdentityRole { Id = command.RoleId, Name = "Admin" };

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(new IdentityUser { Id = command.UserId });
			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(existingRole);
			A.CallTo(() => _userManager.IsInRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), existingRole.Name)).Returns(Task.FromResult(false));
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(u => u.Id == command.UserId), existingRole.Name)).Returns(Task.FromResult(IdentityResult.Success));
			A.CallTo(() => _getUserRolesQueryHandler.GetItemsAsync(A<GetUserRolesQuery>.That.Matches(q => q.UserId == command.UserId))).Returns(new UserRolesViewModel { UserRoles = new List<String> { "Admin" } });

			var handler = new CreateUserRoleCommandHandler(_roleManager, _userManager, _getUserRolesQueryHandler);

			// Act
			var result = await handler.CreateItemAsync(command);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.UserRoles.Count);
		}
	}
}
