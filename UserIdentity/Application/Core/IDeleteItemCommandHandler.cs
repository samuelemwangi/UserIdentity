namespace UserIdentity.Application.Core
{
	public interface IDeleteItemCommandHandler<TCommand, TResult>
	{
		public Task<TResult> DeleteItemAsync(TCommand command);
	}
}
