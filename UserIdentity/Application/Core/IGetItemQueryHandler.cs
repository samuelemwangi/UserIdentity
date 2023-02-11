namespace UserIdentity.Application.Core
{
	public interface IGetItemQueryHandler<Query, VM>
	{
		public Task<VM> GetITemAsync(Query query);
	}
}
