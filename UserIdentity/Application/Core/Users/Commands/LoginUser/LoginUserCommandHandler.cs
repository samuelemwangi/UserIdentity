using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Domain.Entity;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;

using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.Queries.GetUser;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;

namespace UserIdentity.Application.Core.Users.Commands.LoginUser;

public record LoginUserCommand : IBaseCommand
{
	public int AppId { get; set; }

	[Required]
	public string UserName { get; init; } = null!;

	[Required]
	public string Password { get; init; } = null!;
}

public class LoginUserCommandHandler(
	IJwtTokenHandler jwtTokenHandler,
	ITokenFactory tokenFactory,
	UserManager<IdentityUser> userManager,
	IRefreshTokenRepository refreshTokenRepository,
	IMachineDateTime machineDateTime,
	IGetItemQueryHandler<GetUserQuery, UserViewModel> getUserQueryHandler
	) : ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel>
{
	private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
	private readonly ITokenFactory _tokenFactory = tokenFactory;
	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
	private readonly IMachineDateTime _machineDateTime = machineDateTime;
	private readonly IGetItemQueryHandler<GetUserQuery, UserViewModel> _getUserQueryHandler = getUserQueryHandler;

	public async Task<AuthUserViewModel> CreateItemAsync(LoginUserCommand command, string userId)
	{
		var user = (
				await _userManager.FindByNameAsync(command.UserName)
				?? await _userManager.FindByEmailAsync(command.UserName)
				) ?? throw new InvalidCredentialException("No Identity User for User Name - " + command.UserName);

		var userExists = await _userManager.CheckPasswordAsync(user, command.Password);

		if (!userExists)
			throw new InvalidCredentialException("Invalid Credentials Provided for - " + command.UserName);

		var userDto = (await _getUserQueryHandler.GetItemAsync(new GetUserQuery { UserId = user.Id })).User;

		var refreshToken = _tokenFactory.GenerateToken();

		(string token, int expiresIn) = _jwtTokenHandler.CreateToken(user.Id, user.UserName!, userDto.Roles, userDto.RoleClaims);

		var newRefreshToken = new RefreshTokenEntity
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


		return new AuthUserViewModel
		{
			User = userDto,
			UserToken = new AccessTokenViewModel
			{
				AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
				RefreshToken = refreshToken,
			}
		};
	}
}
