using System;
using System.Linq;
using System.Threading.Tasks;

using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Repositories.Users;

using Xunit;

namespace UserIdentity.UnitTests.Persistence.Repositories
{
	public class UserRepositoryTests
	{
		[Fact]
		public async Task Create_User_Creates_User()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);

			var newUser = new User
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = "TestF",
				LastName = "TestL",
			};

			// Act
			var result = await userRepo.CreateUserAsync(newUser);

			// Assert
			Assert.Equal(1, result);
		}

		[Fact]
		public async Task Create_User_Failure_Does_Not_Create_User()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);

			var newUser = new User
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = "TestF",
				LastName = "TestL",
			};

			// Act
			var result = await userRepo.CreateUserAsync(newUser);

			if (result == 1)
				result = await userRepo.CreateUserAsync(newUser);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async Task Get_Existing_User_Returns_User()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);

			var newUser = new User
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = "TestF",
				LastName = "TestL",
			};

			// Act
			context.AppUser.Add(newUser);
			await context.SaveChangesAsync();

			var result = await userRepo.GetUserAsync(newUser.Id);

			Assert.IsType<User>(result);
			Assert.Equal(newUser.Id, result?.Id);
			Assert.Equal(newUser.FirstName, result?.FirstName);
			Assert.Equal(newUser.LastName, result?.LastName);
		}

		[Fact]
		public async Task Get_Non_Existing_User_Returns_Null()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);

			// Act
			var result = await userRepo.GetUserAsync(Guid.NewGuid().ToString());

			Assert.Null(result);
		}

		[Fact]
		public async Task Update_Reset_Password_Token_Updates_Reset_Password_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);

			var newUser = new User
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = "TestF",
				LastName = "TestL",
			};

			var resetPasswordToken = "Ribaloshongilogasheshiakili";

			// Act 
			context.AppUser.Add(newUser);
			await context.SaveChangesAsync();

			var result = await userRepo.UpdateResetPasswordTokenAsync(newUser.Id, resetPasswordToken);

			// Assert
			Assert.Equal(1, result);

			var savedResetPasswordToken = context.AppUser.Where(e => e.Id == newUser.Id).FirstOrDefault()?.ForgotPasswordToken;
			Assert.Equal(resetPasswordToken, savedResetPasswordToken);
		}

		[Fact]
		public async Task Update_Reset_Password_Token_For_Non_Existing_User_Does_Not_Update_Reset_Password_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);


			var resetPasswordToken = "Ribaloshongilogasheshiakili";

			// Act 
			var result = await userRepo.UpdateResetPasswordTokenAsync(Guid.NewGuid().ToString(), resetPasswordToken);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public async Task Update_Reset_Password_Token_Failure_Does_Not_Update_Reset_Password_Token()
		{
			// Arrange
			var context = AppDbContextTestFactory.GetAppDbContext();
			var userRepo = new UserRepository(context);


			var resetPasswordToken = "Ribaloshongilogasheshiakili";

			// Act 
			var result = await userRepo.UpdateResetPasswordTokenAsync(Guid.NewGuid().ToString(), resetPasswordToken);

			// Assert
			Assert.Equal(0, result);
		}
	}
}
