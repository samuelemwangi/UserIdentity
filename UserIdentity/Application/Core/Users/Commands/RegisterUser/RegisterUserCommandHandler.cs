using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.RegisterUser
{
	public record RegisterUserCommand : BaseCommand
	{
		[Required]
		public String? FirstName { get; init; }

		public String? LastName { get; init; }

		[Required]
		public String? Username { get; init; }

		public String? PhoneNumber { get; init; }

		public String? UserEmail { get; init; }

		public String? UserPassword { get; init; }
	}

	public class RegisterUserCommandHandler : ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel>
	{
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

		public RegisterUserCommandHandler(
			UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IUserRepository userRepository,
			IRefreshTokenRepository refreshTokenRepository,
			IJwtFactory jwtFactory,
			ITokenFactory tokenFactory,
			IConfiguration configuration,
			IMachineDateTime machineDateTime,
			ILogHelper<RegisterUserCommandHandler> logHelper,
			IGetItemsQueryHandler<IList<String>, HashSet<String>> getRoleClaimsQueryHandler
			)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_userRepository = userRepository;
			_refreshTokenRepository = refreshTokenRepository;
			_jwtFactory = jwtFactory;
			_tokenFactory = tokenFactory;
			_configuration = configuration;
			_machineDateTime = machineDateTime;
			_logHelper = logHelper;
			_getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;
		}

		public async Task<AuthUserViewModel> CreateItemAsync(RegisterUserCommand command)
		{

			// Check if default role is set in configs
			String defaultRole = _configuration.GetValue<String>("DefaultRole");

			if (String.IsNullOrEmpty(defaultRole))
				throw new IllegalEventException("Reading Default Role", "Role");

			//  Check if default is created otherwise create 
			var defaultRoleDetails = await _roleManager.FindByNameAsync(defaultRole);
			if (defaultRoleDetails == null)
			{
				var createdRoleResult = await _roleManager.CreateAsync(new IdentityRole { Name = defaultRole });
				if (!createdRoleResult.Succeeded)
					throw new RecordCreationException(defaultRole, "Role");
			}

			// Check if user exists by user name, throw RecordExistsException 
			var existingUserByUserName = await _userManager.FindByNameAsync(command.Username);
			if (existingUserByUserName != null)
				throw new RecordExistsException(command.Username + "", "User");

			// Check if user exists by user name, throw RecordExistsException
			var existingUserByEmail = await _userManager.FindByEmailAsync(command.UserEmail);
			if (existingUserByEmail != null)
				throw new RecordExistsException(command.UserEmail + "", "User");

			var newUser = new IdentityUser
			{
				Email = command.UserEmail,
				UserName = command.Username,
				PhoneNumber = command.PhoneNumber,
				PhoneNumberConfirmed = false,
				EmailConfirmed = false
			};

			var identityResult = await _userManager.CreateAsync(newUser, command.UserPassword);

			if (!identityResult.Succeeded)
			{
				String errors = String.Join(" ", identityResult.Errors.Select(e => e.Description));

				_logHelper.LogEvent(errors, LogLevel.Error);
				throw new RecordCreationException(newUser.Id, "User");
			}

			// User vs Default Role
			var resultUserRole = await _userManager.AddToRoleAsync(newUser, defaultRole);

			if (!resultUserRole.Succeeded)
				throw new RecordCreationException(defaultRole, "UserRole");


			// Create user details
			var emailTokenBytes = Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(newUser));
			var confirmEmailToken = WebEncoders.Base64UrlEncode(emailTokenBytes);

			var userDetails = new User
			{
				Id = newUser.Id,
				FirstName = command.FirstName,
				LastName = command.LastName,
				EmailConfirmationToken = confirmEmailToken,
				CreatedBy = newUser.Id,
				LastModifiedBy = newUser.Id,
				CreatedDate = _machineDateTime.Now,
				LastModifiedDate = _machineDateTime.Now,
				IsDeleted = false
			};


			Int32 createUserResult = await _userRepository.CreateUserAsync(userDetails);

			if (createUserResult < 1)
			{
				throw new RecordCreationException(newUser.Id, "User");
			}


			// Get User Roles 
			var userRoles = await _userManager.GetRolesAsync(newUser);

			var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(userRoles);


			// Generate access Token
			(String token, Int32 expiresIn) = await _jwtFactory.GenerateEncodedTokenAsync(newUser.Id, newUser.UserName + "", userRoles, userRoleClaims);


			//Generate and save Refresh Token details
			var refreshToken = _tokenFactory.GenerateRefreshToken();

			var userRefreshToken = new RefreshToken
			{
				UserId = newUser.Id,
				Expires = _machineDateTime.Now.AddSeconds(expiresIn),
				Token = refreshToken,
				CreatedBy = newUser.Id,
				CreatedDate = _machineDateTime.Now,
				LastModifiedBy = newUser.Id,
				LastModifiedDate = _machineDateTime.Now,
				IsDeleted = false
			};


			Int32 createTokenResult = await _refreshTokenRepository.CreateRefreshTokenAsync(userRefreshToken);

			if (createTokenResult < 1)
			{
				throw new RecordCreationException(refreshToken, $"Refresh Token {userRefreshToken.UserId}");
			}


			// Return registration results
			return new AuthUserViewModel
			{
				UserDetails = new UserDTO
				{
					Id = newUser.Id,
					UserName = newUser.UserName,
					FullName = userDetails.FirstName + " " + userDetails.LastName,
					Email = newUser.Email,
					CreatedBy = newUser.Id,
					CreatedDate = _machineDateTime.ResolveDate(userDetails.CreatedDate),
					LastModifiedBy = newUser.Id,
					LastModifiedDate = _machineDateTime.ResolveDate(userDetails.LastModifiedDate),
				},
				UserToken = new AccessTokenViewModel
				{
					AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
					RefreshToken = refreshToken,
				}

			};

		}

	}
}
