namespace UserIdentity.Application.Core
{
	public abstract record ItemDetailBaseViewModel : BaseViewModel
	{
		public bool EditEnabled { get; internal set; }
		public bool DeleteEnabled { get; internal set; }

	}
}
