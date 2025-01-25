using System;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Users.Commands.UpdatePassword;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.UnitTests.TestUtils;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Commands
{
	public class UpdatePasswordCommandHandlerTests
	{

		private readonly UserManager<IdentityUser> _userManager;

		public UpdatePasswordCommandHandlerTests()
		{

			_userManager = A.Fake<UserManager<IdentityUser>>();

		}

		[Fact]
		public async Task UpdatePassword_When_No_Existing_User_Returns_False()
		{
			// Arrange
			var command = new UpdatePasswordCommand
			{
				NewPassword = "test",
				UserId = "test",
				PasswordResetToken = "test"
			};

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(default(IdentityUser));

			var handler = new UpdatePasswordCommandHandler(_userManager);

			// Act 
			var vm = await handler.UpdateItemAsync(command, TestStringHelper.UserId);

			// Assert
			Assert.IsType<UpdatePasswordViewModel>(vm);
			Assert.NotNull(vm.UpdatePasswordResult);
			Assert.False(vm.UpdatePasswordResult.PassWordUpdated);
		}

		[Fact]
		public async Task UpdatePassword_With_Password_Reset_Failure_Returns_False()
		{
			// Arrange
			var command = new UpdatePasswordCommand
			{
				NewPassword = "test",
				UserId = "test",
				PasswordResetToken = "test"
			};

			var existingIdentityUser = new IdentityUser
			{
				Id = command.UserId,
				Email = "test@ml.clm"
			};

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(existingIdentityUser);
			A.CallTo(() => _userManager.ResetPasswordAsync(existingIdentityUser, command.PasswordResetToken, command.NewPassword)).Returns(default(IdentityResult));

			var handler = new UpdatePasswordCommandHandler(_userManager);

			// Act 
			var vm = await handler.UpdateItemAsync(command, TestStringHelper.UserId);

			// Assert
			Assert.IsType<UpdatePasswordViewModel>(vm);
			Assert.NotNull(vm.UpdatePasswordResult);
			Assert.False(vm.UpdatePasswordResult.PassWordUpdated);
		}

		[Fact]
		public async Task UpdatePassword_With_Valid_Details_Returns_True()
		{
			// Arrange
			var command = new UpdatePasswordCommand
			{
				NewPassword = "test",
				UserId = "test",
				PasswordResetToken = "test"
			};

			var existingIdentityUser = new IdentityUser
			{
				Id = command.UserId,
				Email = "test@ml.clm"
			};

			A.CallTo(() => _userManager.FindByIdAsync(command.UserId)).Returns(existingIdentityUser);


			A.CallTo(() => _userManager.ResetPasswordAsync(existingIdentityUser, A<string>.Ignored, command.NewPassword)).Returns(IdentityResult.Success);

			var handler = new UpdatePasswordCommandHandler(_userManager);

			// Act 
			var vm = await handler.UpdateItemAsync(command, TestStringHelper.UserId);

			// Assert
			Assert.IsType<UpdatePasswordViewModel>(vm);
			Assert.NotNull(vm.UpdatePasswordResult);
			Assert.True(vm.UpdatePasswordResult.PassWordUpdated);
		}
	}
}
