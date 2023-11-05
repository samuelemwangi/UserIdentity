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

    public async Task<User?> GetUserAsync(String? Id)
    {
      return await _appDbContext.AppUser
          .Where(u => (u.Id + "").Equals(Id) && !u.IsDeleted)
          .FirstOrDefaultAsync();
    }

    public async Task<Int32> UpdateResetPasswordTokenAsync(String userId, String resetPasswordToken)
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

    public async Task<Boolean> ValidateUpdatePasswordTokenAsync(String userId, String token)
    {
      return await _appDbContext.AppUser
          .AnyAsync(u => (u.Id + "").Equals(userId) && (u.ForgotPasswordToken + "").Equals(token) && !u.IsDeleted);
    }

  }
}
