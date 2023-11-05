using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Application.Core.Roles.Queries.GetRoles
{
  public record GetRolesQuery : BaseQuery
  {
  }

  public class GetRolesQueryHandler : IGetItemsQueryHandler<GetRolesQuery, RolesViewModel>
  {
    private readonly RoleManager<IdentityRole> _roleManager;

    public GetRolesQueryHandler(RoleManager<IdentityRole> roleManager)
    {
      _roleManager = roleManager;
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
