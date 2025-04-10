using PolyzenKit.Persistence.Repositories;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.RegisteredApps;

public interface IRegisteredAppRepository
	: IEntityRepository<RegisteredAppEntity, int>,
	IItemEntityRepository<RegisteredAppEntity, int>,
	IItemEntityDTOsRepository<RegisteredAppDTO, int>
{
}
