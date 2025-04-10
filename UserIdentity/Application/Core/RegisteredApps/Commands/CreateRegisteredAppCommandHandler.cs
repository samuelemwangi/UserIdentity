
using AutoMapper;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Domain.Entity;

using UserIdentity.Application.Core.RegisteredApps.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories;
using UserIdentity.Persistence.Repositories.RegisteredApps;

namespace UserIdentity.Application.Core.RegisteredApps.Commands;

public record CreateRegisteredAppCommand : RegisteredAppEntity, IBaseCommand
{

}

public class CreateRegisteredAppCommandHandler(
	IRegisteredAppRepository registeredAppRepository,
	IMapper mapper,
	IMachineDateTime machineDateTime
	) : ICreateItemCommandHandler<CreateRegisteredAppCommand, RegisteredAppViewModel>
{
	private readonly IRegisteredAppRepository _registeredAppRepository = registeredAppRepository;
	private readonly IMapper _mapper = mapper;
	private readonly IMachineDateTime _machineDateTime = machineDateTime;
	public async Task<RegisteredAppViewModel> CreateItemAsync(CreateRegisteredAppCommand command, string userId)
	{
		await _registeredAppRepository.GetEntityByAlternateIdAsync(new RegisteredAppEntity { AppName = command.AppName }, QueryCondition.MUST_NOT_EXIST);

		var entity = _mapper.Map<RegisteredAppEntity>(command);

		entity.SetEntityAuditFields(userId, _machineDateTime.Now);

		await _registeredAppRepository.CreateEntityItemAsync(entity);

		return new RegisteredAppViewModel
		{
			RegisteredApp = _mapper.Map<RegisteredAppDTO>(entity)
		};
	}
}
