using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Queries.GetUser
{
	public record GetUserQuery : BaseQuery
	{
		public String UserId { get; init; }
	}

	public class GetUserQueryHandler : IGetItemQueryHandler<GetUserQuery, UserViewModel>
	{

		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserRepository _userRepository;
		private readonly IMachineDateTime _machineDateTime;


		public GetUserQueryHandler(UserManager<IdentityUser> userManager, IUserRepository userRepository, IMachineDateTime machineDateTime)
		{
			_userManager = userManager;
			_userRepository = userRepository;
			_machineDateTime = machineDateTime;

		}

		public async Task<UserViewModel> GetItemAsync(GetUserQuery query)
		{
			var user = await _userManager.FindByIdAsync(query.UserId);

			if (user == null)
				throw new NoRecordException(query.UserId + "", "User");

			var userDetails = await _userRepository.GetUserAsync(query.UserId);

			if (userDetails == null)
				throw new NoRecordException(query.UserId + "", "User");

			var vm = new UserViewModel
			{
				User = new UserDTO
				{
					Id = user.Id,
					UserName = user != null ? user.UserName : "",
					FullName = userDetails.FirstName + " " + userDetails.LastName,
					Email = user != null ? user.Email : "",
					CreatedBy = userDetails.CreatedBy,
					CreatedDate = _machineDateTime.ResolveDate(userDetails.CreatedDate),
					LastModifiedBy = userDetails.LastModifiedBy,
					LastModifiedDate = _machineDateTime.ResolveDate(userDetails.LastModifiedDate)
				}
			};

			return vm;
		}

	}

}
