using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Repositories.Users;

public interface IUserRepository
{
	Task<UserEntity?> GetUserAsync(string id);

	Task<int> CreateUserAsync(UserEntity user);

	Task<int> UpdateResetPasswordTokenAsync(string userId, string resetPasswordToken);

	Task<bool> ValidateUpdatePasswordTokenAsync(string token, string userId);

	Task DeleteUserAsync(string userId);
}
