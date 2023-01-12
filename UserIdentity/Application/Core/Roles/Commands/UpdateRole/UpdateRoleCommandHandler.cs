using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Commands.UpdateRole
{

	public record UpdateRoleCommand : BaseCommand
	{
		public String RoleId { get; init; }
		public String RoleName { get; init; }
	}
	public class UpdateRoleCommandHandler
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}


		public async Task<RoleViewModel> UpdateRoleAsync(UpdateRoleCommand command)
		{
			var role = await _roleManager.Roles.Where(r => r.Id == command.RoleId).FirstOrDefaultAsync();

			if (role == null)
				throw new NoRecordException(command.RoleId, "Role");

			role.Name = command.RoleName;

			var updateRoleResult = await _roleManager.UpdateAsync(role);
		

			if (!updateRoleResult.Succeeded)
				throw new RecordUpdateException(command.RoleId, "Role");

			return new RoleViewModel
			{
				Role = new RoleDTO
				{
					Id =  role.Id,
					Name =  role.Name
				}
			};

		}

	}
}
