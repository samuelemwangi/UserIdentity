using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Infrastructure.Utilities;
using UserIdentity.Application.Core.Roles.Queries;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Tokens.Commands;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Tokens.Commands;

public class ExchangeRefreshTokenCommandHandlerTests
{
	private readonly IJwtTokenHandler _jwtTokenHandler;
	private readonly ITokenFactory _tokenFactory;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IMachineDateTime _machineDateTime;

	private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler;


	public ExchangeRefreshTokenCommandHandlerTests()
	{
		_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
		_tokenFactory = A.Fake<ITokenFactory>();
		_userManager = A.Fake<UserManager<IdentityUser>>();
		_refreshTokenRepository = A.Fake<IRefreshTokenRepository>();
		_machineDateTime = new MachineDateTime();
		_getRoleClaimsQueryHandler = A.Fake<IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels>>();
	}

	[Theory]
	[InlineData(null, "valid-user-name")]
	[InlineData("valid-user-id", null)]
	[InlineData(null, null)]
	public async Task ExchangeRefreshToken_With_Refresh_Token_With_No_Id_Or_Subject_Throws_SecurityTokenException(string? userId, string? userName)
	{
		// Arrange
		var tokenValidationResult = new TokenValidationResult
		{
		};

		var command = new ExchangeRefreshTokenCommand
		{
			AccessToken = "SampleInvalidAccessToken",
			RefreshToken = "SampleInvalidRefreshToken"
		};

		A.CallTo(() => _jwtTokenHandler.ValidateTokenAsync(command.AccessToken)).Returns(Task.FromResult(tokenValidationResult));
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtCustomClaimNames.Id)).Returns(userId);
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtRegisteredClaimNames.Sub)).Returns(userName);

		var hanndler = GetExchangeRefreshTokenCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<SecurityTokenException>(() => hanndler.UpdateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task ExchangeRefreshToken_With_No_Existing_Refresh_Token_Throws_SecurityTokenException()
	{
		// Arrange
		var command = new ExchangeRefreshTokenCommand
		{

			AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.hqWGSaFpvbrXkOWc6lrnffhNWR19W_S1YKFBx2arWBk",
			RefreshToken = "SamplevalidRefreshToken"
		};

		var tokenValidationResult = new TokenValidationResult
		{
		};

		A.CallTo(() => _jwtTokenHandler.ValidateTokenAsync(command.AccessToken)).Returns(Task.FromResult(tokenValidationResult));
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtCustomClaimNames.Id)).Returns(TestStringHelper.UserId);
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtRegisteredClaimNames.Sub)).Returns(TestStringHelper.UserName);

		A.CallTo(() => _refreshTokenRepository.GetRefreshTokenAsync(TestStringHelper.UserId, command.RefreshToken)).Returns(default(RefreshTokenEntity));

		var hanndler = GetExchangeRefreshTokenCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<SecurityTokenException>(() => hanndler.UpdateItemAsync(command, TestStringHelper.UserId));

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

		var dbRefreshToken = new RefreshTokenEntity
		{
			Id = Guid.NewGuid(),
			UserId = TestStringHelper.UserId,
			Token = refreshToken,
			Expires = _machineDateTime.Now.AddMinutes(5),
		};

		var userRoles = new GetRoleClaimsForRolesQuery { Roles = ["Admin"] };
		var userRoleClaims = new RoleClaimsForRolesViewModels { RoleClaims = ["Admin"] };
		var updatedRefreshToken = dbRefreshToken + "updated";

		var newAccesToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.tL95SMCv9-I_ApoP8DKhhCHd2YcbscFNWo5feRsOwnQ";
		var newAccesstokenExpiresIn = 90000;

		var tokenValidationResult = new TokenValidationResult
		{
		};

		A.CallTo(() => _jwtTokenHandler.ValidateTokenAsync(command.AccessToken)).Returns(Task.FromResult(tokenValidationResult));
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtCustomClaimNames.Id)).Returns(TestStringHelper.UserId);
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtRegisteredClaimNames.Sub)).Returns(TestStringHelper.UserName);

		A.CallTo(() => _refreshTokenRepository.GetRefreshTokenAsync(TestStringHelper.UserId, refreshToken)).Returns(dbRefreshToken);

		A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.Id == TestStringHelper.UserId))).Returns(userRoles.Roles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(userRoles)).Returns(userRoleClaims);
		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns(updatedRefreshToken);


		A.CallTo(() => _jwtTokenHandler.CreateToken(TestStringHelper.UserId, TestStringHelper.UserName, userRoles.Roles.ToHashSet(), userRoleClaims.RoleClaims)).Returns((newAccesToken, newAccesstokenExpiresIn));

		A.CallTo(() => _refreshTokenRepository.UpdateRefreshTokenAsync(A<RefreshTokenEntity>.That.Matches(x => x.Token == updatedRefreshToken))).Returns(0);

		var hanndler = GetExchangeRefreshTokenCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordUpdateException>(() => hanndler.UpdateItemAsync(command, TestStringHelper.UserId));
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

		var dbRefreshToken = new RefreshTokenEntity
		{
			Id = Guid.NewGuid(),
			UserId = TestStringHelper.UserId,
			Token = refreshToken,
			Expires = _machineDateTime.Now.AddMinutes(5),
		};

		var userRoles = new GetRoleClaimsForRolesQuery { Roles = ["Admin"] };
		var userRoleClaims = new RoleClaimsForRolesViewModels { RoleClaims = ["Admin"] };
		var updatedRefreshToken = dbRefreshToken.Token + "updated";

		var newAccesToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.tL95SMCv9-I_ApoP8DKhhCHd2YcbscFNWo5feRsOwnQ";
		var newAccesstokenExpiresIn = 90000;

		var tokenValidationResult = new TokenValidationResult
		{
		};

		A.CallTo(() => _jwtTokenHandler.ValidateTokenAsync(command.AccessToken)).Returns(Task.FromResult(tokenValidationResult));
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtCustomClaimNames.Id)).Returns(TestStringHelper.UserId);
		A.CallTo(() => _jwtTokenHandler.ResolveTokenValue<string?>(tokenValidationResult, JwtRegisteredClaimNames.Sub)).Returns(TestStringHelper.UserName);

		A.CallTo(() => _refreshTokenRepository.GetRefreshTokenAsync(TestStringHelper.UserId, refreshToken)).Returns(dbRefreshToken);

		A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.Id == TestStringHelper.UserId))).Returns(userRoles.Roles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(userRoles)).Returns(userRoleClaims);
		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns(updatedRefreshToken);


		A.CallTo(() => _jwtTokenHandler.CreateToken(TestStringHelper.UserId, TestStringHelper.UserName, userRoles.Roles.ToHashSet(), userRoleClaims.RoleClaims)).Returns((newAccesToken, newAccesstokenExpiresIn));

		A.CallTo(() => _refreshTokenRepository.UpdateRefreshTokenAsync(A<RefreshTokenEntity>.That.Matches(x => x.Token == updatedRefreshToken))).Returns(1);

		var hanndler = GetExchangeRefreshTokenCommandHandler();

		// Act
		var vm = await hanndler.UpdateItemAsync(command, TestStringHelper.UserId);

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
						_jwtTokenHandler,
						_tokenFactory,
						_userManager,
						_refreshTokenRepository,
						_machineDateTime,
						_getRoleClaimsQueryHandler
		);
	}

}
