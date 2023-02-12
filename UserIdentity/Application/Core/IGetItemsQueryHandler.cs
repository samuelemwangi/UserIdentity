namespace UserIdentity.Application.Core
{
	public interface IGetItemsQueryHandler<TQuery, TResult>
	{
		public Task<TResult> GetItemsAsync(TQuery query);
	}
}
