﻿using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Application.Exceptions;
using UserIdentity.Persistence.Repositories.Users;

namespace UserIdentity.Application.Core.Users.Commands.ResetPassword
{
	public record ResetPasswordCommand : BaseCommand
	{
		[Required]
		[EmailAddress]
		public string UserEmail { get; init; }
	}

	public class ResetPasswordCommandHandler : ICreateItemCommandHandler<ResetPasswordCommand, ResetPasswordViewModel>
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

		public async Task<ResetPasswordViewModel> CreateItemAsync(ResetPasswordCommand command)
		{
			var existingUser = await _userManager.FindByEmailAsync(command.UserEmail);

			// Check if default message is set in configs
			string defaultResetPasswordMessage = _configuration.GetValue<string>(_configuration.GetValue<string>("DefaultResetPasswordMessage"));
			string emailMessage = defaultResetPasswordMessage ?? "Kindly check your email for instructions to reset your password";

			if (existingUser == null)
				throw new NoRecordException(command.UserEmail, "User");

			// generate token
			string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

			// update user details record
			var updateResult = await _userRepository.UpdateResetPasswordTokenAsync(existingUser.Id, resetPasswordToken);

			if (updateResult < 1)
				throw new RecordUpdateException(command.UserEmail, "User");

			// Send Email Logic here
			string emailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));

			return new ResetPasswordViewModel
			{
				ResetPasswordDetails = new ResetPasswordDTO
				{
					EmailMessage = emailMessage
				}
			};

		}
	}
}
