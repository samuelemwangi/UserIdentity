namespace UserIdentity.Application.Core.Interfaces
{
  public interface IGetItemsQueryHandler<TQuery, TResult>
  {
    public Task<TResult> GetItemsAsync(TQuery query);
  }
}
