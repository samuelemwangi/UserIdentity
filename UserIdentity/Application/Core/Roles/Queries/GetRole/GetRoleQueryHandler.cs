using Microsoft.AspNetCore.Identity;

using UserIdentity.Application.Core.Interfaces;
using UserIdentity.Application.Core.Roles.ViewModels;
using UserIdentity.Application.Exceptions;

namespace UserIdentity.Application.Core.Roles.Queries.GetRole
{
	public record GetRoleQuery : BaseQuery
	{
		public string RoleId { get; init; }
	}

	public class GetRoleQueryHandler : IGetItemQueryHandler<GetRoleQuery, RoleViewModel>
	{
		private readonly RoleManager<IdentityRole> _roleManager;

		public GetRoleQueryHandler(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<RoleViewModel> GetItemAsync(GetRoleQuery query)
		{

			var role = await _roleManager.FindByIdAsync(query.RoleId);

			if (role == null)
				throw new NoRecordException(query.RoleId, "Role");

			return new RoleViewModel
			{

				Role = new RoleDTO
				{
					Id = query.RoleId,
					Name = role.Name
				}

			};

		}
	}
}
