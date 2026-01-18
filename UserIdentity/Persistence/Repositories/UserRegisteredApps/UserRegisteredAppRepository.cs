using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.UserRegisteredApps;

namespace UserIdentity.Persistence.Repositories.UserRegisteredApps;

public class UserRegisteredAppRepository(
    AppDbContext dbContext
 ) : EntityRepository<UserRegisteredAppEntity, string>(dbContext), IUserRegisteredAppRepository
{
}
