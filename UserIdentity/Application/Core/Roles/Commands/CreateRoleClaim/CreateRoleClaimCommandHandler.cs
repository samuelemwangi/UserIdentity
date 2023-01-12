using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Application.Core.Roles.Commands.CreateRoleClaim
{
	public record CreateRoleClaimCommand : BaseCommand
	{
		[Required]
		public String RoleId { get; init; }

		[Required]
		public String Resource { get; init; }

		[Required]
		public String Action { get; set; }
	}

	public class CreateRoleClaimCommandHandler
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IJwtFactory _jwtFactory;

		public CreateRoleClaimCommandHandler(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IJwtFactory jwtFactory)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_jwtFactory = jwtFactory;
		}


		public async Task<RoleClaimViewModel> CreateRoleClaimAsync(CreateRoleClaimCommand command)
		{
			// Confirm role exists
			var role = await _roleManager.FindByIdAsync(command.RoleId);

			if (role == null)
				throw new NoRecordException(command.RoleId, "Role");

			// Confirm claim does not exist 
			var scopeClaim = _jwtFactory.GenerateScopeClaim(command.Resource, command.Action);

			var roleClaims = await _roleManager.GetClaimsAsync(role);

			foreach (var roleClaim in roleClaims)
				if (roleClaim.Value == scopeClaim.Value)
					throw new RecordExistsException(scopeClaim.Value, "Role Claim");

			var roleClaimCreateResults = await _roleManager.AddClaimAsync(role, scopeClaim);

			if (!roleClaimCreateResults.Succeeded)
				throw new RecordCreationException(command.ToString(), "RoleClaim");

			return new RoleClaimViewModel
			{
				RoleClaim = new RoleClaimDTO
				{
					Resource = command.Resource,
					Action = command.Action,
					Scope = scopeClaim.Value,
				}

			};

		}

	}
}
