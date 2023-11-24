using FakeItEasy;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Tokens.Commands.ExchangeRefreshToken;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Domain.Identity;
using UserIdentity.Infrastructure.Utilities;
using UserIdentity.Persistence.Repositories.RefreshTokens;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Tokens.Commands
{
	public class ExchangeRefreshTokenCommandHandlerTests
	{
		private readonly IJwtFactory _jwtFactory;
		private readonly ITokenFactory _tokenFactory;
		private readonly IJwtTokenValidator _jwtTokenValidator;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IMachineDateTime _machineDateTime;

		private readonly IGetItemsQueryHandler<IList<String>, HashSet<String>> _getRoleClaimsQueryHandler;


		public ExchangeRefreshTokenCommandHandlerTests()
		{
			_jwtFactory = A.Fake<IJwtFactory>();
			_tokenFactory = A.Fake<ITokenFactory>();
			_jwtTokenValidator = A.Fake<IJwtTokenValidator>();
			_userManager = A.Fake<UserManager<IdentityUser>>();
			_refreshTokenRepository = A.Fake<IRefreshTokenRepository>();
			_machineDateTime = new MachineDateTime();
			_getRoleClaimsQueryHandler = A.Fake<IGetItemsQueryHandler<IList<String>, HashSet<String>>>();
		}

		[Fact]
		public async Task ExchangeRefreshToken_With_Refresh_Token_Throws_SecurityTokenException()
		{
			// Arrange
			var command = new ExchangeRefreshTokenCommand
			{

				AccessToken = "SampleInvalidAccessToken",
				RefreshToken = "SampleInvalidRefreshToken"
			};

			A.CallTo(() => _jwtTokenValidator.GetPrincipalFromToken(command.AccessToken)).Returns(default(ClaimsPrincipal));

			var hanndler = GetExchangeRefreshTokenCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<SecurityTokenException>(() => hanndler.UpdateItemAsync(command));
		}

		[Fact]
		public async Task ExchangeRefreshToken_With_Refresh_Token_With_No_Id_Or_Subject_Throws_SecurityTokenException()
		{
			// Arrange
			var accesstoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.hqWGSaFpvbrXkOWc6lrnffhNWR19W_S1YKFBx2arWBk";
			var refreshToken = "SamplevalidRefreshToken";

			var command = new ExchangeRefreshTokenCommand
			{

				AccessToken = accesstoken,
				RefreshToken = refreshToken
			};

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
																new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
												}, "mock"));

			A.CallTo(() => _jwtTokenValidator.GetPrincipalFromToken(accesstoken)).Returns(claimsPrincipal);

			var hanndler = GetExchangeRefreshTokenCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<SecurityTokenException>(() => hanndler.UpdateItemAsync(command));

		}

		[Fact]
		public async Task ExchangeRefreshToken_With_No_Existing_Refresh_Token_Throws_SecurityTokenException()
		{
			// Arrange
			var accesstoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.hqWGSaFpvbrXkOWc6lrnffhNWR19W_S1YKFBx2arWBk";
			var refreshToken = "SamplevalidRefreshToken";

			var command = new ExchangeRefreshTokenCommand
			{

				AccessToken = accesstoken,
				RefreshToken = refreshToken
			};

			var userId = Guid.NewGuid().ToString();

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
																new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
																new Claim(JwtRegisteredClaimNames.Sub, userId),
																new Claim("id", userId)

												}, "mock"));

			A.CallTo(() => _jwtTokenValidator.GetPrincipalFromToken(accesstoken)).Returns(claimsPrincipal);
			A.CallTo(() => _refreshTokenRepository.GetRefreshTokenAsync(userId, refreshToken)).Returns(default(RefreshToken));

			var hanndler = GetExchangeRefreshTokenCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<SecurityTokenException>(() => hanndler.UpdateItemAsync(command));

		}

		[Fact]
		public async Task ExchangeRefreshToken_When_Update_Rfresh_Token_Fails_Throws_RecordUpdateException()
		{

			// Arrange
			var accesstoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.hqWGSaFpvbrXkOWc6lrnffhNWR19W_S1YKFBx2arWBk";
			var refreshToken = "SamplevalidRefreshToken";

			var command = new ExchangeRefreshTokenCommand
			{

				AccessToken = accesstoken,
				RefreshToken = refreshToken
			};

			var userId = Guid.NewGuid().ToString();

			var dbRefreshToken = new RefreshToken
			{
				UserId = userId,
				Token = refreshToken,
				Expires = _machineDateTime.Now.AddMinutes(5),
			};

			var userRoles = new List<String> { "Admin" };
			var userRoleClaims = new HashSet<String> { "Admin" };
			var updatedRefreshToken = dbRefreshToken + "updated";

			var newAccesToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.tL95SMCv9-I_ApoP8DKhhCHd2YcbscFNWo5feRsOwnQ";
			var newAccesstokenExpiresIn = 90000;

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
																new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
																new Claim(JwtRegisteredClaimNames.Sub, userId),
																new Claim("id", userId)

												}, "mock"));

			A.CallTo(() => _jwtTokenValidator.GetPrincipalFromToken(accesstoken)).Returns(claimsPrincipal);
			A.CallTo(() => _refreshTokenRepository.GetRefreshTokenAsync(userId, refreshToken)).Returns(dbRefreshToken);

			A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.Id == userId))).Returns(userRoles);

			A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(userRoles)).Returns(userRoleClaims);
			A.CallTo(() => _tokenFactory.GenerateRefreshToken(32)).Returns(updatedRefreshToken);


			A.CallTo(() => _jwtFactory.GenerateEncodedTokenAsync(userId, userId, userRoles, userRoleClaims)).Returns((newAccesToken, newAccesstokenExpiresIn));

			A.CallTo(() => _refreshTokenRepository.UpdateRefreshTokenAsync(A<RefreshToken>.That.Matches(x => x.Token == updatedRefreshToken))).Returns(0);

			var hanndler = GetExchangeRefreshTokenCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordUpdateException>(() => hanndler.UpdateItemAsync(command));
		}

		[Fact]
		public async Task ExchangeRefreshToken_When_Update_Rfresh_Token_Returns_New_RefreshToken()
		{

			// Arrange
			var accesstoken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.hqWGSaFpvbrXkOWc6lrnffhNWR19W_S1YKFBx2arWBk";
			var refreshToken = "SamplevalidRefreshToken";

			var command = new ExchangeRefreshTokenCommand
			{

				AccessToken = accesstoken,
				RefreshToken = refreshToken
			};

			var userId = Guid.NewGuid().ToString();

			var dbRefreshToken = new RefreshToken
			{
				UserId = userId,
				Token = refreshToken,
				Expires = _machineDateTime.Now.AddMinutes(5),
			};

			var userRoles = new List<String> { "Admin" };
			var userRoleClaims = new HashSet<String> { "Admin" };
			var updatedRefreshToken = dbRefreshToken.Token + "updated";

			var newAccesToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.tL95SMCv9-I_ApoP8DKhhCHd2YcbscFNWo5feRsOwnQ";
			var newAccesstokenExpiresIn = 90000;

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
																new Claim(JwtRegisteredClaimNames.Jti,userId),
																new Claim(JwtRegisteredClaimNames.Sub, userId),
																new Claim("id", userId)

												}, "mock"));

			A.CallTo(() => _jwtTokenValidator.GetPrincipalFromToken(accesstoken)).Returns(claimsPrincipal);
			A.CallTo(() => _refreshTokenRepository.GetRefreshTokenAsync(userId, refreshToken)).Returns(dbRefreshToken);

			A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.Id == userId))).Returns(userRoles);

			A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(userRoles)).Returns(userRoleClaims);
			A.CallTo(() => _tokenFactory.GenerateRefreshToken(32)).Returns(updatedRefreshToken);


			A.CallTo(() => _jwtFactory.GenerateEncodedTokenAsync(userId, userId, userRoles, userRoleClaims)).Returns((newAccesToken, newAccesstokenExpiresIn));

			A.CallTo(() => _refreshTokenRepository.UpdateRefreshTokenAsync(A<RefreshToken>.That.Matches(x => x.Token == updatedRefreshToken))).Returns(1);

			var hanndler = GetExchangeRefreshTokenCommandHandler();

			// Act
			var vm = await hanndler.UpdateItemAsync(command);

			// Assert
			Assert.IsType<ExchangeRefreshTokenViewModel>(vm);
			Assert.IsType<AccessTokenViewModel>(vm.UserToken);
			Assert.IsType<AccessTokenDTO>(vm.UserToken.AccessToken);

			Assert.Equal(newAccesToken, vm.UserToken.AccessToken?.Token);
			Assert.Equal(newAccesstokenExpiresIn, vm.UserToken.AccessToken?.ExpiresIn);
			Assert.Equal(updatedRefreshToken, vm.UserToken.RefreshToken);
		}


		private ExchangeRefreshTokenCommandHandler GetExchangeRefreshTokenCommandHandler()
		{
			return new ExchangeRefreshTokenCommandHandler(
							_jwtFactory,
							_tokenFactory,
							_jwtTokenValidator,
							_userManager,
							_refreshTokenRepository,
							_machineDateTime,
							_getRoleClaimsQueryHandler
			);
		}

	}
}
