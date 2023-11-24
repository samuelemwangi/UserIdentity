using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Commands.DeleteRole
{

	public record DeleteRoleCommand : BaseCommand
	{
		public String RoleId { get; init; }
	}
	public class DeleteRoleCommandHandler : IDeleteItemCommandHandler<DeleteRoleCommand, DeleteRecordViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public DeleteRoleCommandHandler(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}


		public async Task<DeleteRecordViewModel> DeleteItemAsync(DeleteRoleCommand command)
		{
			var role = await _roleManager.FindByIdAsync(command.RoleId);

			if (role == null)
				throw new NoRecordException(command.RoleId, "Role");

			var deleteRoleResult = await _roleManager.DeleteAsync(role);


			if (!deleteRoleResult.Succeeded)
				throw new RecordDeletionException(command.RoleId, "Role");

			return new DeleteRecordViewModel
			{

			};

		}

	}
}
