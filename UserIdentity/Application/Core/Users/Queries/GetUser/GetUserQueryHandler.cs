using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Domain.DTO;

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
	IMachineDateTime machineDateTime
	) : IGetItemQueryHandler<GetUserQuery, UserViewModel>
{

	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IMachineDateTime _machineDateTime = machineDateTime;

	public async Task<UserViewModel> GetItemAsync(GetUserQuery query)
	{
		var user = await _userManager.FindByIdAsync(query.UserId) ?? throw new NoRecordException(query.UserId + "", "User");

		var userDetails = await _userRepository.GetUserAsync(query.UserId) ?? throw new NoRecordException(query.UserId + "", "User");

		var userDTO = new UserDTO
		{
			Id = user.Id,
			UserName = user != null ? user.UserName : "",
			FullName = (userDetails.FirstName + " " + userDetails.LastName).Trim(),
			Email = user != null ? user.Email : "",
		};

		userDTO.SetDTOAuditFields(userDetails);

		return new UserViewModel
		{
			User = userDTO

		};
	}

}
