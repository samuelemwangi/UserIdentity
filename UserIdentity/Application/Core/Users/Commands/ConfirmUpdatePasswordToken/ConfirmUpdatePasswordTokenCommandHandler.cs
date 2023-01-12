using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.ConfirmUpdatePasswordToken
{
    public record ConfirmUpdatePasswordTokenCommand : BaseCommand
    {
        public String ConfirmPasswordToken { get; init; }
        public String UserId { get; init; }
    }
    public class ConfirmUpdatePasswordTokenCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public ConfirmUpdatePasswordTokenCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<ConfirmUpdatePasswordTokenViewModel> ValidateUpdatePasswordToken(ConfirmUpdatePasswordTokenCommand command)
        {
            try
            {
                String rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.ConfirmPasswordToken));
                return new ConfirmUpdatePasswordTokenViewModel
                {
                    TokenPasswordResult = new ConfirmUpdatePasswordDTO
                    {
                        UpdatePasswordTokenConfirmed = await _userRepository.ValidateUpdatePasswordTokenAsync(rawToken, command.UserId)
                    }
                };

            }
            catch (Exception ex)
            {
                return new ConfirmUpdatePasswordTokenViewModel
                {
                    TokenPasswordResult = new ConfirmUpdatePasswordDTO
                    {
                        UpdatePasswordTokenConfirmed = false
                    }
                };

            }
        }
    }
}

