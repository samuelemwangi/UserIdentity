using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Queries.GetRoles
{
  public record GetRolesQuery : BaseQuery
    {
        public String UserId { get; init; }
    }

    public class GetRolesQueryHandler
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public GetRolesQueryHandler(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<RolesViewModel> GetRolesAsync()
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

        public async Task<UserRolesViewModel> GetUserRolesAsync(GetRolesQuery query)
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
