using System.ComponentModel.DataAnnotations;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;

using UserIdentity.Persistence.Repositories.RegisteredApps;

namespace UserIdentity.Application.Core.RegisteredApps.Commands;

public record DeleteRegisteredAppCommand : IBaseCommand
{
	[Required]
	public int Id { get; internal set; }
}

public class DeleteRegisteredAppCommandHandler(
	IRegisteredAppRepository registeredAppRepository
	) : IDeleteItemCommandHandler<DeleteRegisteredAppCommand, DeleteRecordViewModel>
{
	private readonly IRegisteredAppRepository _registeredAppRepository = registeredAppRepository;
	public async Task<DeleteRecordViewModel> DeleteItemAsync(DeleteRegisteredAppCommand command, string userId)
	{
		await _registeredAppRepository.DeleteEntityItemAsync(command.Id);

		return new DeleteRecordViewModel
		{
		};
	}
}
