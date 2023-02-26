using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Roles.Commands.DeleteRole;
using UserIdentity.Application.Exceptions;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Commands
{
	public class DeleteRoleCommandHandlerTests
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public DeleteRoleCommandHandlerTests()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
		}

		[Fact]
		public async Task DeleteRole_When_Non_Existent_Role_Throws_NoRecordException()
		{
			// Arrange
			var command = new DeleteRoleCommand
			{
				RoleId = "SampleInvalidId"
			};

			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(default(IdentityRole));

			var handler = new DeleteRoleCommandHandler(_roleManager);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.DeleteItemAsync(command));
		}

		[Fact]
		public async Task DeleteRole_Failure_When_Deleting_Role_Throws_RecordDeletionException()
		{
			// Arrange
			var command = new DeleteRoleCommand
			{
				RoleId = "SampleValidId"
			};

			var role = new IdentityRole
			{
				Id = command.RoleId,
				Name = "SampleRole"
			};

			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(role);
			A.CallTo(() => _roleManager.DeleteAsync(role)).Returns(IdentityResult.Failed());

			var handler = new DeleteRoleCommandHandler(_roleManager);

			// Act & Assert
			await Assert.ThrowsAsync<RecordDeletionException>(() => handler.DeleteItemAsync(command));
		}

		[Fact]
		public async Task DeleteRole_When_Existing_Role_Deletes_Role()
		{
			// Arrange
			var command = new DeleteRoleCommand
			{
				RoleId = "SampleValidId"
			};

			var role = new IdentityRole
			{
				Id = command.RoleId,
				Name = "SampleRole"
			};

			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(role);
			A.CallTo(() => _roleManager.DeleteAsync(role)).Returns(IdentityResult.Success);

			var handler = new DeleteRoleCommandHandler(_roleManager);

			// Act
			var vm = await handler.DeleteItemAsync(command);

			// Assert
			Assert.IsType<DeleteRecordViewModel>(vm);
		}
	}
}
