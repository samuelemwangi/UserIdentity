using System;
using System.Net;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core;
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
using UserIdentity.Presentation.Controllers.Users;

using Xunit;

namespace UserIdentity.UnitTests.Presentation.Controllers
{
	public class UserControllerTests
	{
		private readonly static String Controllername = "user";

		private readonly IGetItemQueryHandler<GetUserQuery, UserViewModel> _getUserQueryHandler;
		private readonly ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel> _registerUserCommandHandler;
		private readonly ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel> _loginUserCommandHandler;
		private readonly IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel> _exchangeRefreshTokenCommandHandler;
		private readonly ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel> _resetPasswordCommandHandler;
		private readonly IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel> _confirmUpdatePasswordTokenCommandHandler;
		private readonly IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel> _updatePasswordCommandHandler;


		public UserControllerTests()
		{
			_getUserQueryHandler = A.Fake<IGetItemQueryHandler<GetUserQuery, UserViewModel>>();
			_registerUserCommandHandler = A.Fake<ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel>>();
			_loginUserCommandHandler = A.Fake<ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel>>();
			_exchangeRefreshTokenCommandHandler = A.Fake<IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel>>();
			_resetPasswordCommandHandler = A.Fake<ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel>>();
			_confirmUpdatePasswordTokenCommandHandler = A.Fake<IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel>>();
			_updatePasswordCommandHandler = A.Fake<IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel>>();
		}

		[Fact]
		public async Task Get_User_Should_Return_User()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var userFName = "Test";
			var userLName = "User";
			var username = "test.user";
			var userEmail = username + "@test.com";
			var getUserQuery = new GetUserQuery { UserId = userId };
			var userVM = new UserViewModel { User = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = username, Email = userEmail } };

			// Act
			A.CallTo(() => _getUserQueryHandler.GetItemAsync(getUserQuery)).Returns(userVM);

			var controller = GetUserController();
			controller.UpdateContext(null);
			var actionResult = await controller.GetUser(userId);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as UserViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(userId, vm?.User?.Id);
			Assert.Equal(userFName + userLName, vm?.User?.FullName);
			Assert.Equal(username, vm?.User?.UserName);
			Assert.Equal(userEmail, vm?.User?.Email);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.SUCCESS.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Create_User_Should_Create_User()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var userFName = "Test";
			var userLName = "User";
			var username = "test.user";
			var userEmail = username + "@test.com";
			var userPassword = "testPassword";

			var refreshToken = "abmsmmsrefreshToken";

			var command = new RegisterUserCommand { FirstName = userFName, LastName = userLName, Username = username, UserEmail = userEmail, UserPassword = userPassword };

			var authVM = new AuthUserViewModel { UserDetails = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = username, Email = userEmail }, UserToken = new AccessTokenViewModel { RefreshToken = refreshToken } };

			// Act
			A.CallTo(() => _registerUserCommandHandler.CreateItemAsync(command)).Returns(authVM);

			var controller = GetUserController();
			controller.UpdateContext(null);
			var actionResult = await controller.CreateUser(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as AuthUserViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.Created, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(userId, vm?.UserDetails?.Id);
			Assert.Equal(userFName + userLName, vm?.UserDetails?.FullName);
			Assert.Equal(username, vm?.UserDetails?.UserName);
			Assert.Equal(userEmail, vm?.UserDetails?.Email);


			Assert.Equal(refreshToken, vm?.UserToken?.RefreshToken);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.SUCCESS.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Login_User_Should_Login_User()
		{
			// Arrange
			var username = "test.user";
			var userPassword = "-test.user*1+";

			var userId = Guid.NewGuid().ToString();
			var userFName = "Test";
			var userLName = "User";
			var userEmail = username + "@test.com";

			var refreshToken = "abmsmmsrefreshToken";

			var command = new LoginUserCommand { UserName = username, Password = userPassword };
			var authVM = new AuthUserViewModel { UserDetails = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = username, Email = userEmail }, UserToken = new AccessTokenViewModel { RefreshToken = refreshToken } };

			// Act
			A.CallTo(() => _loginUserCommandHandler.CreateItemAsync(command)).Returns(authVM);

			var controller = GetUserController();
			controller.UpdateContext(Controllername);
			var actionResult = await controller.LoginUser(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as AuthUserViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(userId, vm?.UserDetails?.Id);
			Assert.Equal(userFName + userLName, vm?.UserDetails?.FullName);
			Assert.Equal(username, vm?.UserDetails?.UserName);
			Assert.Equal(userEmail, vm?.UserDetails?.Email);
			Assert.Equal(refreshToken, vm?.UserToken?.RefreshToken);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.SUCCESS.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Refresh_Token_Refreshes_Token()
		{
			// Arrange
			var accessToken = "Acccess Token This";
			var refreshToken = "Refresh Token This";

			var newAccesstoken = "New Access Token This";
			var newRefreshToken = "New Refresh Token This";

			var sucessStatusMessage = "Refresh token generated successfully";

			var exchangeRefreshTokenVM = new ExchangeRefreshTokenViewModel { UserToken = new AccessTokenViewModel { AccessToken = new AccessTokenDTO { Token = newAccesstoken }, RefreshToken = newRefreshToken } };

			var command = new ExchangeRefreshTokenCommand { AccessToken = accessToken, RefreshToken = refreshToken };

			// Act
			A.CallTo(() => _exchangeRefreshTokenCommandHandler.UpdateItemAsync(command)).Returns(exchangeRefreshTokenVM);

			var controller = GetUserController();
			controller.UpdateContext(null);
			var actionResult = await controller.RefreshToken(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as ExchangeRefreshTokenViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(newAccesstoken, vm?.UserToken?.AccessToken?.Token);
			Assert.Equal(newRefreshToken, vm?.UserToken?.RefreshToken);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(sucessStatusMessage, vm?.StatusMessage);
		}

		[Fact]
		public async Task Reset_Password_Resets_Password()
		{
			// Arrange
			var userEmail = "user-email123@server.com";

			var emailMessage = "Password reset email sent successfully";

			var command = new ResetPasswordCommand { UserEmail = userEmail };
			var resetPasswordVM = new ResetPasswordViewModel { ResetPasswordDetails = new ResetPasswordDTO { EmailMessage = emailMessage } };

			// Act
			A.CallTo(() => _resetPasswordCommandHandler.CreateItemAsync(command)).Returns(resetPasswordVM);

			var controller = GetUserController();
			controller.UpdateContext(Controllername, true, true);
			var actionResult = await controller.ResetPassword(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as ResetPasswordViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(emailMessage, vm?.ResetPasswordDetails?.EmailMessage);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.SUCCESS.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Confirm_Password_Token_Confirm_Password_Token()
		{
			// Arrange
			var confirmUpdatePasswordToken = "ConfirmUpdatePasswordToken";
			var userId = Guid.NewGuid().ToString();

			var command = new ConfirmUpdatePasswordTokenCommand { ConfirmPasswordToken = confirmUpdatePasswordToken, UserId = userId };
			var confirmUpdatePasswordTokenVM = new ConfirmUpdatePasswordTokenViewModel { TokenPasswordResult = new ConfirmUpdatePasswordDTO { UpdatePasswordTokenConfirmed = true } };

			// Act
			A.CallTo(() => _confirmUpdatePasswordTokenCommandHandler.UpdateItemAsync(command)).Returns(confirmUpdatePasswordTokenVM);

			var controller = GetUserController();
			controller.UpdateContext(Controllername,true);
			var actionResult = await controller.ConfirmPasswordToken(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as ConfirmUpdatePasswordTokenViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.True(vm?.TokenPasswordResult?.UpdatePasswordTokenConfirmed);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.SUCCESS.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Update_Password_Updates_Password()
		{

			// Arrange
			var userId = Guid.NewGuid().ToString();
			var newPassword = "NewPassword123";
			var passwordresetToken = "PasswordResetToken123";

			var command = new UpdatePasswordCommand { UserId = userId, NewPassword = newPassword, PasswordResetToken = passwordresetToken };
			var updatePasswordViewModel = new UpdatePasswordViewModel { UpdatePasswordResult = new UpdatePasswordDTO { PassWordUpdated = true } };

			// Act
			A.CallTo(() => _updatePasswordCommandHandler.UpdateItemAsync(command)).Returns(updatePasswordViewModel);

			var controller = GetUserController();
			controller.UpdateContext(Controllername, true, true, true);
			var actionResult = await controller.UpdatePassword(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as UpdatePasswordViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.True(vm?.UpdatePasswordResult?.PassWordUpdated);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.SUCCESS.GetDisplayName(), vm?.StatusMessage);
		}

		private UserController GetUserController()
		{
			return new UserController(
				_getUserQueryHandler,
				_registerUserCommandHandler,
				_loginUserCommandHandler,
				_exchangeRefreshTokenCommandHandler,
				_resetPasswordCommandHandler,
				_confirmUpdatePasswordTokenCommandHandler,
				_updatePasswordCommandHandler
			);

		}

	}

}
