namespace UserIdentity.Application.Core.Interfaces
{
    public interface IUpdateItemCommandHandler<TCommand, TResult>
    {
        public Task<TResult> UpdateItemAsync(TCommand command);
    }
}

