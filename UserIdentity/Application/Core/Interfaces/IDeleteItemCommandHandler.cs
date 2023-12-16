namespace UserIdentity.Application.Core.Interfaces
{
	public interface IDeleteItemCommandHandler<TCommand, TResult>
	{
		public Task<TResult> DeleteItemAsync(TCommand command);
	}
}
