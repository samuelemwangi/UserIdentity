using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Attributes;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Enums;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Domain.Entity;
using PolyzenKit.Domain.RegisteredApps;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Infrastructure.Security.Tokens;
using PolyzenKit.Presentation.Settings;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.Events;
using UserIdentity.Application.Core.Users.Queries.GetUser;
using UserIdentity.Application.Core.Users.Settings;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Enums;
using UserIdentity.Application.Interfaces;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RefreshTokens;
using UserIdentity.Persistence.Repositories.UserRegisteredApps;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.RegisterUser;

public record RegisterUserCommand : IBaseCommand
{
	[JsonIgnore]
	public RequestSource RequestSource { get; internal set; } = RequestSource.UI;

	[JsonIgnore]
	public RegisteredAppEntity? RegisteredApp { get; internal set; }

	[JsonIgnore]
	public CrudEvent Event { get; internal set; } = CrudEvent.CREATE;

	[Required]
	public string FirstName { get; init; } = null!;

	public string? LastName { get; init; }

	[Required]
	public string UserName { get; init; } = null!;

	[EitherOr(nameof(RegisterUserCommand.PhoneNumber), nameof(RegisterUserCommand.UserEmail))]
	public string? PhoneNumber { get; init; }

	[EitherOr(nameof(RegisterUserCommand.PhoneNumber), nameof(RegisterUserCommand.UserEmail))]
	public string? UserEmail { get; init; }

	[Required]
	public string UserPassword { get; init; } = null!;

	public string? GoogleRecaptchaToken { get; set; }
}

public class RegisterUserCommandHandler(
	IOptions<RoleSettings> roleSettingsOptions,
	UserManager<IdentityUser> userManager,
	RoleManager<IdentityRole> roleManager,
	IUserRepository userRepository,
	IRefreshTokenRepository refreshTokenRepository,
	IUserRegisteredAppRepository userRegisteredAppRepository,
	IJwtTokenHandler jwtTokenHandler,
	ITokenFactory tokenFactory,
	IMachineDateTime machineDateTime,
	IBaseEventHandler<UserUpdateEvent> userUpdateEventHandler,
	IOptions<GoogleRecaptchaSettings> googleRecaptchaSettingsOptions,
	IGoogleRecaptchaService googleRecaptchaService,
	IGetItemQueryHandler<GetUserQuery, UserViewModel> getUserQueryHandler
	) : ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel>
{
	private readonly RoleSettings _roleSettings = roleSettingsOptions.Value;
	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly RoleManager<IdentityRole> _roleManager = roleManager;

	private readonly IUserRepository _userRepository = userRepository;
	private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
	private readonly IUserRegisteredAppRepository _userRegisteredAppRepository = userRegisteredAppRepository;

	private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
	private readonly ITokenFactory _tokenFactory = tokenFactory;

	private readonly IMachineDateTime _machineDateTime = machineDateTime;

	private readonly IBaseEventHandler<UserUpdateEvent> _userUpdateEventHandler = userUpdateEventHandler;

	private readonly GoogleRecaptchaSettings _googleRecaptchaSettings = googleRecaptchaSettingsOptions.Value;
	private readonly IGoogleRecaptchaService _googleRecaptchaService = googleRecaptchaService;

	private readonly IGetItemQueryHandler<GetUserQuery, UserViewModel> _getUserQueryHandler = getUserQueryHandler;

	public async Task<AuthUserViewModel> CreateItemAsync(RegisterUserCommand command, string userId)
	{
		// Confirm Google Recaptcha Token is valid before processing
		if (_googleRecaptchaSettings.Enabled && command.RequestSource == RequestSource.UI)
		{
			var googleRecaptchaToken = ObjectUtil.RequireNonNullValue<string>(command.GoogleRecaptchaToken, nameof(command.GoogleRecaptchaToken), $"{nameof(command.GoogleRecaptchaToken)} is required");

			if (!(await _googleRecaptchaService.VerifyTokenAsync(googleRecaptchaToken)))
				throw new InvalidDataException($"Invalid {nameof(command.GoogleRecaptchaToken)} provided");
		}

		var userDefaultRole = ResolveDefaultRole(command.RegisteredApp!.AppName);

		//  Check if default role is created otherwise create 
		if (await _roleManager.FindByNameAsync(userDefaultRole) == null)
			if (!(await _roleManager.CreateAsync(new IdentityRole { Name = userDefaultRole })).Succeeded)
				throw new RecordCreationException(userDefaultRole, "Role");

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
			throw new InvalidDataException(errors);
		}

		// User vs Default Role
		var resultUserRole = await _userManager.AddToRoleAsync(newUser, userDefaultRole);

		if (!resultUserRole.Succeeded)
			throw new RecordCreationException(userDefaultRole, "UserRole");


		// Create user details
		var emailTokenBytes = Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(newUser));
		var confirmEmailToken = WebEncoders.Base64UrlEncode(emailTokenBytes);

		var userDetails = new UserEntity
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

		// Create User Registered App Mapping
		var userRegisteredApp = new UserRegisteredAppEntity
		{
			Id = $"{newUser.Id}-{command.RegisteredApp!.Id}",
			UserId = newUser.Id,
			AppId = command.RegisteredApp!.Id
		};
		userRegisteredApp.SetEntityAuditFields(newUser.Id, _machineDateTime.Now);

		await _userRegisteredAppRepository.CreateEntityItemAsync(userRegisteredApp);


		var userDto = (await _getUserQueryHandler.GetItemAsync(new GetUserQuery { UserId = userDetails.Id })).User;

		// Generate access Token
		(string token, int expiresIn) = _jwtTokenHandler.CreateToken(newUser.Id, newUser.UserName + "", userDto.Roles, userDto.RoleClaims);


		//Generate and save Refresh Token details
		var refreshToken = _tokenFactory.GenerateToken();

		var userRefreshToken = new RefreshTokenEntity
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

		// Publish User Created Event
		_ = HandleEventAsync(userDetails, newUser, command.RegisteredApp, command.RequestSource);

		// Return registration results
		return new AuthUserViewModel
		{
			User = userDto,
			UserToken = new AccessTokenViewModel
			{
				AccessToken = new AccessTokenDTO { Token = token, ExpiresIn = expiresIn },
				RefreshToken = refreshToken,
			}
		};
	}

	private string ResolveDefaultRole(string serviceName) => $"{serviceName.Trim().ToLower()}{ZenConstants.SERVICE_ROLE_SEPARATOR}{_roleSettings.DefaultRole.Trim().ToLower()}";

	private async Task HandleEventAsync(UserEntity userEntity, IdentityUser identityUser, RegisteredAppEntity registeredAppEntity, RequestSource requestSource)
	{
		var eventItem = new UserUpdateEvent
		{
			EventType = CrudEvent.CREATE,
			UserContent = new UserEventContent
			{
				UserIdentityId = identityUser.Id,
				FirstName = userEntity.FirstName,
				LastName = userEntity.LastName,
				UserName = identityUser.UserName,
				PhoneNumber = identityUser.PhoneNumber,
				PhoneNumberConfirmed = identityUser.PhoneNumberConfirmed,
				UserEmail = identityUser.Email,
				EmailConfirmed = identityUser.EmailConfirmed
			},
			RegisteredApp = registeredAppEntity,
			RequestSource = requestSource
		};

		await _userUpdateEventHandler.HandleEventAsync(eventItem);
	}
}
