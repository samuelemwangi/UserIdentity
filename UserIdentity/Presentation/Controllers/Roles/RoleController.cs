using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Extensions;
using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.Commands.CreateRole;
using UserIdentity.Application.Core.Roles.Commands.CreateRoleClaim;
using UserIdentity.Application.Core.Roles.Commands.DeleteRole;
using UserIdentity.Application.Core.Roles.Commands.DeleteRoleClaim;
using UserIdentity.Application.Core.Roles.Commands.UpdateRole;
using UserIdentity.Application.Core.Roles.Queries.GetRole;
using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Enums;
using UserIdentity.Presentation.Helpers.ValidationExceptions;

namespace UserIdentity.Presentation.Controllers.Roles
{
    [Authorize]
	[ValidateModel]
	public class RoleController : BaseController
	{
		private readonly ICreateItemCommandHandler<CreateRoleCommand, RoleViewModel> _createRoleCommandHandler;
		private readonly ICreateItemCommandHandler<CreateUserRoleCommand, UserRolesViewModel> _createUserRoleCommandHandler;
		private readonly IGetItemQueryHandler<GetRoleQuery, RoleViewModel> _getRoleQueryHandler;
		private readonly IGetItemsQueryHandler<GetRolesQuery, RolesViewModel> _getRolesQueryHandler;
		private readonly IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> _getUserRolesQueryHandler;
		private readonly IUpdateItemCommandHandler<UpdateRoleCommand, RoleViewModel> _updateRoleCommandHandler;
		private readonly IDeleteItemCommandHandler<DeleteRoleCommand, DeleteRecordViewModel> _deleteRoleCommandHandler;
		private readonly ICreateItemCommandHandler<CreateRoleClaimCommand, RoleClaimViewModel> _createRoleClaimCommandHandler;
		private readonly IGetItemsQueryHandler<GetRoleClaimsQuery, RoleClaimsViewModel> _getRoleClaimsQueryHandler;
		private readonly IDeleteItemCommandHandler<DeleteRoleClaimCommand, DeleteRecordViewModel> _deleteRoleClaimCommandHandler;

		private String resourceName => EntityName ?? "role";


		public RoleController(
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
				)
		{
			_createRoleCommandHandler = createRoleCommandHandler;
			_createUserRoleCommandHandler = createUserRoleCommandHandler;
			_getRoleQueryHandler = getRoleQueryHandler;
			_getRolesQueryHandler = getRolesQueryHandler;
			_getUserRolesQueryHandler = getUserRolesQueryHandler;
			_updateRoleCommandHandler = updateRoleCommandHandler;
			_deleteRoleCommandHandler = deleteRoleCommandHandler;
			_createRoleClaimCommandHandler = createRoleClaimCommandHandler;
			_getRoleClaimsQueryHandler = getRoleClaimsQueryHandler;
			_deleteRoleClaimCommandHandler = deleteRoleClaimCommandHandler;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		public async Task<ActionResult<RolesViewModel>> GetRolesAsync()
		{
			var rolesVM = await _getRolesQueryHandler.GetItemsAsync(new GetRolesQuery { });

			rolesVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			rolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.OK, rolesVM);
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		[Route("{roleId}")]
		public async Task<ActionResult<RoleViewModel>> GetRoleAsync(String roleId)
		{
			var roleVM = await _getRoleQueryHandler.GetItemAsync(new GetRoleQuery { RoleId = roleId });

			roleVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEM_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.OK, roleVM);
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPost]
		public async Task<ActionResult<RoleViewModel>> CreateRoleAsync(CreateRoleCommand command)
		{
			var roleVM = await _createRoleCommandHandler.CreateItemAsync(command);

			roleVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.Created, roleVM);
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPut]
		[Route("{roleId}")]
		public async Task<ActionResult<RoleViewModel>> UpdateRoleAsync(String roleId, [FromBody] UpdateRoleCommand command)
		{
			command.RoleId = roleId;

			var updatedRoleVM = await _updateRoleCommandHandler.UpdateItemAsync(command);

			updatedRoleVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			updatedRoleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.UPDATE_ITEM_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.OK, updatedRoleVM);
		}


		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpDelete]
		[Route("{roleId}")]
		public async Task<ActionResult<DeleteRecordViewModel>> DeleteRoleAsync(String roleId)
		{
			var deleteRoleVM = await _deleteRoleCommandHandler.DeleteItemAsync(new DeleteRoleCommand { RoleId = roleId });

			deleteRoleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.DELETE_ITEM_SUCCESSFUL, "Record deleted successfully");

			return StatusCode((Int32)HttpStatusCode.OK, deleteRoleVM);
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		[Route("user/{userId}")]
		public async Task<ActionResult<UserRolesViewModel>> GetUserRolesAsync(String userId)
		{
			var userRolesVM = await _getUserRolesQueryHandler.GetItemsAsync(new GetUserRolesQuery { UserId = userId });

			userRolesVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			userRolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.OK, userRolesVM);
		}



		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPost]
		[Route("user")]
		public async Task<ActionResult<UserRolesViewModel>> CreateUserRoleAsync(CreateUserRoleCommand command)
		{
			var userRolesVM = await _createUserRoleCommandHandler.CreateItemAsync(command);

			userRolesVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			userRolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.Created, userRolesVM);
		}


		/// <summary>
		/// Roles Claims
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPost]
		[Route("claim")]
		public async Task<ActionResult<RoleClaimViewModel>> CreateRoleClaimAsync(CreateRoleClaimCommand command)
		{
			var roleClaimVM = await _createRoleClaimCommandHandler.CreateItemAsync(command);

			roleClaimVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			roleClaimVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.CREATE_ITEM_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.Created, roleClaimVM);
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		[Route("claim/{roleId}")]
		public async Task<ActionResult<RoleClaimsViewModel>> GetRoleClaimsAsync(String roleId)
		{
			var roleClaimsVM = await _getRoleClaimsQueryHandler.GetItemsAsync(new GetRoleClaimsQuery { RoleId = roleId });

			roleClaimsVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			roleClaimsVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL);

			return StatusCode((Int32)HttpStatusCode.OK, roleClaimsVM);
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpDelete]
		[Route("claim")]
		public async Task<ActionResult<DeleteRecordViewModel>> DelteRoleClaimAsync(DeleteRoleClaimCommand command)
		{
			var deleteClaimVM = await _deleteRoleClaimCommandHandler.DeleteItemAsync(command);

			deleteClaimVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.DELETE_ITEM_SUCCESSFUL, "Record deleted successfully");

			return StatusCode((Int32)HttpStatusCode.OK, deleteClaimVM);
		}
	}
}
