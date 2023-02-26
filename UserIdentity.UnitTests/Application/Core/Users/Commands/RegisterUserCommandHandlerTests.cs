using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Users.Commands.RegisterUser;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Domain.Identity;
using UserIdentity.Infrastructure.Utilities;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Commands
{
	public class RegisterUserCommandHandlerTests : IClassFixture<TestSettingsFixture>
	{
		private static String defaultRoleKey = "DefaultRole";

		private readonly TestSettingsFixture _testSettings;

		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		private readonly IUserRepository _userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IJwtFactory _jwtFactory;
		private readonly ITokenFactory _tokenFactory;

		private readonly IConfiguration _configuration;

		private readonly IMachineDateTime _machineDateTime;

		private readonly ILogHelper<RegisterUserCommandHandler> _logHelper;

		private readonly IGetItemsQueryHandler<IList<String>, HashSet<String>> _getRoleClaimsQueryHandler;

		public RegisterUserCommandHandlerTests(TestSettingsFixture testSettings)
		{
			_testSettings = testSettings;

			_userManager = A.Fake<UserManager<IdentityUser>>();
			_roleManager = A.Fake<RoleManager<IdentityRole>>();

			_userRepository = A.Fake<IUserRepository>();
			_refreshTokenRepository = A.Fake<IRefreshTokenRepository>();

			_jwtFactory = A.Fake<IJwtFactory>();
			_tokenFactory = A.Fake<ITokenFactory>();

			_configuration = _testSettings.Configuration;

			_machineDateTime = new MachineDateTime();
			_logHelper = A.Fake<ILogHelper<RegisterUserCommandHandler>>();

			_getRoleClaimsQueryHandler = A.Fake<IGetItemsQueryHandler<IList<String>, HashSet<String>>>();
		}

		[Fact]
		public async Task Create_User_When_Creating_Default_Role_Fails_Throws_IllegalEventException()
		{
			// Arrange
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(default(IdentityRole));
			A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.That.Matches(x => x.Name == defaultRole))).Returns(IdentityResult.Failed());

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_User_When_User_With_Same_UserName_Exists_Throws_RecordExistsException()
		{
			// Arrange			
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();
			var identityUser = GetIdentityUser();

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(identityUser);

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_User_When_User_With_Same_Email_Exists_Throws_RecordExistsException()
		{
			// Arrange			
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();
			var identityUser = GetIdentityUser();

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(default(IdentityUser));
			A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(identityUser);

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordExistsException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_User_When_User_Creation_Fails_Throws_RecordCreationException()
		{
			// Arrange			
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(default(IdentityUser));
			A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(default(IdentityUser));

			A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), command.UserPassword)).Returns(IdentityResult.Failed());

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_User_When_Assigning_User_Default_Role_Fails_Throws_RecordCreationException()
		{
			// Arrange			
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();
			var identityUser = GetIdentityUser();

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(default(IdentityUser));
			A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(default(IdentityUser));

			A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), command.UserPassword)).Returns(IdentityResult.Success);
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), defaultRole)).Returns(IdentityResult.Failed());

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_User_When_Creating_User_Fails_Throws_RecordCreationException()
		{
			// Arrange
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();
			var identityUser = GetIdentityUser();

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(default(IdentityUser));
			A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(default(IdentityUser));

			A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), command.UserPassword)).Returns(IdentityResult.Success);
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), defaultRole)).Returns(IdentityResult.Success);

			A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username))).Returns("token");

			A.CallTo(() => _userRepository.CreateUserAsync(A<User>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(0));

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));
		}

		[Fact]
		public async Task Create_User_When_Create_Refresh_Token_Fails_Throws_RecordCreationException()
		{
			// Arrange
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();
			var identityUser = GetIdentityUser();
			var userRoles = new List<string> { defaultRole };
			var userRoleClaims = new HashSet<String> { "claim1", "claim2" };

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(default(IdentityUser));
			A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(default(IdentityUser));

			A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), command.UserPassword)).Returns(IdentityResult.Success);
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), defaultRole)).Returns(IdentityResult.Success);

			A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username))).Returns("token");

			A.CallTo(() => _userRepository.CreateUserAsync(A<User>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(1));

			A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username))).Returns(userRoles);

			A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(A<List<String>>.That.Contains(defaultRole))).Returns(Task.FromResult(userRoleClaims));

			A.CallTo(() => _jwtFactory.GenerateEncodedTokenAsync(identityUser.Id, identityUser.UserName + "", userRoles, userRoleClaims)).Returns(Task.FromResult(("token", 2)));

			A.CallTo(() => _tokenFactory.GenerateRefreshToken(32)).Returns("refreshToken");

			A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshToken>.That.Matches(x => x.UserId == "1"))).Returns(Task.FromResult(0));

			var handler = GetRegisterUserCommandHandler();

			// Act & Assert
			await Assert.ThrowsAsync<RecordCreationException>(() => handler.CreateItemAsync(command));

		}

		[Fact]
		public async Task Create_User_With_Valid_User_Datails_Creates_Valid_User()
		{
			// Arrange
			var defaultRole = _testSettings.Configuration.GetValue<String>(defaultRoleKey);
			var command = GetRegisterUserCommand();
			var defaultRoleIdentity = GetIdentityRole();
			var identityUser = GetIdentityUser();
			var userRoles = new List<string> { defaultRole };
			var userRoleClaims = new HashSet<String> { "claim1", "claim2" };
			var resfreshToken = "resfreshToken";

			A.CallTo(() => _roleManager.FindByNameAsync(defaultRole)).Returns(defaultRoleIdentity);
			A.CallTo(() => _userManager.FindByNameAsync(command.Username)).Returns(default(IdentityUser));
			A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(default(IdentityUser));

			A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), command.UserPassword)).Returns(IdentityResult.Success);
			A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username), defaultRole)).Returns(IdentityResult.Success);

			A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username))).Returns("token");

			A.CallTo(() => _userRepository.CreateUserAsync(A<User>.That.Matches(x => x.FirstName == command.FirstName))).Returns(Task.FromResult(1));

			A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.That.Matches(x => x.UserName == command.Username))).Returns(userRoles);

			A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(A<List<String>>.That.Contains(defaultRole))).Returns(Task.FromResult(userRoleClaims));

			A.CallTo(() => _jwtFactory.GenerateEncodedTokenAsync(identityUser.Id, identityUser.UserName + "", userRoles, userRoleClaims)).Returns(Task.FromResult(("token", 2)));

			A.CallTo(() => _tokenFactory.GenerateRefreshToken(32)).Returns(resfreshToken);

			A.CallTo(() => _refreshTokenRepository.CreateRefreshTokenAsync(A<RefreshToken>.That.Matches(x => x.Token == resfreshToken))).Returns(Task.FromResult(1));

			var handler = GetRegisterUserCommandHandler();

			// Act 
			var vm = await handler.CreateItemAsync(command);

			// Assert
			Assert.IsType<AuthUserViewModel>(vm);
			Assert.NotNull(vm.UserDetails);
			Assert.NotNull(vm.UserToken);
			Assert.NotNull(vm.UserToken?.AccessToken);
			Assert.NotNull(vm.UserToken?.RefreshToken);
			Assert.Equal(resfreshToken, vm.UserToken?.RefreshToken);
		}


		private RegisterUserCommandHandler GetRegisterUserCommandHandler()
		{
			return new RegisterUserCommandHandler(
											_userManager,
											_roleManager,
											_userRepository,
											_refreshTokenRepository,
											_jwtFactory,
											_tokenFactory,
											_configuration,
											_machineDateTime,
											_logHelper,
											_getRoleClaimsQueryHandler
											);
		}




		private RegisterUserCommand GetRegisterUserCommand()
		{
			return new RegisterUserCommand
			{
				FirstName = "FName",
				LastName = "LName",
				Username = "UName",
				PhoneNumber = "1234567890",
				UserEmail = "test@email.com",
				UserPassword = "Password@123"
			};
		}

		private IdentityRole GetIdentityRole()
		{
			return new IdentityRole
			{
				Id = "1",
				Name = "DefaultRole",
				NormalizedName = "DEFAULTROLE"
			};
		}

		private IdentityUser GetIdentityUser(Boolean sameAsCommand = true)
		{
			if (sameAsCommand)
			{
				var command = GetRegisterUserCommand();

				return new IdentityUser
				{
					Id = "1",
					UserName = command.Username,
					NormalizedUserName = command?.Username?.ToUpper(),
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
}
