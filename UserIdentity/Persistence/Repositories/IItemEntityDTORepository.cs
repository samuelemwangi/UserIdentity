using PolyzenKit.Domain.DTO;

namespace UserIdentity.Persistence.Repositories;

public interface IItemEntityDTORepository<TEntityDTO, T> where TEntityDTO : IBaseEntityDTO<T>
{
	Task<TEntityDTO> GetEntityDTOByIdAsync(T id);
}
