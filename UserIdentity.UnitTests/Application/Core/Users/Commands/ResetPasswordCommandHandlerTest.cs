using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using UserIdentity.Persistence.Repositories.Users;

using Xunit;
using FakeItEasy;
using UserIdentity.Application.Core.Users.Commands.ResetPassword;
using UserIdentity.Application.Exceptions;
using UserIdentity.Application.Core.Users.ViewModels;

namespace UserIdentity.UnitTests.Application.Core.Users.Commands
{
    public class ResetPasswordCommandHandlerTest : IClassFixture<TestSettingsFixture>
    {
        private readonly TestSettingsFixture _testSettings;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public ResetPasswordCommandHandlerTest(TestSettingsFixture testSettings)
        {
            _testSettings = testSettings;

            _userManager = A.Fake<UserManager<IdentityUser>>();
            _userRepository = A.Fake<IUserRepository>();
            _configuration = _testSettings.Configuration;
        }

        [Fact]
        public async Task ResetPassword_When_No_Existing_Registered_Email_Throws_NoRecordException()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                UserEmail = "test@kl.com"
            };

            A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(default(IdentityUser));

            var handler = new ResetPasswordCommandHandler(_userManager, _userRepository, _configuration);

            // Act & Assert
            await Assert.ThrowsAsync<NoRecordException>(() => handler.CreateItemAsync(command));
        }

        [Fact]
        public async Task ResetPassword_With_Failure_Updating_User_Throws_RecordUpdateException()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                UserEmail = "test@lp.com"
            };

            var existingIdentityUser = new IdentityUser
            {  
				Id = Guid.NewGuid().ToString(),
                Email = command.UserEmail,
				UserName = command.UserEmail
            };

			var resetPassWordToken = "sampleresetPasswordToken";

            A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(existingIdentityUser);
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(existingIdentityUser)).Returns(resetPassWordToken);
			A.CallTo(() => _userRepository.UpdateResetPasswordTokenAsync(existingIdentityUser.Id, resetPassWordToken)).Returns(0);

			var handler = new ResetPasswordCommandHandler(_userManager, _userRepository, _configuration);

			// Act & Assert
			await Assert.ThrowsAsync<RecordUpdateException>(() => handler.CreateItemAsync(command));
        }

		[Fact]
        public async Task ResetPassword_With_Valid_Details_Creates_ResetPassword_Details()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                UserEmail = "test@lp.com"
            };

            var existingIdentityUser = new IdentityUser
            {  
				Id = Guid.NewGuid().ToString(),
                Email = command.UserEmail,
				UserName = command.UserEmail
            };

			var resetPassWordToken = "sampleresetPasswordToken";

            A.CallTo(() => _userManager.FindByEmailAsync(command.UserEmail)).Returns(existingIdentityUser);
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(existingIdentityUser)).Returns(resetPassWordToken);
			A.CallTo(() => _userRepository.UpdateResetPasswordTokenAsync(existingIdentityUser.Id, resetPassWordToken)).Returns(1);

			var handler = new ResetPasswordCommandHandler(_userManager, _userRepository, _configuration);

			// Act 
			var vm = await handler.CreateItemAsync(command);

			// Assert
			Assert.IsType<ResetPasswordViewModel>(vm);		
			Assert.NotNull(vm.ResetPasswordDetails);	
        }


    }
}
