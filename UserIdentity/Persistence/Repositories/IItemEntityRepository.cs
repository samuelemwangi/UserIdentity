using PolyzenKit.Domain.Entity;

namespace UserIdentity.Persistence.Repositories;

public interface IItemEntityRepository<TEntity, T> where TEntity : BaseEntity<T>
{
	Task<TEntity?> GetEntityByAlternateIdAsync(TEntity entity, QueryCondition queryCondition);
}
