using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Persistence.Repositories;

using UserIdentity.Application.Core.Roles.Queries;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;

namespace UserIdentity.Application.Core.Tokens.Commands;

public record ExchangeRefreshTokenCommand : IBaseCommand
{
  [Required]
  public string? AccessToken { get; init; }

  [Required]
  public string? RefreshToken { get; init; }
}

public class ExchangeRefreshTokenCommandHandler(
    IJwtTokenHandler jwtTokenHandler,
    ITokenFactory tokenFactory,
    UserManager<IdentityUser> userManager,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshTokenRepository,
    IMachineDateTime machineDateTime,
    IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> getRoleClaimsQueryHandler
    ) : IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel>
{
  private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
  private readonly ITokenFactory _tokenFactory = tokenFactory;

  private readonly UserManager<IdentityUser> _userManager = userManager;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
  private readonly IMachineDateTime _machineDateTime = machineDateTime;

  private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;

  public async Task<ExchangeRefreshTokenViewModel> UpdateItemAsync(ExchangeRefreshTokenCommand command, string userId)
  {
    var requestAccessToken = ObjectUtil.RequireNonNullValue(command.AccessToken, nameof(command.AccessToken));
    var requestRefreshToken = ObjectUtil.RequireNonNullValue(command.RefreshToken, nameof(command.RefreshToken));

    var tokenValidationResult = await _jwtTokenHandler.ValidateTokenAsync(requestAccessToken);

    var tokenUserId = _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtCustomClaimNames.Id);

    var tokenUserName = _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtRegisteredClaimNames.Sub);

    if (tokenUserId == null || tokenUserName == null)
      throw new SecurityTokenException("Invalid access token provided");

    var tokenEntity = new RefreshTokenEntity
    {
      UserId = tokenUserId,
      Token = requestRefreshToken
    };

    var existingEntity = await _refreshTokenRepository.GetEntityByAlternateIdAsync(tokenEntity, QueryCondition.MAY_OR_MAY_NOT_EXIST)
        ?? throw new SecurityTokenException("Invalid refresh token provided");

    var userRoles = await _userManager.GetRolesAsync(new IdentityUser { Id = tokenUserId });

    var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsForRolesQuery { Roles = userRoles });

    var updateRefreshToken = _tokenFactory.GenerateToken();

    (var token, var expiresIn) = _jwtTokenHandler.CreateToken(tokenUserId, tokenUserName, [.. userRoles], userRoleClaims.RoleClaims);

    existingEntity.Token = updateRefreshToken;
    existingEntity.Expires = _machineDateTime.Now.AddSeconds(expiresIn);

    _refreshTokenRepository.UpdateEntityItem(existingEntity);

    await _unitOfWork.SaveChangesAsync();

    return new ExchangeRefreshTokenViewModel
    {
      UserToken = new AccessTokenViewModel
      {
        AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
        RefreshToken = updateRefreshToken,
      },
      EditEnabled = false,
      DeleteEnabled = false
    };

  }
}
