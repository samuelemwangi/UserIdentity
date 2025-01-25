using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.Users
{
	public interface IUserRepository
	{
		Task<User?> GetUserAsync(string id);

		Task<int> CreateUserAsync(User user);

		Task<int> UpdateResetPasswordTokenAsync(string userId, string resetPasswordToken);

		Task<bool> ValidateUpdatePasswordTokenAsync(string token, string userId);
	}
}
