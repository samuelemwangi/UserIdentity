namespace UserIdentity.Application.Core
{
	public interface ICreateItemCommandHandler<TCommand, TResult>
	{
		public Task<TResult> CreateItemAsync(TCommand command);
	}
}
