using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Configurations;

using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.ResetPassword;

public record ResetPasswordCommand : IBaseCommand
{
	[Required]
	[EmailAddress]
	public string UserEmail { get; init; } = null!;
}

public class ResetPasswordCommandHandler(
	UserManager<IdentityUser> userManager,
	IUserRepository userRepository,
	IConfiguration configuration
	) : ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel>
{
	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IConfiguration _configuration = configuration;

	public async Task<ResetPasswordViewModel> CreateItemAsync(ResetPasswordCommand command, string userId)
	{
		// Check if default message is set in configs
		string resetPasswordMessageKey = _configuration.GetEnvironmentVariable("DefaultResetPasswordMessage", "Kindly check your email for instructions to reset your password");
		string resetPasswordMessage = _configuration.GetEnvironmentVariable(resetPasswordMessageKey, resetPasswordMessageKey);

		var vm = new ResetPasswordViewModel
		{
			ResetPasswordDetails = new ResetPasswordDTO
			{
				EmailMessage = resetPasswordMessage
			}
		};

		var existingUser = await _userManager.FindByEmailAsync(command.UserEmail);

		if (existingUser == null)
		{
			return vm;
		}

		// generate token
		string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

		// update user details record
		var updateResult = await _userRepository.UpdateResetPasswordTokenAsync(existingUser.Id, resetPasswordToken);

		if (updateResult < 1)
			throw new RecordUpdateException(command.UserEmail, "User");

		string emailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));

		Console.WriteLine(emailToken);

		// Send Email Logic here

		return vm;

	}
}
