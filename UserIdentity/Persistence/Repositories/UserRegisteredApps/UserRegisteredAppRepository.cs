using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.UserRegisteredApps;

public class UserRegisteredAppRepository(
    AppDbContext dbContext
    ) : EntityRepository<UserRegisteredAppEntity, string>(dbContext),
            IUserRegisteredAppRepository
{
}
