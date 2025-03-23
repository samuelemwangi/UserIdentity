using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Domain.DTO;
using PolyzenKit.Domain.Entity;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.LoginUser;

public record LoginUserCommand : IBaseCommand
{
	[Required]
	public required string UserName { get; init; }

	[Required]
	public required string Password { get; init; }
}

public class LoginUserCommandHandler(
	IJwtTokenHandler jwtTokenHandler,
	ITokenFactory tokenFactory,
	UserManager<IdentityUser> userManager,
	IUserRepository userRepository,
	IRefreshTokenRepository refreshTokenRepository,
	IMachineDateTime machineDateTime,
	IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> getRoleClaimsQueryHandler
	) : ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel>
{
	private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
	private readonly ITokenFactory _tokenFactory = tokenFactory;
	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
	private readonly IMachineDateTime _machineDateTime = machineDateTime;

	private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;

	public async Task<AuthUserViewModel> CreateItemAsync(LoginUserCommand command, string userId)
	{
		var user = (
				await _userManager.FindByNameAsync(command.UserName)
				?? await _userManager.FindByEmailAsync(command.UserName)
				) ?? throw new InvalidCredentialException("No Identity User for User Name - " + command.UserName);

		var appUserDetails = await _userRepository.GetUserAsync(user.Id) ?? throw new InvalidCredentialException("No User details for User Name - " + command.UserName);

		var userExists = await _userManager.CheckPasswordAsync(user, command.Password);

		if (!userExists)
			throw new InvalidCredentialException("Invalid Credentials Provided for - " + command.UserName);

		var userRoles = await _userManager.GetRolesAsync(user);

		var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsForRolesQuery { Roles = userRoles });

		var refreshToken = _tokenFactory.GenerateToken();

		(string token, int expiresIn) = _jwtTokenHandler.CreateToken(user.Id, user.UserName!, new HashSet<string>(userRoles), userRoleClaims.RoleClaims);

		var newRefreshToken = new RefreshToken
		{
			Id = Guid.NewGuid(),
			Expires = _machineDateTime.Now.AddSeconds((long)expiresIn),
			UserId = user.Id,
			Token = refreshToken,
		};

		newRefreshToken.SetEntityAuditFields(userId, _machineDateTime.Now);

		int createTokenResult = await _refreshTokenRepository.CreateRefreshTokenAsync(newRefreshToken);

		if (createTokenResult < 1)
			throw new RecordCreationException(refreshToken, "Refresh Token");

		var userDTO = new UserDTO
		{
			Id = user.Id,
			UserName = user.UserName,
			FullName = (appUserDetails.FirstName + " " + appUserDetails.LastName).Trim(),
			Email = user.Email,
		};

		userDTO.SetDTOAuditFields(appUserDetails);

		return new AuthUserViewModel
		{
			UserDetails = userDTO,
			UserToken = new AccessTokenViewModel
			{
				AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
				RefreshToken = refreshToken,
			}
		};
	}
}
