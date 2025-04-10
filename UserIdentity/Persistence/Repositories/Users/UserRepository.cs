using Microsoft.EntityFrameworkCore;

using UserIdentity.Domain.Identity;


namespace UserIdentity.Persistence.Repositories.Users;

public class UserRepository(
	AppDbContext appDbContext
	) : IUserRepository
{
	private readonly AppDbContext _appDbContext = appDbContext;

	public async Task<int> CreateUserAsync(UserEntity user)
	{
		try
		{
			_appDbContext.AppUser?.Add(user);
			return await _appDbContext.SaveChangesAsync();

		}
		catch (Exception)
		{
			return 0;
		}
	}

	public async Task<UserEntity?> GetUserAsync(string Id)
	{
		return await _appDbContext.AppUser
				.Where(u => (u.Id + "").Equals(Id) && !u.IsDeleted)
				.FirstOrDefaultAsync();
	}

	public async Task<int> UpdateResetPasswordTokenAsync(string userId, string resetPasswordToken)
	{
		try
		{
			var user = await GetUserAsync(userId);

			if (user == null)
				return 0;

			user.ForgotPasswordToken = resetPasswordToken;
			var result = await _appDbContext.SaveChangesAsync();


			return result;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public async Task<bool> ValidateUpdatePasswordTokenAsync(string userId, string token)
	{
		return await _appDbContext.AppUser
				.AnyAsync(u => (u.Id + "").Equals(userId) && (u.ForgotPasswordToken + "").Equals(token) && !u.IsDeleted);
	}

	public async Task DeleteUserAsync(string userId)
	{
		if ((await _appDbContext.AppUser.Where(e => e.Id == userId).ExecuteDeleteAsync()) < 1)
			throw new InvalidOperationException($"deleting user identified with {userId}");
	}
}
