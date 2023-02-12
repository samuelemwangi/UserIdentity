namespace UserIdentity.Application.Core
{
	public interface IGetItemsQueryHandler<Query, VM>
	{
		public Task<VM> GetItemsAsync(Query query);
	}
}
