using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Commands.CreateRole
{
	public record CreateRoleCommand : BaseCommand
	{
		public String RoleName { get; init; }
	}

	public record CreateUserRoleCommand : BaseCommand
	{
		public String UserId { get; init; }
		public String RoleId { get; init; }
	}

	public class CreateRoleCommandHandler
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly GetUserRolesQueryHandler _getUserRolesQueryHandler;
		public CreateRoleCommandHandler(
			RoleManager<IdentityRole> roleManager,
			UserManager<IdentityUser> userManager,
			GetUserRolesQueryHandler getUserRolesQueryHandler
			)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_getUserRolesQueryHandler = getUserRolesQueryHandler;

		}

		public async Task<RoleViewModel> CreateRoleAsync(CreateRoleCommand command)
		{
			var existingRole = await _roleManager.FindByNameAsync(command.RoleName);
			if (existingRole != null)
				throw new RecordExistsException(command.RoleName, "Role");

			var newRole = new IdentityRole { Name = command.RoleName };

			var createdRoleResult = await _roleManager.CreateAsync(newRole);
			if (!createdRoleResult.Succeeded)
				throw new RecordCreationException(command.RoleName, "Role");

			return new RoleViewModel
			{
				Role =  new RoleDTO
				{
					Id =  newRole.Id,
					Name = newRole.Name
				}

			};
		}

		public async Task<UserRolesViewModel> CreateUserRoleAsync(CreateUserRoleCommand command)
		{
			var user = await _userManager.FindByIdAsync(command.UserId);

			if (user == null)
				throw new NoRecordException(command.UserId + "", "User");

			var role = await _roleManager.FindByIdAsync(command.RoleId);

			if(role == null)
				throw new NoRecordException(command.RoleId + "", "Role");

			var userRoleExists = await _userManager.IsInRoleAsync(user, role.Name);

			if (userRoleExists)
				throw new RecordExistsException(command.RoleId  , "UserRole");

			var resultUserRole = await _userManager.AddToRoleAsync(user, role.Name);

			if (!resultUserRole.Succeeded)
				throw new RecordCreationException(command.RoleId, "UserRole");

			// remove roles from cache
			var resolvedRoles = await _getUserRolesQueryHandler.GetItemsAsync(new GetUserRolesQuery { UserId = command.UserId });

			return resolvedRoles;
		}
	}
}
