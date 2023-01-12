using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.ResetPassword
{
    public record ResetPasswordCommand : BaseCommand
    {
        [Required]
        [EmailAddress]
        public String UserEmail { get; init; }
    }

    public class ResetPasswordCommandHandler
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public ResetPasswordCommandHandler(UserManager<IdentityUser> userManager, IUserRepository userRepository, IConfiguration configuration)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<ResetPasswordViewModel> ResetPassword(ResetPasswordCommand command)
        {
            var existingUser = await _userManager.FindByEmailAsync(command.UserEmail);

            // Check if default message is set in configs
            String defaultResetPasswordMessage = _configuration.GetValue<String>("DefaultResetPasswordMessage");
            String emailMessage = defaultResetPasswordMessage ?? "Kindly check your email for instructions to reset your password";

            if (existingUser == null)
                throw new NoRecordException(emailMessage);

            // generate token
            String resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

            // update user details record
            var updateResult =  await _userRepository.UpdateResetPasswordTokenAsync(existingUser.Id, resetPasswordToken);

            if (updateResult < 1)
                throw new RecordUpdateException(emailMessage);

            // Send Email Logic here
            String emailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));

            return new ResetPasswordViewModel
            {
                ResetPasswordDetails = new ResetPasswordDTO
                {
                    EmailMessage = emailMessage + " ----- " + emailToken
                }
            };

        }
    }
}
