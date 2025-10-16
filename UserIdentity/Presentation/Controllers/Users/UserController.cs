using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyzenKit.Application.Core.Extensions;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.RegisteredApps.Queries;
using PolyzenKit.Application.Core.RegisteredApps.ViewModels;
using PolyzenKit.Application.Enums;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Domain.DTO;
using PolyzenKit.Domain.RegisteredApps;
using PolyzenKit.Presentation.Controllers;
using PolyzenKit.Presentation.Helpers;
using PolyzenKit.Presentation.ValidationHelpers;
using System.Net;
using UserIdentity.Application.Core.Tokens.Commands.ExchangeRefreshToken;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.Commands.ConfirmUpdatePasswordToken;
using UserIdentity.Application.Core.Users.Commands.LoginUser;
using UserIdentity.Application.Core.Users.Commands.RegisterUser;
using UserIdentity.Application.Core.Users.Commands.ResetPassword;
using UserIdentity.Application.Core.Users.Commands.UpdatePassword;
using UserIdentity.Application.Core.Users.Queries.GetUser;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Enums;

namespace UserIdentity.Presentation.Controllers.Users;

[Authorize]
[ValidateModel]
public class UserController(
	IGetItemQueryHandler<GetUserQuery, UserViewModel> getUserQueryHandler,
	ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel> registerUserComandHandler,
	ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel> loginUserCommandHandler,
	IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel> exchangeRefreshTokenCommandHandler,
	ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel> resetPasswordCommandHandler,
	IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel> confirmUpdatePasswordTokenCommandHandler,
	IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel> updatePasswordCommandHandler,

	IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> getRegisteredAppQueryHandler
	) : BaseController
{
	private readonly IGetItemQueryHandler<GetUserQuery, UserViewModel> _getUserQueryHandler = getUserQueryHandler;
	private readonly ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel> _registerUserCommandHandler = registerUserComandHandler;
	private readonly ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel> _loginUserCommandHandler = loginUserCommandHandler;
	private readonly IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel> _exchangeRefreshTokenCommandHandler = exchangeRefreshTokenCommandHandler;
	private readonly ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel> _resetPasswordCommandHandler = resetPasswordCommandHandler;
	private readonly IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel> _confirmUpdatePasswordTokenCommandHandler = confirmUpdatePasswordTokenCommandHandler;
	private readonly IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel> _updatePasswordCommandHandler = updatePasswordCommandHandler;

		private readonly IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> _getRegisteredAppQueryHandler = getRegisteredAppQueryHandler;

	private string GetXAppName() => HttpContext.GetHeaderValue<string>(ZenConstants.X_APP_NAME, true)!;

	private async Task<RegisteredAppDTO> GetAppNameAsync(string appName)
	{
		var result = await _getRegisteredAppQueryHandler.GetItemAsync(new GetRegisteredAppQuery { AppName = appName });
		return result.RegisteredApp;
	}

	[Authorize(Policy = ZenConstants.ADMIN_OR_SAME_USER_POLICY)]
	[HttpGet]
	[Route("{userId}")]
	public async Task<ActionResult<UserViewModel>> GetUser(string userId)
	{

		var userVM = await _getUserQueryHandler.GetItemAsync(new GetUserQuery { UserId = userId });

		userVM.ResolveEditDeleteRights(UserScopeClaims, EntityName, userVM.User.OwnedByLoggedInUser(LoggedInUserId));
		userVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, userVM);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("register")]
	public async Task<ActionResult<AuthUserViewModel>> CreateUser(RegisterUserCommand command)
	{
		command.RegisteredApp = await GetAppNameAsync(GetXAppName());

		var authUserVM = await _registerUserCommandHandler.CreateItemAsync(command, LoggedInUserId);

		authUserVM.ResolveEditDeleteRights(UserScopeClaims, EntityName, true);
		authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);		

		return StatusCode((int)HttpStatusCode.Created, authUserVM);
	}

	[Authorize(Policy = ZenConstants.USER_SCOPE_POLICY, Roles = ZenConstants.INTERNAL_SERVICE_ROLE)]
	[HttpPost]
	[Route("register-for-service")]
	public async Task<ActionResult<AuthUserViewModel>> CreateUserForService(RegisterUserCommand command)
	{
		command.RegisteredApp = await GetAppNameAsync(GetXAppName());
		command.RequestSource = RequestSource.API;

		var authUserVM = await _registerUserCommandHandler.CreateItemAsync(command, LoggedInUserId);

		authUserVM.ResolveEditDeleteRights(UserScopeClaims, EntityName, true);
		authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);	

		return StatusCode((int)HttpStatusCode.Created, authUserVM);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("login")]
	public async Task<ActionResult<AuthUserViewModel>> LoginUser(LoginUserCommand command)
	{
		var authUserVM = await _loginUserCommandHandler.CreateItemAsync(command, LoggedInUserId);

		authUserVM.ResolveEditDeleteRights(UserScopeClaims, EntityName, true);
		authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL, "Login successful");		

		return StatusCode((int)HttpStatusCode.OK, authUserVM);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("refresh-token")]
	public async Task<ActionResult<AccessTokenViewModel>> RefreshToken(ExchangeRefreshTokenCommand command)
	{
		var refreshTokenVM = await _exchangeRefreshTokenCommandHandler.UpdateItemAsync(command, LoggedInUserId);

		refreshTokenVM.EditEnabled = true;
		refreshTokenVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL, "Refresh token generated successfully");

		return StatusCode((int)HttpStatusCode.OK, refreshTokenVM);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("reset-password")]
	public async Task<ActionResult<AccessTokenViewModel>> ResetPassword(ResetPasswordCommand command)
	{
		var resetPassWordVM = await _resetPasswordCommandHandler.CreateItemAsync(command, LoggedInUserId);
		resetPassWordVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL, "Password reset request successful");
		return StatusCode((int)HttpStatusCode.OK, resetPassWordVM);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("confirm-update-password-token")]
	public async Task<ActionResult<AccessTokenViewModel>> ConfirmPasswordToken(ConfirmUpdatePasswordTokenCommand command)
	{
		var confirmPassWordTokenVM = await _confirmUpdatePasswordTokenCommandHandler.UpdateItemAsync(command, LoggedInUserId);

		var httpStatusCode = HttpStatusCode.OK;
		if (confirmPassWordTokenVM.TokenPasswordResult.UpdatePasswordTokenConfirmed)
		{
			confirmPassWordTokenVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.UPDATE_ITEM_SUCCESSFUL, "Token confirmation successful");
		}
		else
		{
			confirmPassWordTokenVM.ResolveRequestStatus(RequestStatus.FAILED, ItemStatusMessage.FETCH_ITEM_FAILED, "Token confirmation failed");
			httpStatusCode = HttpStatusCode.NotAcceptable;
		}

		return StatusCode((int)httpStatusCode, confirmPassWordTokenVM);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("update-password")]
	public async Task<ActionResult<AccessTokenViewModel>> UpdatePassword(UpdatePasswordCommand command)
	{
		var updatePasswordVM = await _updatePasswordCommandHandler.UpdateItemAsync(command, LoggedInUserId);

		var httpStatusCode = HttpStatusCode.OK;
		if (updatePasswordVM.UpdatePasswordResult.PassWordUpdated)
		{
			updatePasswordVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.UPDATE_ITEM_SUCCESSFUL, "Password updated successfully");
		}
		else
		{
			updatePasswordVM.ResolveRequestStatus(RequestStatus.FAILED, ItemStatusMessage.FETCH_ITEM_FAILED, "Password update failed");
			httpStatusCode = HttpStatusCode.NotAcceptable;
		}
		return StatusCode((int)httpStatusCode, updatePasswordVM);
	}
}
