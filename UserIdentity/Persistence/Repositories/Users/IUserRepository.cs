using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.Users;

public interface IUserRepository
    : IEntityRepository<UserEntity, string>,
    IItemEntityRepository<UserEntity, string>
{
}
