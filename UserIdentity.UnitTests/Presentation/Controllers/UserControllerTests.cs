using System;
using System.Net;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.RegisteredApps.Queries;
using PolyzenKit.Application.Core.RegisteredApps.ViewModels;
using PolyzenKit.Application.Enums;

using UserIdentity.Application.Core.Tokens.Commands;
using UserIdentity.Application.Core.Tokens.ViewModels;
using UserIdentity.Application.Core.Users.Commands;
using UserIdentity.Application.Core.Users.Queries;
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

    private readonly IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> _getRegisteredAppQueryHandler;


    public UserControllerTests()
    {
        _getUserQueryHandler = A.Fake<IGetItemQueryHandler<GetUserQuery, UserViewModel>>();
        _registerUserCommandHandler = A.Fake<ICreateItemCommandHandler<RegisterUserCommand, AuthUserViewModel>>();
        _loginUserCommandHandler = A.Fake<ICreateItemCommandHandler<LoginUserCommand, AuthUserViewModel>>();
        _exchangeRefreshTokenCommandHandler = A.Fake<IUpdateItemCommandHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenViewModel>>();
        _resetPasswordCommandHandler = A.Fake<ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel>>();
        _confirmUpdatePasswordTokenCommandHandler = A.Fake<IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel>>();
        _updatePasswordCommandHandler = A.Fake<IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel>>();
        _getRegisteredAppQueryHandler = A.Fake<IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel>>();
    }

    [Fact]
    public async Task Get_User_Should_Return_User()
    {
        // Arrange
        string userId = Guid.NewGuid().ToString();
        string userFName = "Test";
        string userLName = "User";
        string userName = "test.user";
        string userEmail = userName + "@test.com";
        GetUserQuery getUserQuery = new() { UserId = userId };
        UserViewModel userVM = new() { User = new UserDTO { Id = userId, FirstName = userFName, LastName = userLName, UserName = userName, Email = userEmail } };

        // Act
        A.CallTo(() => _getUserQueryHandler.GetItemAsync(getUserQuery)).Returns(userVM);

        var controller = GetUserController();
        controller.UpdateContext(null);
        var actionResult = await controller.GetUser(userId);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        UserViewModel? vm = result?.Value as UserViewModel;

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

        Assert.NotNull(vm);
        Assert.Equal(userId, vm?.User?.Id);
        Assert.Equal(userFName, vm?.User?.FirstName);
        Assert.Equal(userLName, vm?.User?.LastName);
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
        string userId = Guid.NewGuid().ToString();
        string userFName = "Test";
        string userLName = "User";
        string userName = "test.user";
        string userEmail = userName + "@test.com";
        string userPassword = "testPassword";

        string refreshToken = "abmsmmsrefreshToken";

        RegisterUserCommand command = new() { FirstName = userFName, LastName = userLName, UserName = userName, UserEmail = userEmail, UserPassword = userPassword };

        AuthUserViewModel authVM = new() { User = new UserDTO { Id = userId, FirstName = userFName, LastName = userLName, UserName = userName, Email = userEmail }, UserToken = new AccessTokenViewModel { RefreshToken = refreshToken } };

        // Act
        A.CallTo(() => _registerUserCommandHandler.CreateItemAsync(command, TestStringHelper.UserId)).Returns(authVM);

        var controller = GetUserController();
        controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
        var actionResult = await controller.CreateUser(command);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        AuthUserViewModel? vm = result?.Value as AuthUserViewModel;

        // Assert
        Assert.Equal((int)HttpStatusCode.Created, result?.StatusCode);

        Assert.NotNull(vm);
        Assert.Equal(userId, vm?.User?.Id);
        Assert.Equal(userFName, vm?.User?.FirstName);
        Assert.Equal(userLName, vm?.User?.LastName);
        Assert.Equal(userName, vm?.User?.UserName);
        Assert.Equal(userEmail, vm?.User?.Email);


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
        string userName = "test.user";
        string userPassword = "-test.user*1+";

        string userId = Guid.NewGuid().ToString();
        string userFName = "Test";
        string userLName = "User";
        string userEmail = userName + "@test.com";

        string refreshToken = "abmsmmsrefreshToken";
        string loginMessage = "Login successful";

        LoginUserCommand command = new() { UserName = userName, Password = userPassword };
        AuthUserViewModel authVM = new() { User = new UserDTO { Id = userId, FirstName = userFName, LastName = userLName, UserName = userName, Email = userEmail }, UserToken = new AccessTokenViewModel { RefreshToken = refreshToken } };

        // Act
        A.CallTo(() => _loginUserCommandHandler.CreateItemAsync(command, TestStringHelper.UserId)).Returns(authVM);

        var controller = GetUserController();
        controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
        var actionResult = await controller.LoginUser(command);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        AuthUserViewModel? vm = result?.Value as AuthUserViewModel;

        // Assert
        Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);

        Assert.NotNull(vm);
        Assert.Equal(userId, vm?.User?.Id);
        Assert.Equal(userFName, vm?.User?.FirstName);
        Assert.Equal(userLName, vm?.User?.LastName);
        Assert.Equal(userName, vm?.User?.UserName);
        Assert.Equal(userEmail, vm?.User?.Email);
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
        string accessToken = "Acccess Token This";
        string refreshToken = "Refresh Token This";

        string newAccesstoken = "New Access Token This";
        string newRefreshToken = "New Refresh Token This";

        string sucessStatusMessage = "Refresh token generated successfully";

        ExchangeRefreshTokenViewModel exchangeRefreshTokenVM = new() { UserToken = new AccessTokenViewModel { AccessToken = new AccessTokenDTO { Token = newAccesstoken }, RefreshToken = newRefreshToken } };

        ExchangeRefreshTokenCommand command = new() { AccessToken = accessToken, RefreshToken = refreshToken };

        // Act
        A.CallTo(() => _exchangeRefreshTokenCommandHandler.UpdateItemAsync(command, TestStringHelper.UserId)).Returns(exchangeRefreshTokenVM);

        var controller = GetUserController();
        controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
        var actionResult = await controller.RefreshToken(command);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        ExchangeRefreshTokenViewModel? vm = result?.Value as ExchangeRefreshTokenViewModel;

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
        string userEmail = "user-email123@server.com";

        string emailMessage = "Password reset email sent successfully";
        string resetMessage = "Password reset request successful";

        ResetPasswordCommand command = new() { UserEmail = userEmail };
        ResetPasswordViewModel resetPasswordVM = new() { ResetPasswordDetails = new ResetPasswordDTO { EmailMessage = emailMessage } };

        // Act
        A.CallTo(() => _resetPasswordCommandHandler.CreateItemAsync(command, TestStringHelper.UserId)).Returns(resetPasswordVM);

        var controller = GetUserController();
        controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
        var actionResult = await controller.ResetPassword(command);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        ResetPasswordViewModel? vm = result?.Value as ResetPasswordViewModel;

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
        string confirmUpdatePasswordToken = "ConfirmUpdatePasswordToken";
        string userId = Guid.NewGuid().ToString();

        ConfirmUpdatePasswordTokenCommand command = new() { ConfirmPasswordToken = confirmUpdatePasswordToken, UserId = userId };
        ConfirmUpdatePasswordTokenViewModel confirmUpdatePasswordTokenVM = new() { TokenPasswordResult = new ConfirmUpdatePasswordDTO { UpdatePasswordTokenConfirmed = true } };

        // Act
        A.CallTo(() => _confirmUpdatePasswordTokenCommandHandler.UpdateItemAsync(command, TestStringHelper.UserId)).Returns(confirmUpdatePasswordTokenVM);

        var controller = GetUserController();
        controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
        var actionResult = await controller.ConfirmPasswordToken(command);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        ConfirmUpdatePasswordTokenViewModel? vm = result?.Value as ConfirmUpdatePasswordTokenViewModel;

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
        string userId = Guid.NewGuid().ToString();
        string newPassword = "NewPassword123";
        string passwordresetToken = "PasswordResetToken123";

        UpdatePasswordCommand command = new() { UserId = userId, NewPassword = newPassword, PasswordResetToken = passwordresetToken };
        UpdatePasswordViewModel updatePasswordViewModel = new() { UpdatePasswordResult = new UpdatePasswordDTO { PassWordUpdated = true } };

        // Act
        A.CallTo(() => _updatePasswordCommandHandler.UpdateItemAsync(command, TestStringHelper.UserId)).Returns(updatePasswordViewModel);

        var controller = GetUserController();
        controller.UpdateContext(Controllername, addUserId: true, userId: TestStringHelper.UserId);
        var actionResult = await controller.UpdatePassword(command);
        ObjectResult? result = actionResult?.Result as ObjectResult;
        UpdatePasswordViewModel? vm = result?.Value as UpdatePasswordViewModel;

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
            _updatePasswordCommandHandler,
            _getRegisteredAppQueryHandler
        );

    }

}
