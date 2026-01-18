using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Interfaces;
using PolyzenKit.Persistence.Repositories;

using UserIdentity.Application.Core.InviteCodes.ViewModels;
using UserIdentity.Domain.InviteCodes;
using UserIdentity.Persistence.Repositories.InviteCodes;

namespace UserIdentity.Application.Core.InviteCodes.Commands;

public class CreateInviteCodeCommand : InviteCodeEntity, IBaseCommand
{

}

public class CreateInviteCodeCommandHandler(
   IUnitOfWork unitOfWork,
   IInviteCodeRepository inviteCodeRepository,
   IMachineDateTime machineDateTime
  ) : BaseAuditableItemCommandHandler<long, InviteCodeEntity, IInviteCodeRepository>(unitOfWork, inviteCodeRepository, machineDateTime),
      ICreateItemCommandHandler<CreateInviteCodeCommand, InviteCodeViewModel>
{
  public async Task<InviteCodeViewModel> CreateItemAsync(CreateInviteCodeCommand command, string userId)
  {
    var entity = (InviteCodeEntity)command;

    await CreateEntityItemAsync(entity, userId);

    return new InviteCodeViewModel
    {
      InviteCode = await _repository.GetEntityDTOByIdAsync(entity.Id)
    };
  }
}
