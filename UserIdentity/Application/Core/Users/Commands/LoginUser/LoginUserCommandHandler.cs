using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

using Microsoft.AspNetCore.Identity;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.LoginUser
{
    public record LoginUserCommand : BaseCommand
	{
		[Required]
		public String? UserName { get; init; }

		[Required]
		public String? Password { get; init; }
	}

	public class LoginUserCommandHandler : ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel>
	{
		private readonly IJwtFactory _jwtFactory;
		private readonly ITokenFactory _tokenFactory;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserRepository _userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IMachineDateTime _machineDateTime;

		private readonly IGetItemsQueryHandler<IList<String>, HashSet<String>> _getRoleClaimsQueryHandler;


		public LoginUserCommandHandler(
			IJwtFactory jwtFactory,
			ITokenFactory tokenFactory,
			UserManager<IdentityUser> userManager,
			IUserRepository userRepository,
			IRefreshTokenRepository refreshTokenRepository,
			IMachineDateTime machineDateTime,

			IGetItemsQueryHandler<IList<String>, HashSet<String>> getRoleClaimsQueryHandler
			)
		{
			_jwtFactory = jwtFactory;
			_tokenFactory = tokenFactory;
			_userManager = userManager;
			_userRepository = userRepository;
			_refreshTokenRepository = refreshTokenRepository;
			_machineDateTime = machineDateTime;

			_getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;
		}

		public async Task<AuthUserViewModel> CreateItemAsync(LoginUserCommand command)
		{
			var user = await _userManager.FindByNameAsync(command.UserName);

			if (user == null)
				user = await _userManager.FindByEmailAsync(command.UserName);

			if (user == null)
				throw new InvalidCredentialException("No Identity User for User Name - " + command.UserName);


			var appUserDetails = await _userRepository.GetUserAsync(user.Id);

			if (appUserDetails == null)
				throw new InvalidCredentialException("No User details for User Name - " + command.UserName);


			var userExists = await _userManager.CheckPasswordAsync(user, command.Password);

			if (!userExists)
				throw new InvalidCredentialException("Invalid Credentials Provided for - " + command.UserName);

			var userRoles = await _userManager.GetRolesAsync(user);

			var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(userRoles);

			var refreshToken = _tokenFactory.GenerateRefreshToken();

			(String token, Int32 expiresIn) = await _jwtFactory.GenerateEncodedTokenAsync(user.Id, user.UserName, userRoles, userRoleClaims);

			var newRefreshToken = new RefreshToken
			{
				CreatedDate = _machineDateTime.Now,
				Expires = _machineDateTime.Now.AddSeconds((long)expiresIn),
				UserId = user.Id,
				Token = refreshToken,
			};

			Int32 createTokenResult = await _refreshTokenRepository.CreateRefreshTokenAsync(newRefreshToken);

			if (createTokenResult < 1)
				throw new RecordCreationException(refreshToken, "Refresh Token");

			return new AuthUserViewModel
			{
				UserDetails = new UserDTO
				{
					Id = user.Id,
					UserName = user.UserName,
					FullName = appUserDetails.FirstName + " " + appUserDetails.LastName,
					Email = user.Email,
					CreatedBy = appUserDetails.CreatedBy,
					CreatedDate = _machineDateTime.ResolveDate(appUserDetails.CreatedDate),
					LastModifiedBy = user.Id,
					LastModifiedDate = _machineDateTime.ResolveDate(appUserDetails.LastModifiedDate),
				},
				UserToken = new AccessTokenViewModel
				{
					AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
					RefreshToken = refreshToken,
				}

			};
		}
	}
}
