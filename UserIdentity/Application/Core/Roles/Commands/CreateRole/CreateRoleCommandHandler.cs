using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Commands.CreateRole
{
	public record CreateRoleCommand : BaseCommand
	{
		[Required]
		public string RoleName { get; init; }
	}



	public class CreateRoleCommandHandler : ICreateItemCommandHandler<CreateRoleCommand, RoleViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<RoleViewModel> CreateItemAsync(CreateRoleCommand command)
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
				Role = new RoleDTO
				{
					Id = newRole.Id,
					Name = newRole.Name
				}

			};
		}

	}
}
