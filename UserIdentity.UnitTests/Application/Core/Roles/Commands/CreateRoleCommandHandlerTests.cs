using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Roles.Commands.CreateRole;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Commands
{
	public class CreateRoleCommandHandlerTests
	{

		private readonly RoleManager<IdentityRole> _roleManager;


		public CreateRoleCommandHandlerTests()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
		}
		[Fact]
		public async Task Create_Role_When_Role_Exists_Throws_RecordExistsException()
		{
			// Arrange
			var command = new CreateRoleCommand { RoleName = "Admin" };

			A.CallTo(() => _roleManager.FindByNameAsync(command.RoleName)).Returns(new IdentityRole { Id = "1", Name = command.RoleName });

			var handler = new CreateRoleCommandHandler(_roleManager);

			// Act & Assert
			await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_Role_When_Role_Creation_Fails_Throws_RecordCreationException()
		{
			// Arrange
			var command = new CreateRoleCommand { RoleName = "Admin" };


			A.CallTo(() => _roleManager.FindByNameAsync(command.RoleName)).Returns(default(IdentityRole));
			A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.That.Matches(r => r.Name == command.RoleName))).Returns(Task.FromResult(IdentityResult.Failed()));

			var handler = new CreateRoleCommandHandler(_roleManager);

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_Role_Returns_Role()
		{
			// Arrange
			var command = new CreateRoleCommand { RoleName = "Admin" };

			A.CallTo(() => _roleManager.FindByNameAsync(command.RoleName)).Returns(default(IdentityRole));
			A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.That.Matches(r => r.Name == command.RoleName))).Returns(Task.FromResult(IdentityResult.Success));

			var handler = new CreateRoleCommandHandler(_roleManager);

			// Act
			var vm = await handler.CreateItemAsync(command);

			// Assert
			Assert.IsType<RoleViewModel>(vm);
			Assert.NotNull(vm.Role.Id);
			Assert.Equal(command.RoleName, vm.Role.Name);
		}

	}
}