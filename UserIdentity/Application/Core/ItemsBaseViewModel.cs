namespace UserIdentity.Application.Core
{
	public abstract record ItemsBaseViewModel : BaseViewModel
	{
		public bool CreateEnabled { get; internal set; }
		public bool DownloadEnabled { get; internal set; }
	}
}
