using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.WaitLists;

namespace UserIdentity.Persistence.Repositories.WaitLists;

public class WaitListRepository(
  AppDbContext dbContext
) : EntityRepository<WaitListEntity, long>(dbContext), IWaitListRepository
{
}
