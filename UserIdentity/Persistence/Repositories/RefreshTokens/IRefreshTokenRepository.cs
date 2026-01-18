using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.RefreshTokens;

namespace UserIdentity.Persistence.Repositories.RefreshTokens;

public interface IRefreshTokenRepository
    : IEntityRepository<RefreshTokenEntity, Guid>,
    IItemEntityRepository<RefreshTokenEntity, Guid>
{
}
