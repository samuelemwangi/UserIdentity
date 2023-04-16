namespace UserIdentity.Application.Core.Interfaces
{
    public interface ICreateItemCommandHandler<TCommand, TResult>
    {
        public Task<TResult> CreateItemAsync(TCommand command);
    }
}
