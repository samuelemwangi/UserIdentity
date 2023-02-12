namespace UserIdentity.Application.Core
{
	public interface IGetItemQueryHandler<TQuery, TResult>
	{
		public Task<TResult> GetItemAsync(TQuery query);
	}
}
