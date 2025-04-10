using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Domain.DTO;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Queries.GetUser;

public record GetUserQuery : IBaseQuery
{
	public required string UserId { get; init; }
}

public class GetUserQueryHandler(
	UserManager<IdentityUser> userManager,
	IUserRepository userRepository,
	IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> getRoleClaimsQueryHandler
	) : IGetItemQueryHandler<GetUserQuery, UserViewModel>
{

	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;

	public async Task<UserViewModel> GetItemAsync(GetUserQuery query)
	{
		var user = await _userManager.FindByIdAsync(query.UserId) ?? throw new NoRecordException($"{query.UserId}", "User");

		var userDetails = await _userRepository.GetUserAsync(query.UserId) ?? throw new NoRecordException($"{query.UserId}", "User");

		var userRoles = await _userManager.GetRolesAsync(user);

		var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsForRolesQuery { Roles = userRoles });

		var userDTO = new UserDTO
		{
			Id = user.Id,
			UserName = user != null ? user.UserName : "",
			FirstName = userDetails.FirstName,
			LastName = userDetails.LastName,
			Email = user != null ? user.Email : "",
			PhoneNumber = user != null ? user.PhoneNumber : "",
			Roles = [.. userRoles],
			RoleClaims = userRoleClaims.RoleClaims
		};

		userDTO.SetDTOAuditFields(userDetails);

		return new UserViewModel
		{
			User = userDTO
		};
	}
}
