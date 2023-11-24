using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Commands.UpdateRole
{

	public record UpdateRoleCommand : BaseCommand
	{
		[Required]
		public String RoleId { get; set; }

		[Required]
		public String RoleName { get; init; }
	}
	public class UpdateRoleCommandHandler : IUpdateItemCommandHandler<UpdateRoleCommand, RoleViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}


		public async Task<RoleViewModel> UpdateItemAsync(UpdateRoleCommand command)
		{
			var role = await _roleManager.FindByIdAsync(command.RoleId);

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
					Id = role.Id,
					Name = role.Name
				}
			};

		}

	}
}
