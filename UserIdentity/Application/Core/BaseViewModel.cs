namespace UserIdentity.Application.Core
{
	public abstract record BaseViewModel
	{
		public string? RequestStatus { get; internal set; }
		public string? StatusMessage { get; internal set; }
	}
}
