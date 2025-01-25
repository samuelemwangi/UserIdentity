﻿using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Core;
using PolyzenKit.Application.Core.Interfaces;
using PolyzenKit.Common.Exceptions;

using UserIdentity.Application.Core.Roles.Queries.GetRoles;
using UserIdentity.Application.Core.Roles.ViewModels;

namespace UserIdentity.Application.Core.Roles.Commands.CreateRole
{
	public record CreateUserRoleCommand : BaseCommand
	{
		[Required]
		public string UserId { get; init; } = null!;

		[Required]
		public string RoleId { get; init; } = null!;
	}

	public class CreateUserRoleCommandHandler(
		RoleManager<IdentityRole> roleManager,
		UserManager<IdentityUser> userManager,
		IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> getUserRolesQueryHandler
		) : ICreateItemCommandHandler<CreateUserRoleCommand, UserRolesViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager = roleManager;
		private readonly UserManager<IdentityUser> _userManager = userManager;
		private readonly IGetItemsQueryHandler<GetUserRolesQuery, UserRolesViewModel> _getUserRolesQueryHandler = getUserRolesQueryHandler;

		public async Task<UserRolesViewModel> CreateItemAsync(CreateUserRoleCommand command, string userid)
		{
			var user = await _userManager.FindByIdAsync(command.UserId) ?? throw new NoRecordException(command.UserId + "", "User");

			var role = await _roleManager.FindByIdAsync(command.RoleId) ?? throw new NoRecordException(command.RoleId + "", "Role");

			var userRoleExists = await _userManager.IsInRoleAsync(user, role.Name!);

			if (userRoleExists)
				throw new RecordExistsException(command.RoleId, "UserRole");

			var resultUserRole = await _userManager.AddToRoleAsync(user, role.Name!);

			if (!resultUserRole.Succeeded)
				throw new RecordCreationException(command.RoleId, "UserRole");

			// remove roles from cache

			var resolvedRoles = await _getUserRolesQueryHandler.GetItemsAsync(new GetUserRolesQuery { UserId = command.UserId });

			return resolvedRoles;
		}
	}
}
