﻿using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Application.Core.Roles.Commands.DeleteRoleClaim
{
	public record DeleteRoleClaimCommand : BaseCommand
	{
		[Required]
		public string RoleId { get; init; }
		[Required]
		public string Resource { get; init; }
		[Required]
		public string Action { get; init; }
	}
	public class DeleteRoleClaimCommandHandler : IDeleteItemCommandHandler<DeleteRoleClaimCommand, DeleteRecordViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtFactory _jwtFactory;

		public DeleteRoleClaimCommandHandler(RoleManager<IdentityRole> roleManager, IJwtFactory jwtFactory)
		{
			_roleManager = roleManager;
			_jwtFactory = jwtFactory;
		}

		public async Task<DeleteRecordViewModel> DeleteItemAsync(DeleteRoleClaimCommand command)
		{
			// Confirm role exists
			var role = await _roleManager.FindByIdAsync(command.RoleId);

			if (role == null)
				throw new NoRecordException(command.RoleId, "Role");

			// Confirm claim does not exist 
			var scopeClaim = _jwtFactory.GenerateScopeClaim(command.Resource, command.Action);
			var roleClaims = await _roleManager.GetClaimsAsync(role);

			bool claimFound = false;

			foreach (var roleClaim in roleClaims)
			{
				if (string.Equals(roleClaim.Value, scopeClaim.Value, StringComparison.OrdinalIgnoreCase))
				{
					claimFound = true;
					break;
				}
			}

			if (!claimFound)
				throw new NoRecordException($"{command.Resource}:{command.Action}", "RoleClaim");

			// Delete Claim
			var deleteRoleClaimResults = await _roleManager.RemoveClaimAsync(role, scopeClaim);

			if (!deleteRoleClaimResults.Succeeded)
				throw new RecordDeletionException($"{command.Resource}:{command.Action}", "RoleClaim");

			return new DeleteRecordViewModel
			{

			};
		}
	}
}
