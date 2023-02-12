using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core;
using UserIdentity.Application.Core.Extensions;
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
		private readonly CreateRoleCommandHandler _createUserRoleCommandHandler;
		private readonly GetRoleQueryHandler _getRoleQueryHandler;
		private readonly GetRolesQueryHandler _getRolesQueryHandler;
		private readonly GetUserRolesQueryHandler _getUserRolesQueryHandler;
		private readonly UpdateRoleCommandHandler _updateRoleCommandHandler;
		private readonly DeleteRoleCommandHandler _deleteRoleCommandHandler;
		private readonly CreateRoleClaimCommandHandler _createRoleClaimCommandHandler;
		private readonly GetRoleClaimsQueryHandler _getRoleClaimsQueryHandler;
		private readonly DeleteRoleClaimCommandHandler _deleteRoleClaimCommandHandler;

		private String resourceName => EntityName ?? "role";


		public RoleController(
				CreateRoleCommandHandler createUserRoleCommandHandler,
				GetRoleQueryHandler getRoleQueryHandler,
				GetRolesQueryHandler getRolesQueryHandler,
				GetUserRolesQueryHandler getUserRolesQueryHandler,
				UpdateRoleCommandHandler updateRoleCommandHandler,
				DeleteRoleCommandHandler deleteRoleCommandHandler,
				CreateRoleClaimCommandHandler createRoleClaimCommandHandler,
				GetRoleClaimsQueryHandler getRoleClaimsQueryHandler,
				DeleteRoleClaimCommandHandler deleteRoleClaimCommandHandler
				)
		{
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
			rolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return rolesVM;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		[Route("{roleId}")]
		public async Task<ActionResult<RoleViewModel>> GetRoleAsync(string roleId)
		{
			var roleVM = await _getRoleQueryHandler.GetItemAsync(new GetRoleQuery { RoleId = roleId });

			roleVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return roleVM;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPost]
		public async Task<ActionResult<RoleViewModel>> CreateRoleAsync(CreateRoleCommand command)
		{
			var roleVM = await _createUserRoleCommandHandler.CreateRoleAsync(command);

			roleVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			roleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return roleVM;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPut]
		public async Task<ActionResult<RoleViewModel>> UpdateRoleAsync(UpdateRoleCommand command)
		{
			var updatedRoleVM = await _updateRoleCommandHandler.UpdateRoleAsync(command);

			updatedRoleVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			updatedRoleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return updatedRoleVM;
		}


		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpDelete]
		[Route("{roleId}")]
		public async Task<ActionResult<DeleteRecordViewModel>> DeleteRoleAsync(string roleId)
		{
			var deleteRoleVM = await _deleteRoleCommandHandler.DeleteRoleAsync(new DeleteRoleCommand { RoleId = roleId });

			deleteRoleVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS, "Record deleted successfully");

			return deleteRoleVM;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		[Route("user/{userId}")]
		public async Task<ActionResult<UserRolesViewModel>> GetUserRolesAsync(string userId)
		{
			var userRolesVM = await _getUserRolesQueryHandler.GetItemsAsync(new GetUserRolesQuery { UserId = userId });

			userRolesVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			userRolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return userRolesVM;
		}



		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpPost]
		[Route("user")]
		public async Task<ActionResult<UserRolesViewModel>> CreateUserRoleAsync(CreateUserRoleCommand command)
		{
			var userRolesVM = await _createUserRoleCommandHandler.CreateUserRoleAsync(command);

			userRolesVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			userRolesVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return userRolesVM;
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
			var roleClaimVM = await _createRoleClaimCommandHandler.CreateRoleClaimAsync(command);

			roleClaimVM.ResolveEditDeleteRights(UserRoleClaims, resourceName);
			roleClaimVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return roleClaimVM;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpGet]
		[Route("claim/{roleId}")]
		public async Task<ActionResult<RoleClaimsViewModel>> GetRoleClaimsAsync(String roleId)
		{
			var roleClaimsVM = await _getRoleClaimsQueryHandler.GetRoleClaimsAsync(new GetRoleClaimsQuery { RoleId = roleId });

			roleClaimsVM.ResolveCreateDownloadRights(UserRoleClaims, resourceName);
			roleClaimsVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS);

			return roleClaimsVM;
		}

		[Authorize(Roles = "Administrator, SuperAdministrator")]
		[HttpDelete]
		[Route("claim")]
		public async Task<ActionResult<DeleteRecordViewModel>> DelteRoleClaimsAsync(DeleteRoleClaimCommand command)
		{
			var deleteClaimVM = await _deleteRoleClaimCommandHandler.DeleteRoleClaimAsync(command);

			deleteClaimVM.ResolveRequestStatus(RequestStatus.SUCCESSFUL, ItemStatusMessage.SUCCESS, "Record deleted successfully");

			return deleteClaimVM;
		}
	}
}
