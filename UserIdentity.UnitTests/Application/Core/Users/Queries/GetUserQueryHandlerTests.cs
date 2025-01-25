using System.Threading.Tasks;

using FakeItEasy;

using Microsoft.AspNetCore.Identity;

using PolyzenKit.Application.Interfaces;
using PolyzenKit.Common.Exceptions;
using PolyzenKit.Infrastructure.Utilities;

using UserIdentity.Application.Core.Users.Queries.GetUser;
using UserIdentity.Application.Core.Users.ViewModels;
using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.Users;

using Xunit;

namespace UserIdentity.UnitTests.Application.Core.Users.Queries
{
	public class GetUserQueryHandlerTests
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IUserRepository _userRepository;
		private readonly IMachineDateTime _machineDateTime;

		public GetUserQueryHandlerTests()
		{
			_userManager = A.Fake<UserManager<IdentityUser>>();
			_userRepository = A.Fake<IUserRepository>();
			_machineDateTime = new MachineDateTime();
		}

		[Fact]
		public async Task Get_User_When_Non_Existent_In_User_Manager_Throws_NoRecordException()
		{

			// Arrange
			var query = new GetUserQuery
			{
				UserId = "test"
			};

			A.CallTo(() => _userManager.FindByIdAsync(query.UserId)).Returns(default(IdentityUser));

			var handler = new GetUserQueryHandler(_userManager, _userRepository, _machineDateTime);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.GetItemAsync(query));
		}

		[Fact]
		public async Task Get_User_When_Non_Existent_In_User_Repo_Throws_NoRecordException()
		{

			// Arrange
			var query = new GetUserQuery
			{
				UserId = "test"
			};

			var existingIdentityUser = new IdentityUser
			{
				Id = "test",
				UserName = "test",
				Email = "test@lp.mll",
			};

			A.CallTo(() => _userManager.FindByIdAsync(query.UserId)).Returns(existingIdentityUser);
			A.CallTo(() => _userRepository.GetUserAsync(query.UserId)).Returns(default(User));

			var handler = new GetUserQueryHandler(_userManager, _userRepository, _machineDateTime);

			// Act & Assert
			await Assert.ThrowsAsync<NoRecordException>(() => handler.GetItemAsync(query));
		}

		[Fact]
		public async Task Get_User_Existing_User_Returns_User()
		{

			// Arrange
			var query = new GetUserQuery
			{
				UserId = "test"
			};

			var existingIdentityUser = new IdentityUser
			{
				Id = "test",
				UserName = "test",
				Email = "test@lp.mll",
			};

			var existingUser = new User
			{
				Id = existingIdentityUser.Id,
				FirstName = "test",
				LastName = "test",
			};

			A.CallTo(() => _userManager.FindByIdAsync(query.UserId)).Returns(existingIdentityUser);
			A.CallTo(() => _userRepository.GetUserAsync(query.UserId)).Returns(existingUser);

			var handler = new GetUserQueryHandler(_userManager, _userRepository, _machineDateTime);

			// Act 
			var vm = await handler.GetItemAsync(query);

			// Assert
			Assert.IsType<UserViewModel>(vm);
			Assert.IsType<UserDTO>(vm.User);
			Assert.Equal(existingUser.Id, vm.User?.Id);
		}
	}
}
