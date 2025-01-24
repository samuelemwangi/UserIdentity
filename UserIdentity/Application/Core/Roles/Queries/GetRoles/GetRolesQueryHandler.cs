using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;

using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Application.Core.Roles.Queries.GetRoles
{
	public record GetRolesQuery : BaseQuery
	{
	}

	public class GetRolesQueryHandler(
		RoleManager<IdentityRole> roleManager
		) : IGetItemsQueryHandler<GetRolesQuery, RolesViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;

		public async Task<RolesViewModel> GetItemsAsync(GetRolesQuery query)
		{
			var roles = await _roleManager
			.Roles
			.Select(r => new RoleDTO { Id = r.Id, Name = r.Name! })
			.ToListAsync();

			return new RolesViewModel
			{
				Roles = roles
			};
		}
	}
}
