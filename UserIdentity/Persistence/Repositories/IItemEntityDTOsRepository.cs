using PolyzenKit.Domain.DTO;

namespace UserIdentity.Persistence.Repositories;

public interface IItemEntityDTOsRepository<TEntityDTO, T> where TEntityDTO : IBaseEntityDTO<T>
{
	Task<List<TEntityDTO>> GetEntityDTOsAsync();
}
