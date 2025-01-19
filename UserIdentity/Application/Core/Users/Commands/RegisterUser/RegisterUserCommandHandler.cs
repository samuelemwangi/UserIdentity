using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Attributes;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Domain.DTO;
using PolyzenKit.Domain.Entity;
using PolyzenKit.Infrastructure.Configurations;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Presentation.Settings;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.RegisterUser
{
	public record RegisterUserCommand : BaseCommand
	{
		[Required]
		public required string FirstName { get; init; }

		public string? LastName { get; init; }

		[Required]
		public required string UserName { get; init; }

		[EitherOr(nameof(RegisterUserCommand.PhoneNumber), nameof(RegisterUserCommand.UserEmail))]
		public string? PhoneNumber { get; init; }

		[EitherOr(nameof(RegisterUserCommand.PhoneNumber), nameof(RegisterUserCommand.UserEmail))]
		public string? UserEmail { get; init; }

		[Required]
		public required string UserPassword { get; init; }
	}

	public class RegisterUserCommandHandler(
		IOptions<RoleSettings> roleSettings,
		UserManager<IdentityUser> userManager,
		RoleManager<IdentityRole> roleManager,
		IUserRepository userRepository,
		IRefreshTokenRepository refreshTokenRepository,
		IJwtTokenHandler jwtTokenHandler,
		ITokenFactory tokenFactory,
		IConfiguration configuration,
		IMachineDateTime machineDateTime,
		IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> getRoleClaimsQueryHandler
		) : ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel>
	{
		private readonly RoleSettings _roleSettings = roleSettings.Value;
		private readonly UserManager<IdentityUser> _userManager = userManager;
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;

		private readonly IUserRepository _userRepository = userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
		private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
		private readonly ITokenFactory _tokenFactory = tokenFactory;

		private readonly IConfiguration _configuration = configuration;

		private readonly IMachineDateTime _machineDateTime = machineDateTime;

		private readonly IGetItemsQueryHandler<GetRoleClaimsForRolesQuery, RoleClaimsForRolesViewModels> _getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;

		public async Task<AuthUserViewModel> CreateItemAsync(RegisterUserCommand command, string userId)
		{
			//  Check if default role is created otherwise create 
			if (await _roleManager.FindByNameAsync(_roleSettings.DefaultRole) == null)
				if (!(await _roleManager.CreateAsync(new IdentityRole { Name = _roleSettings.DefaultRole })).Succeeded)
					throw new RecordCreationException(_roleSettings.DefaultRole, "Role");

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
				throw new RecordCreationException(errors);
			}

			// User vs Default Role
			var resultUserRole = await _userManager.AddToRoleAsync(newUser, _roleSettings.DefaultRole);

			if (!resultUserRole.Succeeded)
				throw new RecordCreationException(_roleSettings.DefaultRole, "UserRole");


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

			userDetails.SetEntityAuditFields(newUser.Id, _machineDateTime.Now);

			int createUserResult = await _userRepository.CreateUserAsync(userDetails);

			if (createUserResult < 1)
				throw new RecordCreationException(newUser.Id, "User");


			// Get User Roles 
			var userRoles = await _userManager.GetRolesAsync(newUser);

			var userRoleClaims = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsForRolesQuery { Roles = userRoles });

			// Generate access Token
			(string token, int expiresIn) = _jwtTokenHandler.CreateToken(newUser.Id, newUser.UserName + "", new HashSet<string>(userRoles), userRoleClaims.RoleClaims);


			//Generate and save Refresh Token details
			var refreshToken = _tokenFactory.GenerateToken();

			var userRefreshToken = new RefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = newUser.Id,
				Expires = _machineDateTime.Now.AddSeconds(expiresIn),
				Token = refreshToken
			};

			userRefreshToken.SetEntityAuditFields(newUser.Id, _machineDateTime.Now);

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
