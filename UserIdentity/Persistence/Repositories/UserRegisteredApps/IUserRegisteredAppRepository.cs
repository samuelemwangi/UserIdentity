using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.UserRegisteredApps;

namespace UserIdentity.Persistence.Repositories.UserRegisteredApps;

public interface IUserRegisteredAppRepository : IEntityRepository<UserRegisteredAppEntity, string>
{
}
