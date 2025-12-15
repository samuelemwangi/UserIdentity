using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Common.Exceptions;

using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Application.Core.Roles.Queries;

public record GetRoleQuery : IBaseQuery
{
    public required string RoleId { get; init; }
}

public class GetRoleQueryHandler(
    RoleManager<IdentityRole> roleManager
    ) : IGetItemQueryHandler<GetRoleQuery, RoleViewModel>
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<RoleViewModel> GetItemAsync(GetRoleQuery query)
    {

        var role = await _roleManager.FindByIdAsync(query.RoleId) ?? throw new NoRecordException(query.RoleId, "Role");

        return new RoleViewModel
        {

            Role = new RoleDTO
            {
                Id = query.RoleId,
                Name = role.Name!
            }
        };
    }
}
