using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.RegisteredApps.Queries;
using PolyzenKit.Application.Core.RegisteredApps.ViewModels;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Presentation.Controllers;

using UserIdentity.Application.Core.InviteCodes.Commands;
using UserIdentity.Application.Core.InviteCodes.Queries;
using UserIdentity.Application.Core.InviteCodes.ViewModels;

namespace UserIdentity.Presentation.Controllers;

[Authorize]
public class InviteCodeController(
  ICreateItemCommandHandler<CreateInviteCodeCommand, InviteCodeViewModel> createItemCommandHandler,
  IGetItemQueryHandler<GetInviteCodeQuery, InviteCodeViewModel> getItemQueryHandler,

  IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> getRegisteredAppQueryHandler
  ) : BaseController
{

  private readonly ICreateItemCommandHandler<CreateInviteCodeCommand, InviteCodeViewModel> _createItemCommandHandler = createItemCommandHandler;
  private readonly IGetItemQueryHandler<GetInviteCodeQuery, InviteCodeViewModel> _getItemQueryHandler = getItemQueryHandler;

  private readonly IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> _getRegisteredAppQueryHandler = getRegisteredAppQueryHandler;


  [Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
  [HttpGet]
  [Route("{id}")]
  public async Task<ActionResult<InviteCodeViewModel>> GetInviteCode(long id)
  {
    var vm = await _getItemQueryHandler.GetItemAsync(new GetInviteCodeQuery { InviteCodeId = id });
    return ResolveGetItemActionResult(vm, true);
  }

  [Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
  [HttpGet]
  [Route("email/{email}")]
  public async Task<ActionResult<InviteCodeViewModel>> GetInviteCodeByEmail(string email)
  {
    var vm = await _getItemQueryHandler.GetItemAsync(new GetInviteCodeQuery { UserEmail = email });
    return ResolveGetItemActionResult(vm, true);
  }

  [Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
  [HttpPost]
  [Route("")]
  public async Task<ActionResult<InviteCodeViewModel>> CreateInviteCode(CreateInviteCodeCommand command)
  {
    var registeredAppDTO = await GetAppNameAsync(_getRegisteredAppQueryHandler);

    command.AppId = registeredAppDTO.Id;

    var vm = await _createItemCommandHandler.CreateItemAsync(command, LoggedInUserId);
    return ResolveCreateItemActionResult(vm, true);
  }

}
