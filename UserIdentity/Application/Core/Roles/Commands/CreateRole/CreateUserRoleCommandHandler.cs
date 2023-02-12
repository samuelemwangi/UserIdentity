using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Commands.CreateRole
{
	public record CreateUserRoleCommand : BaseCommand
	{
		public String UserId { get; init; }
		public String RoleId { get; init; }
	}

	public class CreateUserRoleCommandHandler : ICreateItemCommandHandler<CreateUserRoleCommand, UserRolesViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> _getUserRolesQueryHandler;

		public CreateUserRoleCommandHandler(
			RoleManager<IdentityRole> roleManager,
			UserManager<IdentityUser> userManager,
			IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> getUserRolesQueryHandler
			)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_getUserRolesQueryHandler = getUserRolesQueryHandler;

		}


		public async Task<UserRolesViewModel> CreateItemAsync(CreateUserRoleCommand command)
		{
			var user = await _userManager.FindByIdAsync(command.UserId);

			if (user == null)
				throw new NoRecordException(command.UserId + "", "User");

			var role = await _roleManager.FindByIdAsync(command.RoleId);

			if (role == null)
				throw new NoRecordException(command.RoleId + "", "Role");

			var userRoleExists = await _userManager.IsInRoleAsync(user, role.Name);

			if (userRoleExists)
				throw new RecordExistsException(command.RoleId, "UserRole");

			var resultUserRole = await _userManager.AddToRoleAsync(user, role.Name);

			if (!resultUserRole.Succeeded)
				throw new RecordCreationException(command.RoleId, "UserRole");

			// remove roles from cache
			var resolvedRoles = await _getUserRolesQueryHandler.GetItemsAsync(new GetUserRolesQuery { UserId = command.UserId });

			return resolvedRoles;
		}
	}
}
