using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.WebUtilities;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands;

public record ConfirmUpdatePasswordTokenCommand : IBaseCommand
{
	[Required]
	public string ConfirmPasswordToken { get; init; } = null!;

	[Required]
	public string UserId { get; init; } = null!;
}
public class ConfirmUpdatePasswordTokenCommandHandler(
	IUserRepository userRepository
	) : IUpdateItemCommandHandler<ConfirmUpdatePasswordTokenCommand, ConfirmUpdatePasswordTokenViewModel>
{
	private readonly IUserRepository _userRepository = userRepository;

	public async Task<ConfirmUpdatePasswordTokenViewModel> UpdateItemAsync(ConfirmUpdatePasswordTokenCommand command, string userId)
	{
		try
		{
			string rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.ConfirmPasswordToken));

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

