using Microsoft.EntityFrameworkCore;

using UserIdentity.Domain.Identity;

/// <summary>
/// 
/// </summary>

namespace UserIdentity.Persistence.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Int32> CreateUserAsync(User user)
        {
            _appDbContext.AppUser?.Add(user);
            return await _appDbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserAsync(String? Id)
        {
            return await _appDbContext.AppUser
                .Where(u => (u.Id + "").Equals(Id))
                .FirstOrDefaultAsync();
        }

        public Task<IReadOnlyCollection<User>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }



        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<Int32> UpdateResetPasswordTokenAsync(String userId, String resetPasswordToken)
        {
            var user = await GetUserAsync(userId);

            if (user == null)
                return 0;

            user.ForgotPasswordToken = resetPasswordToken;
            var result = await _appDbContext.SaveChangesAsync();


            return result;
        }

        public async Task<Boolean> ValidateUpdatePasswordTokenAsync(String token, String userId)
        {
            return await _appDbContext.AppUser
                .AnyAsync(u => (u.Id + "").Equals(userId) && (u.ForgotPasswordToken + "").Equals(token));

        }

       
    }
}
