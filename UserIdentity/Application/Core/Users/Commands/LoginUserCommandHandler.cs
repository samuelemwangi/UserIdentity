using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Domain.Entity;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Persistence.Repositories;

using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.Queries;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;

namespace UserIdentity.Application.Core.Users.Commands;

public record LoginUserCommand : IBaseCommand
{
  public int AppId { get; set; }

  [Required]
  public string UserName { get; init; } = null!;

  [Required]
  public string Password { get; init; } = null!;
}

public class LoginUserCommandHandler(
    IOptions<IdentityOptions> identityOptions,
    IJwtTokenHandler jwtTokenHandler,
    ITokenFactory tokenFactory,
    UserManager<IdentityUser> userManager,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshTokenRepository,
    IMachineDateTime machineDateTime,
    IGetItemQueryHandler<GetUserQuery, UserViewModel> getUserQueryHandler
    ) : ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel>
{
  private readonly IdentityOptions _identityOptions = identityOptions.Value;
  private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
  private readonly ITokenFactory _tokenFactory = tokenFactory;
  private readonly UserManager<IdentityUser> _userManager = userManager;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
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

    UserDTO userDto;

    try
    {
      userDto = (await _getUserQueryHandler.GetItemAsync(new GetUserQuery { UserId = user.Id })).User;
    }
    catch (NoRecordException ex)
    {
      throw new InvalidCredentialException("No User Record for Identity User - " + command.UserName, ex);
    }

    // We only include the accessToken in the payload, when user is confirmed.
    var isConfirmed = !_identityOptions.SignIn.RequireConfirmedAccount || user.EmailConfirmed || user.PhoneNumberConfirmed;
    AccessTokenViewModel? userToken = null;

    if (isConfirmed)
    {
      var refreshToken = _tokenFactory.GenerateToken();

      (var token, var expiresIn) = _jwtTokenHandler.CreateToken(user.Id, user.UserName!, userDto.Roles, userDto.RoleClaims);

      var newRefreshToken = new RefreshTokenEntity
      {
        Expires = _machineDateTime.Now.AddSeconds(expiresIn),
        UserId = user.Id,
        Token = refreshToken,
      };

      newRefreshToken.SetEntityAuditFields(user.Id, _machineDateTime.Now);

      _refreshTokenRepository.CreateEntityItem(newRefreshToken);

      await _unitOfWork.SaveChangesAsync();

      userToken = new AccessTokenViewModel
      {
        AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
        RefreshToken = refreshToken,
      };
    }

    return new AuthUserViewModel
    {
      User = userDto,
      UserToken = userToken,
      IsConfirmed = isConfirmed,
    };
  }
}
