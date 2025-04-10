using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;

using UserIdentity.Application.Core.RegisteredApps.ViewModels;
using UserIdentity.Persistence.Repositories.RegisteredApps;

namespace UserIdentity.Application.Core.RegisteredApps.Queries;

public record GetRegisteredAppsQuery : IBaseQuery
{
}

public class GetRegisteredAppsQueryHandler(
	IRegisteredAppRepository registeredAppRepository
	) : IGetItemQueryHandler<GetRegisteredAppsQuery, RegisteredAppsViewModel>
{
	private readonly IRegisteredAppRepository _registeredAppRepository = registeredAppRepository;
	public async Task<RegisteredAppsViewModel> GetItemAsync(GetRegisteredAppsQuery query)
	{
		return new RegisteredAppsViewModel
		{
			RegisteredApps = await _registeredAppRepository.GetEntityDTOsAsync()
		};
	}
}
