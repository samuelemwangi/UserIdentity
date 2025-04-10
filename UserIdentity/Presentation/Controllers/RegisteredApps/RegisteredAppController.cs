using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Extensions;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Enums;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Presentation.Controllers;
using PolyzenKit.Presentation.ValidationHelpers;

using UserIdentity.Application.Core.RegisteredApps.Commands;
using UserIdentity.Application.Core.RegisteredApps.Queries;
using UserIdentity.Application.Core.RegisteredApps.ViewModels;
using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Presentation.Controllers.RegisteredApps;

[Authorize]
[ValidateModel]
public class RegisteredAppController(
	IGetItemQueryHandler<GetRegisteredAppsQuery, RegisteredAppsViewModel> getRegisteredAppsQueryHandler,
	IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> getRegisteredAppQueryHandler,
	ICreateItemCommandHandler<CreateRegisteredAppCommand, RegisteredAppViewModel> createRegisteredAppCommandHandler,
	IUpdateItemCommandHandler<UpdateRegisteredAppCommand, RegisteredAppViewModel> updateRegisteredAppCommandHandler,
	IDeleteItemCommandHandler<DeleteRegisteredAppCommand, DeleteRecordViewModel> deleteRegisteredAppCommandHandler
	) : BaseController
{
	private readonly IGetItemQueryHandler<GetRegisteredAppsQuery, RegisteredAppsViewModel> _getRegisteredAppsQueryHandler = getRegisteredAppsQueryHandler;
	private readonly IGetItemQueryHandler<GetRegisteredAppQuery, RegisteredAppViewModel> _getRegisteredAppQueryHandler = getRegisteredAppQueryHandler;
	private readonly ICreateItemCommandHandler<CreateRegisteredAppCommand, RegisteredAppViewModel> _createRegisteredAppCommandHandler = createRegisteredAppCommandHandler;
	private readonly IUpdateItemCommandHandler<UpdateRegisteredAppCommand, RegisteredAppViewModel> _updateRegisteredAppCommandHandler = updateRegisteredAppCommandHandler;
	private readonly IDeleteItemCommandHandler<DeleteRegisteredAppCommand, DeleteRecordViewModel> _deleteRegisteredAppCommandHandler = deleteRegisteredAppCommandHandler;

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpGet]
	public async Task<ActionResult<RegisteredAppsViewModel>> GetRegisteredApps()
	{
		var vm = await _getRegisteredAppsQueryHandler.GetItemAsync(new GetRegisteredAppsQuery { });

		vm.ResolveCreateDownloadRights(UserScopeClaims, EntityName);
		vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, vm.RegisteredApps.Count > 0 ? ItemStatusMessage.FETCH_ITEM_SUCCESSFUL : ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL_NO_ITEMS);

		return StatusCode((int)HttpStatusCode.OK, vm);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpGet]
	[Route("{registeredAppId}")]
	public async Task<ActionResult<RoleViewModel>> GetRegisteredApp(int registeredAppId)
	{
		var roleVM = await _getRegisteredAppQueryHandler.GetItemAsync(new GetRegisteredAppQuery { Id = registeredAppId });

		roleVM.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, roleVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpGet]
	[Route("name/{appName}")]
	public async Task<ActionResult<RoleViewModel>> GetRegisteredApp(string appName)
	{
		var roleVM = await _getRegisteredAppQueryHandler.GetItemAsync(new GetRegisteredAppQuery { AppName = appName });

		roleVM.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, roleVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpPost]
	public async Task<ActionResult<RegisteredAppViewModel>> CreateRegisteredApp(CreateRegisteredAppCommand command)
	{
		var vm = await _createRegisteredAppCommandHandler.CreateItemAsync(command, LoggedInUserId);

		vm.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.Created, vm);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpPut]
	[Route("{registeredAppId}")]
	public async Task<ActionResult<RegisteredAppViewModel>> UpdateRegisteredApp(int registeredAppId, [FromBody] UpdateRegisteredAppCommand command)
	{
		command.Id = registeredAppId;

		var vm = await _updateRegisteredAppCommandHandler.UpdateItemAsync(command, LoggedInUserId);

		vm.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.UPDATE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, vm);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpDelete]
	[Route("{registeredAppId}")]
	public async Task<ActionResult<RegisteredAppViewModel>> DeleteRegisteredApp(int registeredAppId)
	{
		var vm = await _deleteRegisteredAppCommandHandler.DeleteItemAsync(new DeleteRegisteredAppCommand { Id = registeredAppId }, LoggedInUserId);

		vm.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.DELETE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, vm);
	}
}
