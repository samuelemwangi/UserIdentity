using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Infrastructure.Utilities;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Users.Commands.LoginUser;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Commands;

public class LoginUserCommandHandlerTests
{
	private readonly IJwtTokenHandler _jwtTokenHandler;
	private readonly ITokenFactory _tokenFactory;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IUserRepository _userRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IMachineDateTime _machineDateTime;

	private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler;


	public LoginUserCommandHandlerTests()
	{
		_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
		_tokenFactory = A.Fake<ITokenFactory>();
		_userManager = A.Fake<UserManager<IdentityUser>>();
		_userRepository = A.Fake<IUserRepository>();
		_refreshTokenRepository = A.Fake<IRefreshTokenRepository>();
		_machineDateTime = new MachineDateTime();
		_getRoleClaimsQueryHandler = A.Fake<IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels>>();

	}


	[Fact]
	public async Task Login_With_Non_Existent_User_In_UserManager_Throws_InvalidCredentialException()
	{
		// Arrange
		var command = new LoginUserCommand
		{
			UserName = "test",
			Password = "test"
		};

		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserName)).Returns(default(IdentityUser));

		var handler = GetLoginUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<InvalidCredentialException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}


	[Fact]
	public async Task Login_With_Non_Existent_User_In_User_Repo_Throws_InvalidCredentialException()
	{
		// Arrange
		var command = new LoginUserCommand
		{
			UserName = "test",
			Password = "test"
		};

		var existingIdentityUser = GetIdentityUser();

		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserName)).Returns(existingIdentityUser);

		A.CallTo(() => _userRepository.GetUserAsync(existingIdentityUser.Id)).Returns(default(User));

		var handler = GetLoginUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<InvalidCredentialException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Login_With_Existing_And_No_Matching_Password_Throws_InvalidCredentialException()
	{
		// Arrange
		var command = new LoginUserCommand
		{
			UserName = "test",
			Password = "test"
		};

		var existingIdentityUser = GetIdentityUser();

		var user = GetUser(existingIdentityUser.Id);

		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserName)).Returns(existingIdentityUser);

		A.CallTo(() => _userRepository.GetUserAsync(existingIdentityUser.Id)).Returns(user);

		A.CallTo(() => _userManager.CheckPasswordAsync(existingIdentityUser, command.Password)).Returns(false);

		var handler = GetLoginUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<InvalidCredentialException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Login_With_Valid_Details_Create_Refresh_Token_Failure_Throws_InvalidCredentialException()
	{
		// Arrange
		var command = new LoginUserCommand
		{
			UserName = "test",
			Password = "test"
		};

		var existingIdentityUser = GetIdentityUser();

		var user = GetUser(existingIdentityUser.Id);

		var userRoles = new GetRoleClaimsForRolesQuery { Roles = ["Admin"] };
		var userRoleClaims = new RoleClaimsForRolesViewModels { RoleClaims = ["Admin"] };

		var refreshToken = "sampleRfereshToken";

		var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.MY3nEoHkEEVKv6sWCN6AuwJD9d3Hx6Ly66n_8spdv1Q";
		var expiresIn = 3600;

		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserName)).Returns(existingIdentityUser);

		A.CallTo(() => _userRepository.GetUserAsync(existingIdentityUser.Id)).Returns(user);

		A.CallTo(() => _userManager.CheckPasswordAsync(existingIdentityUser, command.Password)).Returns(true);

		A.CallTo(() => _userManager.GetRolesAsync(existingIdentityUser)).Returns(userRoles.Roles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(userRoles)).Returns(userRoleClaims);

		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns(refreshToken);

		A.CallTo(() => _jwtTokenHandler.CreateToken(existingIdentityUser.Id, A<string>.Ignored, userRoles.Roles.ToHashSet(), userRoleClaims.RoleClaims)).Returns((accessToken, expiresIn));

		A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshToken>.That.Matches(x => x.Token == refreshToken))).Returns(Task.FromResult(0));

		var handler = GetLoginUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Login_With_Valid_Details_Returns_Valid_Authenticated_User()
	{
		// Arrange
		var command = new LoginUserCommand
		{
			UserName = "test",
			Password = "test"
		};

		var existingIdentityUser = GetIdentityUser();

		var user = GetUser(existingIdentityUser.Id);

		var userRoles = new GetRoleClaimsForRolesQuery { Roles = ["Admin"] };
		var userRoleClaims = new RoleClaimsForRolesViewModels { RoleClaims = ["Admin"] };

		var refreshToken = "sampleRfereshToken";

		var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.MY3nEoHkEEVKv6sWCN6AuwJD9d3Hx6Ly66n_8spdv1Q";
		var expiresIn = 3600;

		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserName)).Returns(existingIdentityUser);

		A.CallTo(() => _userRepository.GetUserAsync(existingIdentityUser.Id)).Returns(user);

		A.CallTo(() => _userManager.CheckPasswordAsync(existingIdentityUser, command.Password)).Returns(true);

		A.CallTo(() => _userManager.GetRolesAsync(existingIdentityUser)).Returns(userRoles.Roles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(userRoles)).Returns(userRoleClaims);

		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns(refreshToken);

		A.CallTo(() => _jwtTokenHandler.CreateToken(existingIdentityUser.Id, A<string>.Ignored, userRoles.Roles.ToHashSet(), userRoleClaims.RoleClaims)).Returns((accessToken, expiresIn));

		A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshToken>.That.Matches(x => x.Token == refreshToken))).Returns(Task.FromResult(1));

		var handler = GetLoginUserCommandHandler();

		// Act 
		var vm = await handler.CreateItemAsync(command, TestStringHelper.UserId);

		// Assert
		Assert.IsType<AuthUserViewModel>(vm);
		Assert.NotNull(vm.UserDetails);
		Assert.NotNull(vm.UserToken);
		Assert.NotNull(vm.UserToken?.AccessToken);
		Assert.NotNull(vm.UserToken?.RefreshToken);
		Assert.Equal(refreshToken, vm.UserToken?.RefreshToken);
	}


	private LoginUserCommandHandler GetLoginUserCommandHandler()
	{
		return new LoginUserCommandHandler(
												_jwtTokenHandler,
												_tokenFactory,
												_userManager,
												_userRepository,
												_refreshTokenRepository,
												_machineDateTime,
												_getRoleClaimsQueryHandler
										);
	}

	private IdentityUser GetIdentityUser()
	{
		return new IdentityUser
		{
			Id = Guid.NewGuid().ToString(),
			UserName = "test",
			Email = "test@pl.pl"
		};

	}

	private User GetUser(string? id = null)
	{
		return new User
		{
			Id = id ?? Guid.NewGuid().ToString(),
			FirstName = "test",
			LastName = "test",
			EmailConfirmationToken = "skksk",

		};
	}
}
