﻿using System;
using System.Net;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Enums;

using UserIdentity.Application.Core.Tokens.Commands.ExchangeRefreshToken;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.Commands.ConfirmUpdatePasswordToken;
using UserIdentity.Application.Core.Users.Commands.LoginUser;
using UserIdentity.Application.Core.Users.Commands.RegisterUser;
using UserIdentity.Application.Core.Users.Commands.ResetPassword;
using UserIdentity.Application.Core.Users.Commands.UpdatePassword;
using UserIdentity.Application.Core.Users.Queries.GetUser;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Presentation.Controllers.Users;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

namespace UserIdentity.UnitTests.Presentation.Controllers;

public class UserControllerTests
{
	private readonly static string Controllername = "user";

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
		var userName = "test.user";
		var userEmail = userName + "@test.com";
		var getUserQuery = new GetUserQuery { UserId = userId };
		var userVM = new UserViewModel { User = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = userName, Email = userEmail } };

		// Act
		A.CallTo(() => _getUserQueryHandler.GetItemAsync(getUserQuery)).Returns(userVM);

		var controller = GetUserController();
		controller.UpdateContext(null);
		var actionResult = await controller.GetUser(userId);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as UserViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.Equal(userId, vm?.User?.Id);
		Assert.Equal(userFName + userLName, vm?.User?.FullName);
		Assert.Equal(userName, vm?.User?.UserName);
		Assert.Equal(userEmail, vm?.User?.Email);

		Assert.False(vm?.EditEnabled);
		Assert.False(vm?.DeleteEnabled);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains(ItemStatusMessage.FETCH_ITEM_SUCCESSFUL.Description(), vm?.StatusMessage);
	}

	[Fact]
	public async Task Create_User_Should_Create_User()
	{
		// Arrange
		var userId = Guid.NewGuid().ToString();
		var userFName = "Test";
		var userLName = "User";
		var userName = "test.user";
		var userEmail = userName + "@test.com";
		var userPassword = "testPassword";

		var refreshToken = "abmsmmsrefreshToken";

		var command = new RegisterUserCommand { FirstName = userFName, LastName = userLName, UserName = userName, UserEmail = userEmail, UserPassword = userPassword };

		var authVM = new AuthUserViewModel { UserDetails = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = userName, Email = userEmail }, UserToken = new AccessTokenViewModel { RefreshToken = refreshToken } };

		// Act
		A.CallTo(() => _registerUserCommandHandler.CreateItemAsync(command, TestStringHelper.UserId)).Returns(authVM);

		var controller = GetUserController();
		controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
		var actionResult = await controller.CreateUser(command);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as AuthUserViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.Created, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.Equal(userId, vm?.UserDetails?.Id);
		Assert.Equal(userFName + userLName, vm?.UserDetails?.FullName);
		Assert.Equal(userName, vm?.UserDetails?.UserName);
		Assert.Equal(userEmail, vm?.UserDetails?.Email);


		Assert.Equal(refreshToken, vm?.UserToken?.RefreshToken);

		Assert.True(vm?.EditEnabled);
		Assert.True(vm?.DeleteEnabled);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains(ItemStatusMessage.CREATE_ITEM_SUCCESSFUL.Description(), vm?.StatusMessage);
	}

	[Fact]
	public async Task Login_User_Should_Login_User()
	{
		// Arrange
		var userName = "test.user";
		var userPassword = "-test.user*1+";

		var userId = Guid.NewGuid().ToString();
		var userFName = "Test";
		var userLName = "User";
		var userEmail = userName + "@test.com";

		var refreshToken = "abmsmmsrefreshToken";
		var loginMessage = "Login successful";

		var command = new LoginUserCommand { UserName = userName, Password = userPassword };
		var authVM = new AuthUserViewModel { UserDetails = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = userName, Email = userEmail }, UserToken = new AccessTokenViewModel { RefreshToken = refreshToken } };

		// Act
		A.CallTo(() => _loginUserCommandHandler.CreateItemAsync(command, TestStringHelper.UserId)).Returns(authVM);

		var controller = GetUserController();
		controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
		var actionResult = await controller.LoginUser(command);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as AuthUserViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.Equal(userId, vm?.UserDetails?.Id);
		Assert.Equal(userFName + userLName, vm?.UserDetails?.FullName);
		Assert.Equal(userName, vm?.UserDetails?.UserName);
		Assert.Equal(userEmail, vm?.UserDetails?.Email);
		Assert.Equal(refreshToken, vm?.UserToken?.RefreshToken);

		Assert.True(vm?.EditEnabled);
		Assert.True(vm?.DeleteEnabled);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains(loginMessage, vm?.StatusMessage);
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
		A.CallTo(() => _exchangeRefreshTokenCommandHandler.UpdateItemAsync(command, TestStringHelper.UserId)).Returns(exchangeRefreshTokenVM);

		var controller = GetUserController();
		controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
		var actionResult = await controller.RefreshToken(command);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as ExchangeRefreshTokenViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.Equal(newAccesstoken, vm?.UserToken?.AccessToken?.Token);
		Assert.Equal(newRefreshToken, vm?.UserToken?.RefreshToken);

		Assert.True(vm?.EditEnabled);
		Assert.False(vm?.DeleteEnabled);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains(sucessStatusMessage, vm?.StatusMessage);
	}

	[Fact]
	public async Task Reset_Password_Resets_Password()
	{
		// Arrange
		var userEmail = "user-email123@server.com";

		var emailMessage = "Password reset email sent successfully";
		var resetMessage = "Password reset request successful";

		var command = new ResetPasswordCommand { UserEmail = userEmail };
		var resetPasswordVM = new ResetPasswordViewModel { ResetPasswordDetails = new ResetPasswordDTO { EmailMessage = emailMessage } };

		// Act
		A.CallTo(() => _resetPasswordCommandHandler.CreateItemAsync(command, TestStringHelper.UserId)).Returns(resetPasswordVM);

		var controller = GetUserController();
		controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
		var actionResult = await controller.ResetPassword(command);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as ResetPasswordViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.Equal(emailMessage, vm?.ResetPasswordDetails?.EmailMessage);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains(resetMessage, vm?.StatusMessage);
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
		A.CallTo(() => _confirmUpdatePasswordTokenCommandHandler.UpdateItemAsync(command, TestStringHelper.UserId)).Returns(confirmUpdatePasswordTokenVM);

		var controller = GetUserController();
		controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
		var actionResult = await controller.ConfirmPasswordToken(command);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as ConfirmUpdatePasswordTokenViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.True(vm?.TokenPasswordResult?.UpdatePasswordTokenConfirmed);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains("Token confirmation successful", vm?.StatusMessage);
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
		A.CallTo(() => _updatePasswordCommandHandler.UpdateItemAsync(command, TestStringHelper.UserId)).Returns(updatePasswordViewModel);

		var controller = GetUserController();
		controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
		var actionResult = await controller.UpdatePassword(command);
		var result = actionResult?.Result as ObjectResult;
		var vm = result?.Value as UpdatePasswordViewModel;

		// Assert
		Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

		Assert.NotNull(vm);
		Assert.True(vm?.UpdatePasswordResult?.PassWordUpdated);

		Assert.Contains(RequestStatus.SUCCESSFUL.Description(), vm?.RequestStatus);
		Assert.Contains("Password updated successfully", vm?.StatusMessage);
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
