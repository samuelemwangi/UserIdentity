using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Common.Exceptions;

using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Application.Core.Roles.Commands.UpdateRole
{

	public record UpdateRoleCommand : BaseCommand
	{
		[Required]
		public required string RoleId { get; set; }

		[Required]
		public required string RoleName { get; init; }
	}
	public class UpdateRoleCommandHandler(
		RoleManager<IdentityRole> roleManager
		) : IUpdateItemCommandHandler<UpdateRoleCommand, RoleViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;

		public async Task<RoleViewModel> UpdateItemAsync(UpdateRoleCommand command, string userId)
		{
			var role = await _roleManager.FindByIdAsync(command.RoleId) ?? throw new NoRecordException(command.RoleId, "Role");

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
