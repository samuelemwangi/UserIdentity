using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.Users
{
	public interface IUserRepository
	{
		Task<IReadOnlyCollection<User>> GetUsersAsync();

		Task<User?> GetUserAsync(String? id);

		Task<Int32> CreateUserAsync(User user);

		Task UpdateUserAsync(User user);

		Task<Int32> UpdateResetPasswordTokenAsync(String userId, String resetPasswordToken);

		Task<Boolean> ValidateUpdatePasswordTokenAsync(String token, String userId);
    }
}
