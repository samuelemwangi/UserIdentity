using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.ConfirmUpdatePasswordToken
{
	public record ConfirmUpdatePasswordTokenCommand : BaseCommand
	{
		[Required]
		public String ConfirmPasswordToken { get; init; }
		[Required]
		public String UserId { get; init; }
	}
	public class ConfirmUpdatePasswordTokenCommandHandler : IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel>
	{
		private readonly IUserRepository _userRepository;

		public ConfirmUpdatePasswordTokenCommandHandler(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}


		public async Task<ConfirmUpdatePasswordTokenViewModel> UpdateItemAsync(ConfirmUpdatePasswordTokenCommand command)
		{
			try
			{
				String rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.ConfirmPasswordToken));
				return new ConfirmUpdatePasswordTokenViewModel
				{
					TokenPasswordResult = new ConfirmUpdatePasswordDTO
					{
						UpdatePasswordTokenConfirmed = await _userRepository.ValidateUpdatePasswordTokenAsync(command.UserId, rawToken)
					}
				};

			}
			catch (Exception)
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

