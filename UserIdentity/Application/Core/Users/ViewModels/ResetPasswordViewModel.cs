namespace UserIdentity.Application.Core.Users.ViewModels
{
	public record ResetPasswordDTO
	{
		public string EmailMessage { get; init; }
	}
	public record ResetPasswordViewModel : BaseViewModel
	{
		public ResetPasswordDTO ResetPasswordDetails { get; init; }
	}
}

