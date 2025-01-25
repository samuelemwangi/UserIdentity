using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.Jwt;

using UserIdentity.Application.Core.Roles.Commands.DeleteRoleClaim;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Commands
{
	public class DeleteRoleClaimCommandHandlerTests
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtTokenHandler _jwtTokenHandler;
		public DeleteRoleClaimCommandHandlerTests()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
			_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
		}

		[Fact]
		public async Task DeleteRoleClaim_When_Non_Existent_Role_Throws_NoRecordException()
		{
			// Arrange
			var command = new DeleteRoleClaimCommand
			{
				RoleId = "SampleInvalidId",
				Resource = "SampleResource",
				Action = "SampleAction"
			};

			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(default(IdentityRole));

			var handler = new DeleteRoleClaimCommandHandler(_roleManager, _jwtTokenHandler);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.DeleteItemAsync(command, TestStringHelper.UserId));
		}

		[Fact]
		public async Task DeleteRoleClaim_When_Non_Existent_RoleClaim_Throws_NoRecordException()
		{
			// Arrange
			var command = new DeleteRoleClaimCommand
			{
				RoleId = "SampleValidId",
				Resource = "SampleResource",
				Action = "SampleAction"
			};

			var role = new IdentityRole
			{
				Id = command.RoleId,
				Name = "SampleRole"
			};

			var scopeClaim = new Claim("scope", $"{command.Resource}:{command.Action}");
			var roleClaims = new List<Claim>{
								new("scope", "SampleResource:SampleAction1"),
								new("scope", "SampleResource:SampleAction2")
						};


			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(role);
			A.CallTo(() => _jwtTokenHandler.GenerateScopeClaim(command.Resource, command.Action)).Returns(scopeClaim);

			A.CallTo(() => _roleManager.GetClaimsAsync(role)).Returns(roleClaims);

			var handler = new DeleteRoleClaimCommandHandler(_roleManager, _jwtTokenHandler);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.DeleteItemAsync(command, TestStringHelper.UserId));
		}

		[Fact]
		public async Task DeleteRoleClaim_Failure_When_Deleting_RoleClaim_Throws_RecordDeletionException()
		{
			// Arrange
			var command = new DeleteRoleClaimCommand
			{
				RoleId = "SampleValidId",
				Resource = "SampleResource",
				Action = "SampleAction"
			};

			var role = new IdentityRole
			{
				Id = command.RoleId,
				Name = "SampleRole"
			};

			var scopeClaim = new Claim("scope", $"{command.Resource}:{command.Action}");
			var roleClaims = new List<Claim>{
								scopeClaim,
								new("scope", "SampleResource:SampleAction2")
						};


			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(role);
			A.CallTo(() => _jwtTokenHandler.GenerateScopeClaim(command.Resource, command.Action)).Returns(scopeClaim);

			A.CallTo(() => _roleManager.GetClaimsAsync(role)).Returns(roleClaims);

			A.CallTo(() => _roleManager.RemoveClaimAsync(role, scopeClaim)).Returns(IdentityResult.Failed());

			var handler = new DeleteRoleClaimCommandHandler(_roleManager, _jwtTokenHandler);

			// Act & Assert
			await Assert.ThrowsAsync<RecordDeletionException>(() => handler.DeleteItemAsync(command, TestStringHelper.UserId));
		}

		[Fact]
		public async Task DeleteRoleClaim_Failure_When_Existing_RoleClaim_Deletes_RoleClaim()
		{
			// Arrange
			var command = new DeleteRoleClaimCommand
			{
				RoleId = "SampleValidId",
				Resource = "SampleResource",
				Action = "SampleAction"
			};

			var role = new IdentityRole
			{
				Id = command.RoleId,
				Name = "SampleRole"
			};

			var scopeClaim = new Claim("scope", $"{command.Resource}:{command.Action}");
			var roleClaims = new List<Claim>{
								scopeClaim,
								new Claim("scope", "SampleResource:SampleAction2")
						};


			A.CallTo(() => _roleManager.FindByIdAsync(command.RoleId)).Returns(role);
			A.CallTo(() => _jwtTokenHandler.GenerateScopeClaim(command.Resource, command.Action)).Returns(scopeClaim);

			A.CallTo(() => _roleManager.GetClaimsAsync(role)).Returns(roleClaims);

			A.CallTo(() => _roleManager.RemoveClaimAsync(role, scopeClaim)).Returns(IdentityResult.Success);

			var handler = new DeleteRoleClaimCommandHandler(_roleManager, _jwtTokenHandler);

			// Act 
			var vm = await handler.DeleteItemAsync(command, TestStringHelper.UserId);

			// Assert
			Assert.IsType<DeleteRecordViewModel>(vm);

		}
	}
}
