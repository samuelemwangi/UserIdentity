using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Mvc;

using UserIdentity.Application.Core;
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
using UserIdentity.Presentation.Controllers.Roles;

using Xunit;

namespace UserIdentity.UnitTests.Presentation.Controllers
{

	public class RoleControllerTests
	{
		private static readonly String Controllername = "role";

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

		public RoleControllerTests()
		{
			_createRoleCommandHandler = A.Fake<ICreateItemCommandHandler<CreateRoleCommand, RoleViewModel>>();
			_createUserRoleCommandHandler = A.Fake<ICreateItemCommandHandler<CreateUserRoleCommand, UserRolesViewModel>>();
			_getRoleQueryHandler = A.Fake<IGetItemQueryHandler<GetRoleQuery, RoleViewModel>>();
			_getRolesQueryHandler = A.Fake<IGetItemsQueryHandler<GetRolesQuery, RolesViewModel>>();
			_getUserRolesQueryHandler = A.Fake<IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel>>();
			_updateRoleCommandHandler = A.Fake<IUpdateItemCommandHandler<UpdateRoleCommand, RoleViewModel>>();
			_deleteRoleCommandHandler = A.Fake<IDeleteItemCommandHandler<DeleteRoleCommand, DeleteRecordViewModel>>();
			_createRoleClaimCommandHandler = A.Fake<ICreateItemCommandHandler<CreateRoleClaimCommand, RoleClaimViewModel>>();
			_getRoleClaimsQueryHandler = A.Fake<IGetItemsQueryHandler<GetRoleClaimsQuery, RoleClaimsViewModel>>();
			_deleteRoleClaimCommandHandler = A.Fake<IDeleteItemCommandHandler<DeleteRoleClaimCommand, DeleteRecordViewModel>>();
		}

		[Fact]
		public async Task Get_Roles_Returns_Roles()
		{
			// Arrange
			var query = new GetRolesQuery();

			var rolesVM = new RolesViewModel
			{
				Roles = new List<RoleDTO>
								{
										new RoleDTO
										{
												Id = Guid.NewGuid().ToString(),
												Name = "Admin"
										},
										new RoleDTO
										{
												Id = Guid.NewGuid().ToString(),
												Name = "User"
										}
								}

			};

			// Act
			A.CallTo(() => _getRolesQueryHandler.GetItemsAsync(query)).Returns(rolesVM);

			var controller = GetRoleController();
			controller.UpdateContext(null);

			var actionResult = await controller.GetRolesAsync();
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as RolesViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(rolesVM.Roles.Count, vm?.Roles.Count);

			var allRecordsExist = false;

			foreach (var role in rolesVM.Roles)
			{
				allRecordsExist = vm?.Roles.Where(x => x.Id == role.Id && x.Name == role.Name).Any() ?? false;
			}

			Assert.True(allRecordsExist);

			Assert.False(vm?.DownloadEnabled);
			Assert.False(vm?.CreateEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Get_Role_Returns_Role()
		{
			// Arrange
			var roleId = Guid.NewGuid().ToString();
			var query = new GetRoleQuery { RoleId = roleId };

			var roleVM = new RoleViewModel
			{
				Role = new RoleDTO
				{
					Id = roleId,
					Name = "Admin"
				}
			};

			// Act
			A.CallTo(() => _getRoleQueryHandler.GetItemAsync(query)).Returns(roleVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername);

			var actionResult = await controller.GetRoleAsync(roleId);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as RoleViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(roleVM.Role.Id, vm?.Role.Id);
			Assert.Equal(roleVM.Role.Name, vm?.Role.Name);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.FETCH_ITEM_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Create_Role_Creates_Role()
		{
			// Arrange

			var command = new CreateRoleCommand
			{
				RoleName = "Admin"
			};

			var roleVM = new RoleViewModel
			{
				Role = new RoleDTO
				{
					Id = Guid.NewGuid().ToString(),
					Name = "Admin"
				}
			};

			// Act
			A.CallTo(() => _createRoleCommandHandler.CreateItemAsync(command)).Returns(roleVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername);

			var actionResult = await controller.CreateRoleAsync(command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as RoleViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.Created, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(roleVM.Role.Id, vm?.Role.Id);
			Assert.Equal(roleVM.Role.Name, vm?.Role.Name);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.CREATE_ITEM_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Update_Role_Updates_Role()
		{
			// Arrange

			var roleId = Guid.NewGuid().ToString();

			var command = new UpdateRoleCommand
			{
				RoleName = "Admin"
			};

			var roleVM = new RoleViewModel
			{
				Role = new RoleDTO
				{
					Id = roleId,
					Name = "Admin"
				}
			};

			// Act
			A.CallTo(() => _updateRoleCommandHandler.UpdateItemAsync(command)).Returns(roleVM);

			var controller = GetRoleController();
			controller.UpdateContext(null);

			var actionResult = await controller.UpdateRoleAsync(roleId, command);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as RoleViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(roleVM.Role.Id, vm?.Role.Id);
			Assert.Equal(roleVM.Role.Name, vm?.Role.Name);

			Assert.False(vm?.EditEnabled);
			Assert.False(vm?.DeleteEnabled);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.UPDATE_ITEM_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Delete_Role_Deletes_Role()
		{
			// Arrange
			var roleId = Guid.NewGuid().ToString();
			var command = new DeleteRoleCommand { RoleId = roleId };
			var deleteSuccesMessage = "Record deleted successfully";

			var deleteRoleVM = new DeleteRecordViewModel { };

			// Act
			A.CallTo(() => _deleteRoleCommandHandler.DeleteItemAsync(command)).Returns(deleteRoleVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername);

			var actionResult = await controller.DeleteRoleAsync(roleId);
			var result = actionResult?.Result as ObjectResult;
			var vm = result?.Value as DeleteRecordViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(deleteSuccesMessage, vm?.StatusMessage);
		}

		[Fact]
		public async Task Get_User_Roles_Returns_User_Roles()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var query = new GetUserRolesQuery { UserId = userId };

			var userRolesVM = new UserRolesViewModel
			{
				UserRoles = new List<String> { "Admin", "User" },
			};

			// Act
			A.CallTo(() => _getUserRolesQueryHandler.GetItemsAsync(query)).Returns(userRolesVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername);

			var actionResult = await controller.GetUserRolesAsync(userId);
			var result = actionResult?.Result as ObjectResult;

			var vm = result?.Value as UserRolesViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(userRolesVM.UserRoles.Count, vm?.UserRoles.Count);

			var allRolesExist = false;

			foreach (var role in userRolesVM.UserRoles)
			{
				if (vm?.UserRoles.Contains(role) != true)
				{
					allRolesExist = false;
					break;
				}
				else
				{
					allRolesExist = true;
				}
			}

			Assert.True(allRolesExist);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Create_User_Role_Creates_User_Role()
		{
			// Arrange
			var userId = Guid.NewGuid().ToString();
			var roleId = Guid.NewGuid().ToString();

			var command = new CreateUserRoleCommand
			{
				UserId = userId,
				RoleId = roleId
			};

			var userRolesVM = new UserRolesViewModel
			{
				UserRoles = new List<String> { "Admin", "User" },
			};

			// Act
			A.CallTo(() => _createUserRoleCommandHandler.CreateItemAsync(command)).Returns(userRolesVM);

			var controller = GetRoleController();
			controller.UpdateContext(null);

			var actionResult = await controller.CreateUserRoleAsync(command);
			var result = actionResult?.Result as ObjectResult;

			var vm = result?.Value as UserRolesViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.Created, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(userRolesVM.UserRoles.Count, vm?.UserRoles.Count);

			var allRolesExist = false;

			foreach (var role in userRolesVM.UserRoles)
			{
				if (vm?.UserRoles.Contains(role) != true)
				{
					allRolesExist = false;
					break;
				}
				else
				{
					allRolesExist = true;
				}
			}

			Assert.True(allRolesExist);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.CREATE_ITEM_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Create_Role_Claim_Creates_Role_Claim()
		{
			// Arrange
			var roleId = Guid.NewGuid().ToString();
			var resource = "Resource";
			var action = "Action";

			var command = new CreateRoleClaimCommand
			{
				RoleId = roleId,
				Resource = resource,
				Action = action
			};

			var roleClaimVM = new RoleClaimViewModel
			{
				RoleClaim = new RoleClaimDTO
				{
					Resource = resource,
					Action = action,
					Scope = resource + ":" + action
				}
			};

			// Act
			A.CallTo(() => _createRoleClaimCommandHandler.CreateItemAsync(command)).Returns(roleClaimVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername, true, true);

			var actionResult = await controller.CreateRoleClaimAsync(command);
			var result = actionResult?.Result as ObjectResult;

			var vm = result?.Value as RoleClaimViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.Created, result?.StatusCode);

			Assert.NotNull(vm);

			Assert.Equal(roleClaimVM.RoleClaim.Resource, vm?.RoleClaim.Resource);
			Assert.Equal(roleClaimVM.RoleClaim.Action, vm?.RoleClaim.Action);
			Assert.Equal(roleClaimVM.RoleClaim.Scope, vm?.RoleClaim.Scope);
		}

		[Fact]
		public async Task Get_Role_Claims_Returns_Role_Claims()
		{
			// Arrange
			var roleId = Guid.NewGuid().ToString();
			var query = new GetRoleClaimsQuery { RoleId = roleId };

			var roleClaimsVM = new RoleClaimsViewModel
			{
				RoleClaims = new List<RoleClaimDTO>
				{
					new RoleClaimDTO
					{
						Resource = "Resource",
						Action = "Action",
						Scope = "Resource:Action"
					}
				}
			};

			// Act
			A.CallTo(() => _getRoleClaimsQueryHandler.GetItemsAsync(query)).Returns(roleClaimsVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername, true);

			var actionResult = await controller.GetRoleClaimsAsync(roleId);
			var result = actionResult?.Result as ObjectResult;

			var vm = result?.Value as RoleClaimsViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);
			Assert.Equal(roleClaimsVM.RoleClaims.Count, vm?.RoleClaims.Count);

			var allClaimsExist = false;

			foreach (var claim in roleClaimsVM.RoleClaims)
			{
				if (vm?.RoleClaims.Contains(claim) != true)
				{
					allClaimsExist = false;
					break;
				}
				else
				{
					allClaimsExist = true;
				}
			}

			Assert.True(allClaimsExist);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(ItemStatusMessage.FETCH_ITEMS_SUCCESSFUL.GetDisplayName(), vm?.StatusMessage);
		}

		[Fact]
		public async Task Delte_Role_Claims_Deletes_Role_Claims()
		{
			// Arrange
			var roleId = Guid.NewGuid().ToString();
			var resource = "Resource";
			var action = "Action";
			var deleteMessage = "Record deleted successfully";

			var command = new DeleteRoleClaimCommand
			{
				RoleId = roleId,
				Resource = resource,
				Action = action
			};

			var deleteRoleClaimVM = new DeleteRecordViewModel { };

			// Act
			A.CallTo(() => _deleteRoleClaimCommandHandler.DeleteItemAsync(command)).Returns(deleteRoleClaimVM);

			var controller = GetRoleController();
			controller.UpdateContext(Controllername, true, true, true);

			var actionResult = await controller.DelteRoleClaimAsync(command);
			var result = actionResult?.Result as ObjectResult;

			var vm = result?.Value as DeleteRecordViewModel;

			// Assert
			Assert.Equal((Int32)HttpStatusCode.OK, result?.StatusCode);

			Assert.NotNull(vm);

			Assert.Contains(RequestStatus.SUCCESSFUL.GetDisplayName(), vm?.RequestStatus);
			Assert.Contains(deleteMessage, vm?.StatusMessage);

		}


		private RoleController GetRoleController()
		{
			return new RoleController(
							_createRoleCommandHandler,
							_createUserRoleCommandHandler,
							_getRoleQueryHandler,
							_getRolesQueryHandler,
							_getUserRolesQueryHandler,
							_updateRoleCommandHandler,
							_deleteRoleCommandHandler,
							_createRoleClaimCommandHandler,
							_getRoleClaimsQueryHandler,
							_deleteRoleClaimCommandHandler);
		}
	}



}
