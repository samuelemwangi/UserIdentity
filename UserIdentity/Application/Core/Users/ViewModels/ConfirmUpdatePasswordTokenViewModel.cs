namespace UserIdentity.Application.Core.Users.ViewModels
{
	public record ConfirmUpdatePasswordDTO
	{
		public bool UpdatePasswordTokenConfirmed { get; init; }
	}

	public record ConfirmUpdatePasswordTokenViewModel : BaseViewModel
	{
		public ConfirmUpdatePasswordDTO TokenPasswordResult { get; init; }

	}
}

