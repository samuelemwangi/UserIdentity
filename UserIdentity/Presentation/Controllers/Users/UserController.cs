using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly GetUserQueryHandler _getUserQueryHandler;
        private readonly RegisterUserCommandHandler _registerUserCommandHandler;
        private readonly LoginUserCommandHandler _loginUserCommandHandler;
        private readonly ExchangeRefreshTokenCommandHandler _exchangeRefreshTokenCommandHandler;
        private readonly ResetPasswordCommandHandler _resetPasswordCommandHandler;
        private readonly ConfirmUpdatePasswordTokenCommandHandler _confirmUpdatePasswordTokenCommandHandler;
        private readonly UpdatePasswordCommandHandler _updatePasswordCommandHandler;

        private String resourceName => EntityName ?? "user";


        public UserController(
            GetUserQueryHandler getUserQueryHandler,
            RegisterUserCommandHandler registerUserComandHandler,
            LoginUserCommandHandler loginUserCommandHandler,
            ExchangeRefreshTokenCommandHandler exchangeRefreshTokenCommandHandler,
            ResetPasswordCommandHandler resetPasswordCommandHandler,
            ConfirmUpdatePasswordTokenCommandHandler confirmUpdatePasswordTokenCommandHandler,
            UpdatePasswordCommandHandler updatePasswordCommandHandler
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

            UserViewModel userVM = await _getUserQueryHandler.GetUserAsync(new GetUserQuery { UserId = UserId });

            var ownedByLoggedInUser = userVM.User?.CreatedBy == LoggedInUserId || userVM.User?.LastModifiedBy == LoggedInUserId;

            userVM.ResolveEditDeleteRights(UserRoleClaims, resourceName, ownedByLoggedInUser);
            userVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

            return Ok(userVM);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<AuthUserViewModel>> CreateUser(RegisterUserCommand command)
        {
            AuthUserViewModel authUserVM = await _registerUserCommandHandler.CreateUserAsync(command);

            authUserVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
            authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

            return Ok(authUserVM);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthUserViewModel>> LoginUser(LoginUserCommand command)
        {
            AuthUserViewModel authUserVM = await _loginUserCommandHandler.LoginUserAsync(command);

            authUserVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
            authUserVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

            return Ok(authUserVM);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult<AccessTokenViewModel>> RefreshToken(ExchangeRefreshTokenCommand command)
        {
            var refreshTokenVM = await _exchangeRefreshTokenCommandHandler.ExchangeRefreshTokenAsync(command);
            refreshTokenVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS, "Refresh token generated successfully");
            return Ok(refreshTokenVM);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("reset-password")]
        public async Task<ActionResult<AccessTokenViewModel>> ResetPassword(ResetPasswordCommand command)
        {
            var resetPassWordVM = await _resetPasswordCommandHandler.ResetPassword(command);
            resetPassWordVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);
            return Ok(resetPassWordVM);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("confirm-update-password-token")]
        public async Task<ActionResult<AccessTokenViewModel>> ConfirmPasswordTokne(ConfirmUpdatePasswordTokenCommand command)
        {
            var confirmPassWordTokenVM = await _confirmUpdatePasswordTokenCommandHandler.ValidateUpdatePasswordToken(command);
            confirmPassWordTokenVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);
            return Ok(confirmPassWordTokenVM);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("update-password")]
        public async Task<ActionResult<AccessTokenViewModel>> UpdatePassWord(UpdatePasswordCommand command)
        {
            var updatePasswordVM = await _updatePasswordCommandHandler.UpdatePassWord(command);
            updatePasswordVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);
            return Ok(updatePasswordVM);
        }
    }
}
