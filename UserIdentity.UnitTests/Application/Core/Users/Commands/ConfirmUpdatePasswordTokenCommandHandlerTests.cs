using System.Text;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Application.Core.Users.Commands.ConfirmUpdatePasswordToken;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Persistence.Repositories.Users;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Commands
{
	public class ConfirmUpdatePasswordTokenCommandHandlerTests
	{
		private readonly IUserRepository _userRepository;

		public ConfirmUpdatePasswordTokenCommandHandlerTests()
		{
			_userRepository = A.Fake<IUserRepository>();
		}

		[Fact]
		public async Task Confirm_UpdatePassword_Token_With_Bad_Token_Returns_False()
		{
			// Arrange
			var command = new ConfirmUpdatePasswordTokenCommand
			{
				ConfirmPasswordToken = "test",
				UserId = "test"
			};

			var handler = new ConfirmUpdatePasswordTokenCommandHandler(_userRepository);

			// Act
			var vm = await handler.UpdateItemAsync(command);

			// Assert
			Assert.IsType<ConfirmUpdatePasswordTokenViewModel>(vm);
			Assert.NotNull(vm.TokenPasswordResult);
			Assert.False(vm.TokenPasswordResult.UpdatePasswordTokenConfirmed);

		}

		[Fact]
		public async Task Confirm_UpdatePassword_Token_With_Non_Existing_Token_Returns_False()
		{
			// Arrange
			var rawToken = "test123+*";

			var command = new ConfirmUpdatePasswordTokenCommand
			{
				ConfirmPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken)),
				UserId = "test"
			};

			A.CallTo(() => _userRepository.ValidateUpdatePasswordTokenAsync(command.UserId, rawToken)).Returns(false);

			var handler = new ConfirmUpdatePasswordTokenCommandHandler(_userRepository);

			// Act
			var vm = await handler.UpdateItemAsync(command);

			// Assert
			Assert.IsType<ConfirmUpdatePasswordTokenViewModel>(vm);
			Assert.NotNull(vm.TokenPasswordResult);
			Assert.False(vm.TokenPasswordResult.UpdatePasswordTokenConfirmed);
		}

		[Fact]
		public async Task Confirm_UpdatePassword_Token_With_Query_Token_Exception_Returns_False()
		{
			// Arrange
			var rawToken = "test123+*";

			var command = new ConfirmUpdatePasswordTokenCommand
			{
				ConfirmPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken)),
				UserId = "test"
			};

			A.CallTo(() => _userRepository.ValidateUpdatePasswordTokenAsync(command.UserId, rawToken)).Throws(new System.Exception());

			var handler = new ConfirmUpdatePasswordTokenCommandHandler(_userRepository);

			// Act
			var vm = await handler.UpdateItemAsync(command);

			// Assert
			Assert.IsType<ConfirmUpdatePasswordTokenViewModel>(vm);
			Assert.NotNull(vm.TokenPasswordResult);
			Assert.False(vm.TokenPasswordResult.UpdatePasswordTokenConfirmed);
		}

		[Fact]
		public async Task Confirm_UpdatePassword_Token_With_Existing_Token_Returns_True()
		{
			// Arrange
			var rawToken = "test123+*";

			var command = new ConfirmUpdatePasswordTokenCommand
			{
				ConfirmPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken)),
				UserId = "test"
			};

			A.CallTo(() => _userRepository.ValidateUpdatePasswordTokenAsync(command.UserId, rawToken)).Returns(true);

			var handler = new ConfirmUpdatePasswordTokenCommandHandler(_userRepository);

			// Act
			var vm = await handler.UpdateItemAsync(command);

			// Assert
			Assert.IsType<ConfirmUpdatePasswordTokenViewModel>(vm);
			Assert.NotNull(vm.TokenPasswordResult);
			Assert.True(vm.TokenPasswordResult.UpdatePasswordTokenConfirmed);
		}
	}
}
