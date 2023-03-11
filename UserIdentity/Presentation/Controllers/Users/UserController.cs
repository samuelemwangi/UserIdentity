using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Extensions;
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
using UserIdentity.Presentation.Helpers.ValidationExceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserIdentity.Presentation.Controllers.Users
{
	[Authorize]
	[ValidateModel]
	public class UserController : BaseController
	{
		private readonly IGetItemQueryHandler<GetUserQuery, UserViewModel> _getUserQueryHandler;
		private readonly ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel> _registerUserCommandHandler;
		private readonly ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel> _loginUserCommandHandler;
		private readonly IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel> _exchangeRefreshTokenCommandHandler;
		private readonly ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel> _resetPasswordCommandHandler;
		private readonly IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel> _confirmUpdatePasswordTokenCommandHandler;
		private readonly IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel> _updatePasswordCommandHandler;

		private String resourceName => EntityName ?? "user";


		public UserController(
				IGetItemQueryHandler<GetUserQuery, UserViewModel> getUserQueryHandler,
				ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel> registerUserComandHandler,
				ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel> loginUserCommandHandler,
				IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel> exchangeRefreshTokenCommandHandler,
				ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel> resetPasswordCommandHandler,
				IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel> confirmUpdatePasswordTokenCommandHandler,
				IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel> updatePasswordCommandHandler
				)
		{
			_getUserQueryHandler = getUserQueryHandler;
			_registerUserCommandHandler = registerUserComandHandler;
			_loginUserCommandHandler = loginUserCommandHandler;
			_exchangeRefreshTokenCommandHandler = exchangeRefreshTokenCommandHandler;
			_resetPasswordCommandHandler = resetPasswordCommandHandler;
			_confirmUpdatePasswordTokenCommandHandler = confirmUpdatePasswordTokenCommandHandler;
			_updatePasswordCommandHandler = updatePasswordCommandHandler;
		}


		[HttpGet]
		[Route("{UserId}")]
		public async Task<ActionResult<UserViewModel>> GetUser(String UserId)
		{

			var userVM = await _getUserQueryHandler.GetItemAsync(new GetUserQuery { UserId = UserId });

			var ownedByLoggedInUser = userVM.User.OwnedByLoggedInUser(LoggedInUserId);

			userVM.ResolveEditDeleteRights(UserRoleClaims, resourceName, ownedByLoggedInUser);
			userVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return Ok(userVM);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("register")]
		public async Task<ActionResult<AuthUserViewModel>> CreateUser(RegisterUserCommand command)
		{
			var authUserVM = await _registerUserCommandHandler.CreateItemAsync(command);

			authUserVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return Ok(authUserVM);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("login")]
		public async Task<ActionResult<AuthUserViewModel>> LoginUser(LoginUserCommand command)
		{
			var authUserVM = await _loginUserCommandHandler.CreateItemAsync(command);

			authUserVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return Ok(authUserVM);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("refresh-token")]
		public async Task<ActionResult<AccessTokenViewModel>> RefreshToken(ExchangeRefreshTokenCommand command)
		{
			var refreshTokenVM = await _exchangeRefreshTokenCommandHandler.UpdateItemAsync(command);
			refreshTokenVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS, "Refresh token generated successfully");
			return Ok(refreshTokenVM);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("reset-password")]
		public async Task<ActionResult<AccessTokenViewModel>> ResetPassword(ResetPasswordCommand command)
		{
			var resetPassWordVM = await _resetPasswordCommandHandler.CreateItemAsync(command);
			resetPassWordVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);
			return Ok(resetPassWordVM);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("confirm-update-password-token")]
		public async Task<ActionResult<AccessTokenViewModel>> ConfirmPasswordToken(ConfirmUpdatePasswordTokenCommand command)
		{
			var confirmPassWordTokenVM = await _confirmUpdatePasswordTokenCommandHandler.UpdateItemAsync(command);
			confirmPassWordTokenVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);
			return Ok(confirmPassWordTokenVM);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("update-password")]
		public async Task<ActionResult<AccessTokenViewModel>> UpdatePassword(UpdatePasswordCommand command)
		{
			var updatePasswordVM = await _updatePasswordCommandHandler.UpdateItemAsync(command);
			updatePasswordVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);
			return Ok(updatePasswordVM);
		}
	}
}
