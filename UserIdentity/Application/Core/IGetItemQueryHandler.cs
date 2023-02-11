namespace UserIdentity.Application.Core
{
	public interface IGetItemQueryHandler<Query, VM>
	{
		public Task<VM> GetItemAsync(Query query);
	}
}
