using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Users.Commands.UpdatePassword
{
	public record UpdatePasswordCommand : BaseCommand
	{
		[Required]
		public String NewPassword { get; init; }

		[Required]
		public String UserId { get; init; }

		[Required]
		public String PasswordResetToken { get; init; }
	}

	public class UpdatePasswordCommandHandler : IUpdateItemCommandHandler<UpdatePasswordCommand, UpdatePasswordViewModel>
	{
		private readonly UserManager<IdentityUser> _userManager;

		public UpdatePasswordCommandHandler(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<UpdatePasswordViewModel> UpdateItemAsync(UpdatePasswordCommand command)
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

				return new UpdatePasswordViewModel
				{
					UpdatePasswordResult = new UpdatePasswordDTO
					{
						PassWordUpdated = result
					}
				};


			}
			catch (Exception)
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

