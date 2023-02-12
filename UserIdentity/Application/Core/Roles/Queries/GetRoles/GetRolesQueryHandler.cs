using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Queries.GetRoles
{
	public record GetRolesQuery : BaseQuery
	{
	}

	public class GetRolesQueryHandler : IGetItemsQueryHandler<GetRolesQuery, RolesViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;

		public GetRolesQueryHandler(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public async Task<RolesViewModel> GetItemsAsync(GetRolesQuery query)
		{
			var roles = await _roleManager
			.Roles
			.Select(r => new RoleDTO { Id = r.Id, Name = r.Name })
			.ToAsyncEnumerable()
			.ToListAsync();

			return new RolesViewModel
			{
				Roles = roles
			};
		}
	}
}
