using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Infrastructure.Utilities;
using PolyzenKit.Presentation.Settings;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Users.Commands.RegisterUser;
using UserIdentity.Application.Core.Users.Events;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.UserRegisteredApps;
using UserIdentity.Persistence.Repositories.Users;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Commands;

public class RegisterUserCommandHandlerTests : IClassFixture<TestSettingsFixture>
{
	private readonly IOptions<RoleSettings> _roleSettings;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	private readonly IUserRepository _userRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IUserRegisteredAppRepository _userRegisteredAppRepository;

	private readonly IJwtTokenHandler _jwtTokenHandler;
	private readonly ITokenFactory _tokenFactory;

	private readonly IMachineDateTime _machineDateTime;

	private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler;

	IBaseEventHandler<UserUpdateEvent> _userUpdateEventHandler;

	public RegisterUserCommandHandlerTests()
	{
		_roleSettings = A.Fake<IOptions<RoleSettings>>();

		_userManager = A.Fake<UserManager<IdentityUser>>();
		_roleManager = A.Fake<RoleManager<IdentityRole>>();

		_userRepository = A.Fake<IUserRepository>();
		_refreshTokenRepository = A.Fake<IRefreshTokenRepository>();
		_userRegisteredAppRepository = A.Fake<IUserRegisteredAppRepository>();

		_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
		_tokenFactory = A.Fake<ITokenFactory>();

		_machineDateTime = new MachineDateTime();

		_getRoleClaimsQueryHandler = A.Fake<IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels>>();
		_userUpdateEventHandler = A.Fake<IBaseEventHandler<UserUpdateEvent>>();
	}


	[Fact]
	public async Task Create_User_When_Creating_Default_Role_Fails_Throws_IllegalEventException()
	{
		// Arrange
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(default(IdentityRole));
		A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.That.Matches(x => x.Name == roleSettings.DefaultRole))).Returns(IdentityResult.Failed());

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Create_User_When_User_With_Same_UserName_Exists_Throws_RecordExistsException()
	{
		// Arrange			
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();
		var identityUser = GetIdentityUser();

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(identityUser);

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Create_User_When_User_With_Same_Email_Exists_Throws_RecordExistsException()
	{
		// Arrange			
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();
		var identityUser = GetIdentityUser();

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(identityUser);

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Create_User_When_User_Creation_Fails_Throws_InvalidDataException()
	{
		// Arrange			
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), command.UserPassword)).Returns(IdentityResult.Failed());

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<InvalidDataException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Create_User_When_Assigning_User_Default_Role_Fails_Throws_RecordCreationException()
	{
		// Arrange			
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();
		var identityUser = GetIdentityUser();

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), command.UserPassword)).Returns(IdentityResult.Success);
		A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), roleSettings.DefaultRole)).Returns(IdentityResult.Failed());

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Create_User_When_Creating_User_Fails_Throws_RecordCreationException()
	{
		// Arrange
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();
		var identityUser = GetIdentityUser();

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), command.UserPassword)).Returns(IdentityResult.Success);
		A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), roleSettings.DefaultRole)).Returns(IdentityResult.Success);

		A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns("token");

		A.CallTo(() => _userRepository.CreateUserAsync(A<UserEntity>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(0));

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));
	}

	[Fact]
	public async Task Create_User_When_Create_Refresh_Token_Fails_Throws_RecordCreationException()
	{
		// Arrange
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();
		var identityUser = GetIdentityUser();
		var userRoles = new List<string> { roleSettings.DefaultRole };
		var userRoleClaims = new HashSet<string> { "claim1", "claim2" };

		var userRolesQuery = new GetRoleClaimsForRolesQuery { Roles = userRoles };
		var userRoleClaimsVm = new RoleClaimsForRolesViewModels { RoleClaims = userRoleClaims };

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), command.UserPassword)).Returns(IdentityResult.Success);
		A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), roleSettings.DefaultRole)).Returns(IdentityResult.Success);

		A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns("token");

		A.CallTo(() => _userRepository.CreateUserAsync(A<UserEntity>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(1));

		A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns(userRoles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(A<GetRoleClaimsForRolesQuery>.That.Matches( x => x.Roles.Contains(roleSettings.DefaultRole)))).Returns(Task.FromResult(userRoleClaimsVm));

		A.CallTo(() => _jwtTokenHandler.CreateToken(identityUser.Id, identityUser.UserName + "", userRoles.ToHashSet(), userRoleClaims)).Returns(("token", 2));

		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns("refreshToken");

		A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshTokenEntity>.That.Matches(x => x.UserId == "1"))).Returns(Task.FromResult(0));

		var handler = GetRegisterUserCommandHandler();

		// Act & Assert
		await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command, TestStringHelper.UserId));

	}

	[Fact]
	public async Task Create_User_With_Valid_User_Datails_Creates_Valid_User()
	{
		// Arrange
		var roleSettings = GetRoleSettings();
		var command = GetRegisterUserCommand();
		var defaultRoleIdentity = GetIdentityRole();
		var identityUser = GetIdentityUser();
		var userRoles = new List<string> { roleSettings.DefaultRole };
		var userRoleClaims = new HashSet<string> { "claim1", "claim2" };
		var resfreshToken = "resfreshToken";

		var userRolesQuery = new GetRoleClaimsForRolesQuery { Roles = userRoles };
		var userRoleClaimsVm = new RoleClaimsForRolesViewModels { RoleClaims = userRoleClaims };

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);
		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);
		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));
		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), command.UserPassword)).Returns(IdentityResult.Success);
		A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), roleSettings.DefaultRole)).Returns(IdentityResult.Success);

		A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns("token");

		A.CallTo(() => _userRepository.CreateUserAsync(A<UserEntity>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(1));

		A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns(userRoles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(A<GetRoleClaimsForRolesQuery>.That.Matches( x => x.Roles.Contains(roleSettings.DefaultRole)))).Returns(Task.FromResult(userRoleClaimsVm));

		A.CallTo(() => _jwtTokenHandler.CreateToken(identityUser.Id, identityUser.UserName + "", userRoles.ToHashSet(), userRoleClaims)).Returns(("token", 2));

		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns(resfreshToken);

		A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshTokenEntity>.That.Matches(x => x.Token == resfreshToken))).Returns(Task.FromResult(1));

		var handler = GetRegisterUserCommandHandler();

		// Act 
		var vm = await handler.CreateItemAsync(command, TestStringHelper.UserId);

		// Assert
		Assert.IsType<AuthUserViewModel>(vm);
		Assert.NotNull(vm.User);
		Assert.NotNull(vm.UserToken);
		Assert.NotNull(vm.UserToken?.AccessToken);
		Assert.NotNull(vm.UserToken?.RefreshToken);
		Assert.Equal(resfreshToken, vm.UserToken?.RefreshToken);
	}

	[Theory]
	[InlineData("random@test.com", null)]
	[InlineData("random@test.com", "")]
	[InlineData(null, "712121212")]
	[InlineData("", "712121212")]
	public async Task Create_User_With_All_Required_Details_Creates_Valid_User(string? UserEmail, string? PhoneNumber)
	{
		// Arrange
		// Arrange
		var roleSettings = GetRoleSettings();

		var command = GetRegisterUserCommand() with { UserEmail = UserEmail, PhoneNumber = PhoneNumber };

		var defaultRoleIdentity = GetIdentityRole();

		var identityUser = GetIdentityUser();
		identityUser.Email = UserEmail;
		identityUser.NormalizedEmail = UserEmail?.ToUpper();
		identityUser.PhoneNumber = PhoneNumber;

		var userRoles = new List<string> { roleSettings.DefaultRole };
		var userRoleClaims = new HashSet<string> { "claim1", "claim2" };
		var resfreshToken = "resfreshToken";

		var userRolesQuery = new GetRoleClaimsForRolesQuery { Roles = userRoles };
		var userRoleClaimsVm = new RoleClaimsForRolesViewModels { RoleClaims = userRoleClaims };

		A.CallTo(() => _roleSettings.Value).Returns(roleSettings);

		A.CallTo(() => _roleManager.FindByNameAsync(roleSettings.DefaultRole)).Returns(defaultRoleIdentity);

		A.CallTo(() => _userManager.FindByNameAsync(command.UserName)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail!)).Returns(default(IdentityUser));

		A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), command.UserPassword)).Returns(IdentityResult.Success);
		
		A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName), roleSettings.DefaultRole)).Returns(IdentityResult.Success);

		A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns("token");

		A.CallTo(() => _userRepository.CreateUserAsync(A<UserEntity>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(1));

		A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.UserName))).Returns(userRoles);

		A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(A<GetRoleClaimsForRolesQuery>.That.Matches(x => x.Roles.Contains(roleSettings.DefaultRole)))).Returns(Task.FromResult(userRoleClaimsVm));

		A.CallTo(() => _jwtTokenHandler.CreateToken(identityUser.Id, identityUser.UserName + "", userRoles.ToHashSet(), userRoleClaims)).Returns(("token", 2));

		A.CallTo(() => _tokenFactory.GenerateToken(32)).Returns(resfreshToken);

		A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshTokenEntity>.That.Matches(x => x.Token == resfreshToken))).Returns(Task.FromResult(1));

		var handler = GetRegisterUserCommandHandler();

		// Act 
		var vm = await handler.CreateItemAsync(command, TestStringHelper.UserId);

		// Assert
		Assert.IsType<AuthUserViewModel>(vm);
		Assert.NotNull(vm.User);
		Assert.NotNull(vm.UserToken);
		Assert.NotNull(vm.UserToken?.AccessToken);
		Assert.NotNull(vm.UserToken?.RefreshToken);
		Assert.Equal(resfreshToken, vm.UserToken?.RefreshToken);
	}


	private RegisterUserCommandHandler GetRegisterUserCommandHandler()
	{
		return new RegisterUserCommandHandler(
										_roleSettings,
										_userManager,
										_roleManager,
										_userRepository,
										_refreshTokenRepository,
										_userRegisteredAppRepository,
										_jwtTokenHandler,
										_tokenFactory,
										_machineDateTime,
										_getRoleClaimsQueryHandler,
										_userUpdateEventHandler
										);
	}

	private RoleSettings GetRoleSettings() => new()
	{
		ServiceName = "UserIdentity",
		DefaultRole = "DefaultRole",
		AdminRoles = "AdminRoles",
	};


	private RegisterUserCommand GetRegisterUserCommand() => new()
	{
		FirstName = "FName",
		LastName = "LName",
		UserName = "UName",
		PhoneNumber = "1234567890",
		UserEmail = "test@email.com",
		UserPassword = "Password@123"
	};

	private IdentityRole GetIdentityRole() => new()
	{
		Id = "1",
		Name = "DefaultRole",
		NormalizedName = "DEFAULTROLE"
	};

	private IdentityUser GetIdentityUser(bool sameAsCommand = true)
	{
		if (sameAsCommand)
		{
			var command = GetRegisterUserCommand();

			return new IdentityUser
			{
				Id = "1",
				UserName = command.UserName,
				NormalizedUserName = command?.UserName?.ToUpper(),
				Email = command?.UserEmail,
				NormalizedEmail = command?.UserEmail?.ToUpper(),
				PhoneNumber = command?.PhoneNumber,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				LockoutEnabled = false,
				SecurityStamp = Guid.NewGuid().ToString("D")
			};
		}

		return new IdentityUser
		{
			Id = "1",
			UserName = "UName",
			NormalizedUserName = "UNAME",
			Email = "tester@pl.com",
			EmailConfirmed = true,
			PhoneNumberConfirmed = true,
			LockoutEnabled = false,
			SecurityStamp = Guid.NewGuid().ToString("D")
		};
	}

}
