using System;
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
		private static String Controllername = "user";

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
			var userEmail = "test.user@test.com";
			var getUserQuery = new GetUserQuery { UserId = userId };
			var userVM = new UserViewModel { User = new UserDTO { Id = userId, FullName = userFName + userLName, UserName = username, Email = userEmail } };

			// Act
			A.CallTo(() => _getUserQueryHandler.GetItemAsync(getUserQuery)).Returns(userVM);

			var controller = GetUserController();
			controller.UpdateContext(Controllername);
			var actionResult = await controller.GetUser(userId);
			var result = actionResult?.Result as OkObjectResult;
			var vm = result?.Value as UserViewModel;

			// Assert
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
