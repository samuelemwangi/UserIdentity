using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Tokens.Commands.ExchangeRefreshToken
{
	public record ExchangeRefreshTokenCommand : BaseCommand
	{
		public String AccessToken { get; init; }
		public String RefreshToken { get; init; }
	}

	public class ExchangeRefreshTokenCommandHandler: IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel>
	{
		private readonly IJwtFactory _jwtFactory;
		private readonly ITokenFactory _tokenFactory;
		private readonly IJwtTokenValidator _jwtTokenValidator;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IMachineDateTime _machineDateTime;

		private readonly IGetItemsQueryHandler<IList<String>, HashSet<String>> _getRoleClaimsQueryHandler;


		public ExchangeRefreshTokenCommandHandler(
			IJwtFactory jwtFactory,
			ITokenFactory tokenFactory,
			IJwtTokenValidator jwtTokenValidator,
			UserManager<IdentityUser> userManager,
			IRefreshTokenRepository refreshTokenRepository,
			IMachineDateTime machineDateTime,
			IGetItemsQueryHandler<IList<String>, HashSet<String>> getRoleClaimsQueryHandler
			)
		{
			_jwtFactory = jwtFactory;
			_tokenFactory = tokenFactory;
			_jwtTokenValidator = jwtTokenValidator;
			_userManager = userManager;
			_refreshTokenRepository = refreshTokenRepository;
			_machineDateTime = machineDateTime;
			_getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;
		}

		public async Task<ExchangeRefreshTokenViewModel> UpdateItemAsync(ExchangeRefreshTokenCommand command)
		{			
			var claimsPrincipal = _jwtTokenValidator.GetPrincipalFromToken(command.AccessToken);
			
			if (claimsPrincipal == null)
				throw new SecurityTokenException("Invalid access token provided");		

			var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "id");
			var userName = claimsPrincipal.Claims.FirstOrDefault(c => c.Subject != null);

			if (userId == null || userName == null)
				throw new SecurityTokenException("Invalid access token provided");

			var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(userId.Value, command.RefreshToken);

			if (refreshToken == null)
				throw new SecurityTokenException("Invalid refresh token provided");

			var userRoles = await _userManager.GetRolesAsync(new IdentityUser { Id = userId.Value });

			var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(userRoles);

			var updateRefreshToken = _tokenFactory.GenerateRefreshToken();

			(String token, Int32 expiresIn) = await _jwtFactory.GenerateEncodedTokenAsync(userId.Value, userName.Value, userRoles, userRoleClaims);

			refreshToken.Token = updateRefreshToken;
			refreshToken.Expires = _machineDateTime.Now.AddSeconds((long)expiresIn);

			Int32 createTokenResult = await _refreshTokenRepository.UpdateRefreshTokenAsync(refreshToken);

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
