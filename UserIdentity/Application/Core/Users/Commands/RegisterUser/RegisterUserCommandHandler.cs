using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Application.Attributes;
using UserIdentity.Application.Core.Extensions;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Interfaces.Security;
using UserIdentity.Application.Interfaces.Utilities;
using UserIdentity.Domain;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.RegisterUser
{
	public record RegisterUserCommand : BaseCommand
	{
		[Required]
		public string? FirstName { get; init; }

		public string? LastName { get; init; }

		[Required]
		public string? UserName { get; init; }

		[EitherOr(nameof(RegisterUserCommand.PhoneNumber), nameof(RegisterUserCommand.UserEmail))]
		public string? PhoneNumber { get; init; }

		[EitherOr(nameof(RegisterUserCommand.PhoneNumber), nameof(RegisterUserCommand.UserEmail))]
		public string? UserEmail { get; init; }

		[Required]
		public string? UserPassword { get; init; }
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


		private readonly IGetItemsQueryHandler<IList<string>, HashSet<string>> _getRoleClaimsQueryHandler;

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
			IGetItemsQueryHandler<IList<string>, HashSet<string>> getRoleClaimsQueryHandler
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
			string defaultRoleKey = _configuration.GetValue<string>("DefaultRole");

			// Use env role if role is set in the env
			string defaultRole = _configuration.GetValue<string>(defaultRoleKey) ?? defaultRoleKey;

			if (string.IsNullOrEmpty(defaultRole))
				throw new IllegalEventException("Reading Default Role", "Role");

			//  Check if default is created otherwise create 
			if (await _roleManager.FindByNameAsync(defaultRole) == null)
				if (!(await _roleManager.CreateAsync(new IdentityRole { Name = defaultRole })).Succeeded)
					throw new RecordCreationException(defaultRole, "Role");

			// Check if user exists by user name, throw RecordExistsException 
			if (await _userManager.FindByNameAsync(command.UserName) != null)
				throw new RecordExistsException(command.UserName + "", "User");

			// Check if user exists by user email, throw RecordExistsException
			if (command.UserEmail != null && (await _userManager.FindByEmailAsync(command.UserEmail) != null))
				throw new RecordExistsException(command.UserEmail + "", "User");

			var newUser = new IdentityUser
			{
				Email = command.UserEmail,
				UserName = command.UserName,
				PhoneNumber = command.PhoneNumber,
				PhoneNumberConfirmed = false,
				EmailConfirmed = false
			};

			var identityResult = await _userManager.CreateAsync(newUser, command.UserPassword);

			if (!identityResult.Succeeded)
			{
				string errors = string.Join(" ", identityResult.Errors.Select(e => e.Description));

				_logHelper.LogEvent(errors, LogLevel.Error);
				throw new RecordCreationException(errors);
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
				IsDeleted = false
			};

			userDetails.SetAuditFields(newUser.Id, _machineDateTime.Now);


			int createUserResult = await _userRepository.CreateUserAsync(userDetails);

			if (createUserResult < 1)
				throw new RecordCreationException(newUser.Id, "User");


			// Get User Roles 
			var userRoles = await _userManager.GetRolesAsync(newUser);

			var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(userRoles);


			// Generate access Token
			(string token, int expiresIn) = await _jwtFactory.GenerateEncodedTokenAsync(newUser.Id, newUser.UserName + "", userRoles, userRoleClaims);


			//Generate and save Refresh Token details
			var refreshToken = _tokenFactory.GenerateRefreshToken();

			var userRefreshToken = new RefreshToken
			{
				UserId = newUser.Id,
				Expires = _machineDateTime.Now.AddSeconds(expiresIn),
				Token = refreshToken
			};

			userRefreshToken.SetAuditFields(newUser.Id, _machineDateTime.Now);


			int createTokenResult = await _refreshTokenRepository.CreateRefreshTokenAsync(userRefreshToken);

			if (createTokenResult < 1)
				throw new RecordCreationException(refreshToken, $"Refresh Token {userRefreshToken.UserId}");


			var userDTO = new UserDTO
			{
				Id = newUser.Id,
				UserName = newUser.UserName,
				FullName = (userDetails.FirstName + " " + userDetails.LastName).Trim(),
				Email = newUser.Email
			};

			userDTO.SetDTOAuditFields(userDetails, _machineDateTime.ResolveDate);

			// Return registration results
			return new AuthUserViewModel
			{
				UserDetails = userDTO,
				UserToken = new AccessTokenViewModel
				{
					AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
					RefreshToken = refreshToken,
				}

			};

		}

	}
}
