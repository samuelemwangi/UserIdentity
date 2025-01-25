using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Security.Jwt;

using UserIdentity.Application.Core.Roles.Queries.GetRoleClaims;
using UserIdentity.Application.Core.Roles.ViewModels;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Roles.Queries
{
	public class GetRoleClaimsQueryHandlerTests
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IJwtTokenHandler _jwtTokenHandler;
		public GetRoleClaimsQueryHandlerTests()
		{
			_roleManager = A.Fake<RoleManager<IdentityRole>>();
			_jwtTokenHandler = A.Fake<IJwtTokenHandler>();
		}

		[Fact]
		public async Task Get_RoleClaims_When_Role_Is_Not_Found_Throws_NoRecordException()
		{
			// Arrange
			var query = new GetRoleClaimsQuery
			{
				RoleId = "1"
			};

			A.CallTo(() => _roleManager.FindByIdAsync(query.RoleId)).Returns((default(IdentityRole)));

			var handler = new GetRoleClaimsQueryHandler(_roleManager, _jwtTokenHandler);


			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.GetItemsAsync(query));
		}

		[Fact]
		public async Task Get_RoleClaims_Returns_RoleClaims()
		{
			// Arrange
			var query = new GetRoleClaimsQuery
			{
				RoleId = "1"
			};

			var role = new IdentityRole
			{
				Id = "1",
				Name = "Admin"
			};

			var resource = "resource";
			var action = "action";

			var roleClaims = new List<Claim>
			{
				new("scope", $"{resource}:{action}")
			};



			A.CallTo(() => _roleManager.FindByIdAsync(query.RoleId)).Returns(role);

			A.CallTo(() => _roleManager.GetClaimsAsync(role)).Returns(roleClaims);

			A.CallTo(() => _jwtTokenHandler.DecodeScopeClaim(roleClaims[0])).Returns((resource, action));

			var handler = new GetRoleClaimsQueryHandler(_roleManager, _jwtTokenHandler);

			// Act
			var vm = await handler.GetItemsAsync(query);

			// Assert
			Assert.IsType<RoleClaimsViewModel>(vm);
			Assert.IsType<ICollection<RoleClaimDTO>>(vm.RoleClaims, exactMatch: false);
			Assert.Equal(roleClaims.Count, vm.RoleClaims.Count);
		}

		[Fact]
		public async Task Get_RoleClaims_Given_Roles_Returns_Unique_RoleClaims()
		{

			// Arrange
			var roles = new List<string>
			{
				"Admin"
			};

			var identityRole = new IdentityRole
			{
				Id = "1234Admin",
				Name = "Admin"
			};

			var resource = "resource";
			var action = "action";
			var scopedClaim = $"{resource}:{action}";

			var roleClaims = new List<Claim>
			{
				new("scope",scopedClaim)
			};

			A.CallTo(() => _roleManager.FindByNameAsync(roles[0])).Returns(identityRole); 
			A.CallTo(() => _roleManager.GetClaimsAsync(identityRole)).Returns(roleClaims);


			var handler = new GetRoleClaimsQueryHandler(_roleManager, _jwtTokenHandler);

			// Act
			var result = await handler.GetItemsAsync(new GetRoleClaimsForRolesQuery { Roles = roles});

			// Assert
			Assert.IsType<RoleClaimsForRolesViewModels>(result);
			Assert.Single(result.RoleClaims);
			Assert.Contains(scopedClaim, result.RoleClaims);
		}

	}
}
