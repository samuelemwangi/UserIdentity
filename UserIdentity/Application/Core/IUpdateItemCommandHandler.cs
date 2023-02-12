namespace UserIdentity.Application.Core
{
	public interface IUpdateItemCommandHandler<TCommand, TResult>
	{
		public Task<TResult> UpdateItemAsync(TCommand command);
	}
}

