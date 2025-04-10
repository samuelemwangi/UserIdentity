
using AutoMapper;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Attributes;
using PolyzenKit.Application.Core.Interfaces;

using UserIdentity.Application.Core.RegisteredApps.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories;
using UserIdentity.Persistence.Repositories.RegisteredApps;

namespace UserIdentity.Application.Core.RegisteredApps.Queries;

public record GetRegisteredAppQuery : IBaseQuery
{
	[EitherOr(nameof(Id), nameof(AppName))]
	public int? Id { get; set; }

	[EitherOr(nameof(Id), nameof(AppName))]
	public string? AppName { get; set; }
}

public class GetRegisteredAppQueryHandler(
	IRegisteredAppRepository registeredAppRepository,
	IMapper mapper
	) : IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel>
{
	private readonly IRegisteredAppRepository _registeredAppRepository = registeredAppRepository;
	private readonly IMapper _mapper = mapper;
	public async Task<RegisteredAppViewModel> GetItemAsync(GetRegisteredAppQuery query)
	{
		var existingEntity = query.Id.HasValue
			? await _registeredAppRepository.GetEntityItemAsync(query.Id.Value)
			: await _registeredAppRepository.GetEntityByAlternateIdAsync(new RegisteredAppEntity { AppName = query.AppName! }, QueryCondition.MUST_EXIST);

		return new RegisteredAppViewModel
		{
			RegisteredApp = _mapper.Map<RegisteredAppDTO>(existingEntity)
		};
	}
}
