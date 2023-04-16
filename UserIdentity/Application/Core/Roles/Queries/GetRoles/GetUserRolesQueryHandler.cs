using Microsoft.AspNetCore.Identity;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Queries.GetRoles
{
    public record GetUserRolesQuery : BaseQuery
	{
		public String UserId { get; init; }
	}

	public class GetUserRolesQueryHandler : IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<IdentityUser> _userManager;

		public GetUserRolesQueryHandler(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public async Task<UserRolesViewModel> GetItemsAsync(GetUserRolesQuery query)
		{

			var user = await _userManager.FindByIdAsync(query.UserId);

			if (user == null)
				throw new NoRecordException(query.UserId, "User");

			var userRoles = await _userManager.GetRolesAsync(user);

			if (userRoles == null)
				userRoles = new List<String>();

			return new UserRolesViewModel
			{
				UserRoles = userRoles
			};

		}
	}
}
