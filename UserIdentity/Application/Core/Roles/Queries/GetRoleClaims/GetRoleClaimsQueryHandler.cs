﻿using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;

namespace UserIdentity.Application.Core.Roles.Queries.GetRoleClaims
{
	public record GetRoleClaimsQuery : BaseQuery
	{
		public string RoleId { get; init; }
	}
	public class GetRoleClaimsQueryHandler : IGetItemsQueryHandler<GetRoleClaimsQuery, RoleClaimsViewModel>,
		IGetItemsQueryHandler<IList<string>, HashSet<string>>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtFactory _jwtFactory;

		public GetRoleClaimsQueryHandler(RoleManager<IdentityRole> roleManager, IJwtFactory jwtFactory)
		{
			_roleManager = roleManager;
			_jwtFactory = jwtFactory;
		}


		public async Task<RoleClaimsViewModel> GetItemsAsync(GetRoleClaimsQuery query)
		{
			var role = await _roleManager.FindByIdAsync(query.RoleId);

			if (role == null)
				throw new NoRecordException(query.RoleId, "Role");

			var roleClaims = await _roleManager.GetClaimsAsync(role);

			var rolesCollection = new List<RoleClaimDTO>();

			foreach (var roleClaim in roleClaims)
			{
				(string resource, string action) = _jwtFactory.DecodeScopeClaim(roleClaim);
				RoleClaimDTO claimDTO = new()
				{
					Resource = resource,
					Action = action,
					Scope = roleClaim.Value
				};

				rolesCollection.Add(claimDTO);
			}

			return new RoleClaimsViewModel
			{
				RoleClaims = rolesCollection
			};

		}

		public async Task<HashSet<string>> GetItemsAsync(IList<string> roles)
		{
			HashSet<string> roleClaims = new();

			foreach (var role in roles)
			{

				var currentRole = await _roleManager.FindByNameAsync(role);
				var currentRoleClaims = await _roleManager.GetClaimsAsync(currentRole);


				foreach (var claim in currentRoleClaims)
				{
					roleClaims.Add(claim.Value);
				}

			}

			return roleClaims;

		}
	}
}
