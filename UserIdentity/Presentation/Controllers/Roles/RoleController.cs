using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Extensions;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Application.Enums;
using PolyzenKit.Common.Utilities;
using PolyzenKit.Presentation.Controllers;
using PolyzenKit.Presentation.ValidationHelpers;

using UserIdentity.Application.Core.Roles.Commands.CreateRole;
using UserIdentity.Application.Core.Roles.Commands.CreateRoleClaim;
using UserIdentity.Application.Core.Roles.Commands.DeleteRole;
using UserIdentity.Application.Core.Roles.Commands.DeleteRoleClaim;
using UserIdentity.Application.Core.Roles.Commands.UpdateRole;
using UserIdentity.Application.Core.Roles.Queries.GetRole;
using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Presentation.Controllers.Roles;

[Authorize]
[ValidateModel]
public class RoleController(
	ICreateItemCommandHandler<CreateRoleCommand, RoleViewModel> createRoleCommandHandler,
	ICreateItemCommandHandler<CreateUserRoleCommand, UserRolesViewModel> createUserRoleCommandHandler,
	IGetItemQueryHandler<GetRoleQuery, RoleViewModel> getRoleQueryHandler,
	IGetItemsQueryHandler<GetRolesQuery, RolesViewModel> getRolesQueryHandler,
	IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> getUserRolesQueryHandler,
	IUpdateItemCommandHandler<UpdateRoleCommand, RoleViewModel> updateRoleCommandHandler,
	IDeleteItemCommandHandler<DeleteRoleCommand, DeleteRecordViewModel> deleteRoleCommandHandler,
	ICreateItemCommandHandler<CreateRoleClaimCommand, RoleClaimViewModel> createRoleClaimCommandHandler,
	IGetItemsQueryHandler<GetRoleClaimsQuery, RoleClaimsViewModel> getRoleClaimsQueryHandler,
	IDeleteItemCommandHandler<DeleteRoleClaimCommand, DeleteRecordViewModel> deleteRoleClaimCommandHandler
	) : BaseController
{
	private readonly ICreateItemCommandHandler<CreateRoleCommand, RoleViewModel> _createRoleCommandHandler = createRoleCommandHandler;
	private readonly ICreateItemCommandHandler<CreateUserRoleCommand, UserRolesViewModel> _createUserRoleCommandHandler = createUserRoleCommandHandler;
	private readonly IGetItemQueryHandler<GetRoleQuery, RoleViewModel> _getRoleQueryHandler = getRoleQueryHandler;
	private readonly IGetItemsQueryHandler<GetRolesQuery, RolesViewModel> _getRolesQueryHandler = getRolesQueryHandler;
	private readonly IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> _getUserRolesQueryHandler = getUserRolesQueryHandler;
	private readonly IUpdateItemCommandHandler<UpdateRoleCommand, RoleViewModel> _updateRoleCommandHandler = updateRoleCommandHandler;
	private readonly IDeleteItemCommandHandler<DeleteRoleCommand, DeleteRecordViewModel> _deleteRoleCommandHandler = deleteRoleCommandHandler;
	private readonly ICreateItemCommandHandler<CreateRoleClaimCommand, RoleClaimViewModel> _createRoleClaimCommandHandler = createRoleClaimCommandHandler;
	private readonly IGetItemsQueryHandler<GetRoleClaimsQuery, RoleClaimsViewModel> _getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;
	private readonly IDeleteItemCommandHandler<DeleteRoleClaimCommand, DeleteRecordViewModel> _deleteRoleClaimCommandHandler = deleteRoleClaimCommandHandler;

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpGet]
	public async Task<ActionResult<RolesViewModel>> GetRolesAsync()
	{
		var roleClaims = User.Claims
											.Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
											.Select(c => c.Value)
											.ToList();

		var rolesVM = await _getRolesQueryHandler.GetItemsAsync(new GetRolesQuery { });

		rolesVM.ResolveCreateDownloadRights(UserScopeClaims, EntityName);
		rolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, rolesVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpGet]
	[Route("{roleId}")]
	public async Task<ActionResult<RoleViewModel>> GetRoleAsync(string roleId)
	{
		var roleVM = await _getRoleQueryHandler.GetItemAsync(new GetRoleQuery { RoleId = roleId });

		roleVM.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, roleVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpPost]
	public async Task<ActionResult<RoleViewModel>> CreateRoleAsync(CreateRoleCommand command)
	{
		var roleVM = await _createRoleCommandHandler.CreateItemAsync(command, LoggedInUserId);

		roleVM.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.Created, roleVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpPut]
	[Route("{roleId}")]
	public async Task<ActionResult<RoleViewModel>> UpdateRoleAsync(string roleId, [FromBody] UpdateRoleCommand command)
	{
		command.RoleId = roleId;

		var updatedRoleVM = await _updateRoleCommandHandler.UpdateItemAsync(command, LoggedInUserId);

		updatedRoleVM.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		updatedRoleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.UPDATE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, updatedRoleVM);
	}


	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpDelete]
	[Route("{roleId}")]
	public async Task<ActionResult<DeleteRecordViewModel>> DeleteRoleAsync(string roleId)
	{
		var deleteRoleVM = await _deleteRoleCommandHandler.DeleteItemAsync(new DeleteRoleCommand { RoleId = roleId }, LoggedInUserId);

		deleteRoleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.DELETE_ITEM_SUCCESSFUL, "Record deleted successfully");

		return StatusCode((int)HttpStatusCode.OK, deleteRoleVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_OR_SAME_USER_POLICY)]
	[HttpGet]
	[Route("user/{userId}")]
	public async Task<ActionResult<UserRolesViewModel>> GetUserRolesAsync(string userId)
	{
		var userRolesVM = await _getUserRolesQueryHandler.GetItemsAsync(new GetUserRolesQuery { UserId = userId });

		userRolesVM.ResolveCreateDownloadRights(UserScopeClaims, EntityName);
		userRolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, userRolesVM);
	}


	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpPost]
	[Route("user")]
	public async Task<ActionResult<UserRolesViewModel>> CreateUserRoleAsync(CreateUserRoleCommand command)
	{
		var userRolesVM = await _createUserRoleCommandHandler.CreateItemAsync(command, LoggedInUserId);

		userRolesVM.ResolveCreateDownloadRights(UserScopeClaims, EntityName);
		userRolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.Created, userRolesVM);
	}


	/// <summary>
	/// Roles Claims
	/// </summary>
	/// <param name="command"></param>
	/// <returns></returns>

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpPost]
	[Route("claim")]
	public async Task<ActionResult<RoleClaimViewModel>> CreateRoleClaimAsync(CreateRoleClaimCommand command)
	{
		var roleClaimVM = await _createRoleClaimCommandHandler.CreateItemAsync(command, LoggedInUserId);

		roleClaimVM.ResolveEditDeleteRights(UserScopeClaims, EntityName);
		roleClaimVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.Created, roleClaimVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpGet]
	[Route("claim/{roleId}")]
	public async Task<ActionResult<RoleClaimsViewModel>> GetRoleClaimsAsync(string roleId)
	{
		var roleClaimsVM = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsQuery { RoleId = roleId });

		roleClaimsVM.ResolveCreateDownloadRights(UserScopeClaims, EntityName);
		roleClaimsVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL);

		return StatusCode((int)HttpStatusCode.OK, roleClaimsVM);
	}

	[Authorize(Policy = ZenConstants.ADMIN_USER_POLICY)]
	[HttpDelete]
	[Route("claim")]
	public async Task<ActionResult<DeleteRecordViewModel>> DeleteRoleClaimAsync(DeleteRoleClaimCommand command)
	{
		var deleteClaimVM = await _deleteRoleClaimCommandHandler.DeleteItemAsync(command, LoggedInUserId);

		deleteClaimVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.DELETE_ITEM_SUCCESSFUL, "Record deleted successfully");

		return StatusCode((int)HttpStatusCode.OK, deleteClaimVM);
	}
}
