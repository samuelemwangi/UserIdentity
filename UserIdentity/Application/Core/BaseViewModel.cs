namespace UserIdentity.Application.Core
{
	public abstract record BaseViewModel
	{
		public String? RequestStatus { get; internal set; }
		public String? StatusMessage { get; internal set; }
	}
}
