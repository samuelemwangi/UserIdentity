using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Persistence.Repositories.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UserIdentity.Application.Core.Users.Commands.UpdatePassword
{
    public record UpdatePasswordCommand : BaseCommand
    {
        public String NewPassword { get; init; }
        public String UserId { get; init; }
        public String PasswordResetToken { get; init; }
    }

    public class UpdatePasswordCommandHandler
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UpdatePasswordCommandHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UpdatePasswordViewModel> UpdatePassWord(UpdatePasswordCommand command)
        {

            try
            {

                var userDetails = await _userManager.FindByIdAsync(command.UserId);

                if (userDetails == null)
                    throw new NoRecordException(command.UserId + "", "User");

                String rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.PasswordResetToken));

                var reSetPassWordTokenresult = await _userManager.ResetPasswordAsync(userDetails, rawToken, command.NewPassword);


                Boolean result = true;

                if (!reSetPassWordTokenresult.Succeeded)
                    result = false;

                Console.WriteLine("\n\n\n" + reSetPassWordTokenresult+"\n\n\n\n");
                Console.WriteLine("Hello");

                return new UpdatePasswordViewModel
                {
                    UpdatePasswordResult = new UpdatePasswordDTO
                    {
                        PassWordUpdated = result
                    }
                };


            }
            catch (Exception ex)
            {
                return new UpdatePasswordViewModel
                {
                    UpdatePasswordResult = new UpdatePasswordDTO
                    {
                        PassWordUpdated = false
                    }
                };

            }


        }

    }
}

