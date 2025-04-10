using System.ComponentModel.DataAnnotations;

using AutoMapper;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Domain.Entity;

using UserIdentity.Application.Core.RegisteredApps.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.RegisteredApps;

namespace UserIdentity.Application.Core.RegisteredApps.Commands;

public record UpdateRegisteredAppCommand : IBaseCommand
{
	[Required]
	public int Id { get; internal set; }

	[StringLength(100)]
	public string? AppName { get; set; }

	[StringLength(100)]
	public string? AppSecretKey { get; set; }

	[StringLength(600)]
	public string? CallbackUrl { get; set; }

	public bool? ForwardServiceToken { get; set; }

	public Dictionary<string, string>? CallbackHeaders { get; set; }

}

public class UpdateRegisteredAppCommandHandler(
	IRegisteredAppRepository registeredAppRepository,
	IMapper mapper,
	IMachineDateTime machineDateTime
	) : IUpdateItemCommandHandler<UpdateRegisteredAppCommand, RegisteredAppViewModel>
{
	private readonly IRegisteredAppRepository _registeredAppRepository = registeredAppRepository;
	private readonly IMapper _mapper = mapper;
	private readonly IMachineDateTime _machineDateTime = machineDateTime;
	public async Task<RegisteredAppViewModel> UpdateItemAsync(UpdateRegisteredAppCommand command, string userId)
	{
		var existingEntity = await _registeredAppRepository.GetEntityItemAsync(command.Id);

		existingEntity.AppName = command.AppName ?? existingEntity.AppName;
		existingEntity.AppSecretKey = command.AppSecretKey ?? existingEntity.AppSecretKey;
		existingEntity.CallbackUrl = command.CallbackUrl ?? existingEntity.CallbackUrl;
		existingEntity.ForwardServiceToken = command.ForwardServiceToken ?? existingEntity.ForwardServiceToken;
		existingEntity.CallbackHeaders = command.CallbackHeaders ?? existingEntity.CallbackHeaders;

		existingEntity.UpdateEntityAuditFields(userId, _machineDateTime.Now);

		await _registeredAppRepository.UpdateEntityItemAsync(existingEntity);

		return new RegisteredAppViewModel
		{
			RegisteredApp = _mapper.Map<RegisteredAppDTO>(existingEntity)
		};
	}
}
