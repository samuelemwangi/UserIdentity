using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Persistence.Repositories.RefreshTokens;

namespace UserIdentity.Application.Core.Tokens.Commands.ExchangeRefreshToken
{
	public record ExchangeRefreshTokenCommand : BaseCommand
	{
		[Required]
		public required string AccessToken { get; init; }

		[Required]
		public required string RefreshToken { get; init; }
	}

	public class ExchangeRefreshTokenCommandHandler(
		IJwtTokenHandler jwtTokenHandler,
		ITokenFactory tokenFactory,
		UserManager<IdentityUser> userManager,
		IRefreshTokenRepository refreshTokenRepository,
		IMachineDateTime machineDateTime,
		IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> getRoleClaimsQueryHandler
		) : IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel>
	{
		private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
		private readonly ITokenFactory _tokenFactory = tokenFactory;

		private readonly UserManager<IdentityUser> _userManager = userManager;
		private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
		private readonly IMachineDateTime _machineDateTime = machineDateTime;

		private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;

		public async Task<ExchangeRefreshTokenViewModel> UpdateItemAsync(ExchangeRefreshTokenCommand command, string userId)
		{
			// validate accesstoken - lifetime not checked
			var tokenValidationResult = await _jwtTokenHandler.ValidateTokenAsync(command.AccessToken);

			var tokenUserId = _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtCustomClaimNames.Id);

			var tokenUserName = _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtRegisteredClaimNames.Sub);

			if (tokenUserId == null || tokenUserName == null)
				throw new SecurityTokenException("Invalid access token provided");

			var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(tokenUserId, command.RefreshToken) ?? throw new SecurityTokenException("Invalid refresh token provided");

			var userRoles = await _userManager.GetRolesAsync(new IdentityUser { Id = tokenUserId });

			var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsForRolesQuery { Roles = userRoles });

			var updateRefreshToken = _tokenFactory.GenerateToken();

			(string token, int expiresIn) = _jwtTokenHandler.CreateToken(tokenUserId, tokenUserName, new HashSet<string>(userRoles), userRoleClaims.RoleClaims);

			refreshToken.Token = updateRefreshToken;
			refreshToken.Expires = _machineDateTime.Now.AddSeconds((long)expiresIn);

			int createTokenResult = await _refreshTokenRepository.UpdateRefreshTokenAsync(refreshToken);

			if (createTokenResult < 1)
			{
				throw new RecordUpdateException(updateRefreshToken, "Refresh Token");
			}

			var accessTokenVM = new AccessTokenViewModel
			{
				AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
				RefreshToken = updateRefreshToken,
			};

			return new ExchangeRefreshTokenViewModel
			{
				UserToken = accessTokenVM,
				EditEnabled = false,
				DeleteEnabled = false
			};

		}
	}
}
