namespace UserIdentity.Application.Core.Interfaces
{
  public interface IGetItemQueryHandler<TQuery, TResult>
  {
    public Task<TResult> GetItemAsync(TQuery query);
  }
}
