using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.UserRegisteredApps;

public interface IUserRegisteredAppRepository : IEntityRepository<UserRegisteredAppEntity, string>
{
}
