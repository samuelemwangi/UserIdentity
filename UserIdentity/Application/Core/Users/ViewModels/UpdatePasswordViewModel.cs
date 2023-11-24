namespace UserIdentity.Application.Core.Users.ViewModels
{
	public record UpdatePasswordDTO
	{
		public Boolean PassWordUpdated { get; init; }

	}

	public record UpdatePasswordViewModel : BaseViewModel
	{
		public UpdatePasswordDTO UpdatePasswordResult { get; init; }
	}
}

