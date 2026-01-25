using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Core.RegisteredApps.Queries;
using PolyzenKit.Application.Core.RegisteredApps.ViewModels;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Presentation.Controllers;

using UserIdentity.Application.Core.WaitLists.Commands;
using UserIdentity.Application.Core.WaitLists.Queries;
using UserIdentity.Application.Core.WaitLists.ViewModels;

namespace UserIdentity.Presentation.Controllers;

public class WaitListController(
   ICreateItemCommandHandler<CreateWaitListCommand, WaitListViewModel> createItemCommandHandler,
   IGetItemQueryHandler<GetWaitListQuery, WaitListViewModel> getItemQueryHandler,

   IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> getRegisteredAppQueryHandler
  ) : BaseController
{

  private readonly ICreateItemCommandHandler<CreateWaitListCommand, WaitListViewModel> _createItemCommandHandler = createItemCommandHandler;
  private readonly IGetItemQueryHandler<GetWaitListQuery, WaitListViewModel> _getItemQueryHandler = getItemQueryHandler;

  private readonly IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> _getRegisteredAppQueryHandler = getRegisteredAppQueryHandler;

  [Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
  [HttpGet]
  [Route("{id}")]
  public async Task<ActionResult<WaitListViewModel>> GetWaitList(long id)
  {
    var vm = await _getItemQueryHandler.GetItemAsync(new GetWaitListQuery { WaitListId = id });
    return ResolveGetItemActionResult(vm, true);
  }

  [Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
  [HttpGet]
  [Route("email/{email}")]
  public async Task<ActionResult<WaitListViewModel>> GetWaitListByEmail(string email)
  {
    var vm = await _getItemQueryHandler.GetItemAsync(new GetWaitListQuery { UserEmail = email });
    return ResolveGetItemActionResult(vm, true);
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("")]
  public async Task<ActionResult<WaitListViewModel>> CreateWaitList(CreateWaitListCommand command)
  {
    var registeredAppDTO = await GetAppNameAsync(_getRegisteredAppQueryHandler);

    command.AppId = registeredAppDTO.Id;

    var vm = await _createItemCommandHandler.CreateItemAsync(command, LoggedInUserId);
    return ResolveCreateItemActionResult(vm, true);
  }

}
