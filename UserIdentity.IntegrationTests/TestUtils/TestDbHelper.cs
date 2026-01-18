using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using PolyzenKit.Common.Utilities;
using PolyzenKit.Infrastructure.Security.Jwt;
using PolyzenKit.Presentation.Settings;

using UserIdentity.Domain.RefreshTokens;
using UserIdentity.Domain.Users;
using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests.TestUtils;

public sealed class TestDbHelper(
  IConfiguration configuration,
  AppDbContext appDbContext,
  UserManager<IdentityUser> userManager,
  RoleManager<IdentityRole> roleManager
  )
{

  private readonly RoleSettings _roleSettings = configuration.GetSetting<RoleSettings>();
  private readonly AppDbContext _appDbContext = appDbContext;
  private readonly UserManager<IdentityUser> _userManager = userManager;
  private readonly RoleManager<IdentityRole> _roleManager = roleManager;

  public string DefaultRole => $"{_roleSettings.ServiceName.ToLower()}{ZenConstants.SERVICE_ROLE_SEPARATOR}{_roleSettings.DefaultRole}";
  public HashSet<string> AdminRoles => [.. _roleSettings.AdminRoles.Split(',').Select(r => $"{_roleSettings.ServiceName.ToLower()}{ZenConstants.SERVICE_ROLE_SEPARATOR}{r.Trim()}")];

  #region IdentityUser Related
  public IdentityUser CreateIdentityUser()
  {

    DeleteIdentityUser(UserSettingHelper.UserId);

    var user = new IdentityUser
    {
      UserName = UserSettingHelper.UserName,
      PhoneNumber = UserSettingHelper.PhoneNumber,
      Id = UserSettingHelper.UserId,
      Email = UserSettingHelper.UserEmail,
      EmailConfirmed = true,
      NormalizedEmail = UserSettingHelper.UserEmail.ToUpper(),
      NormalizedUserName = UserSettingHelper.UserName.ToUpper(),
      ConcurrencyStamp = DateTime.Now.Ticks.ToString(),
    };

    user.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(user, UserSettingHelper.UserPassword);

    _appDbContext.Users.Add(user);
    _appDbContext.SaveChanges();

    return user;
  }

  public void UpdateIdentityUser(IdentityUser user)
  {
    _appDbContext.Users.Update(user);
    _appDbContext.SaveChanges();
  }

  public IdentityUser? GetIdentityUser(string userId)
  {
    return _appDbContext.Users.FirstOrDefault(u => u.Id == userId);
  }

  public IdentityUser? GetIdentityUserWithNoTracking(string userId)
  {
    return _appDbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
  }

  public void DeleteIdentityUser(string userId)
  {
    var user = GetIdentityUser(userId);

    if (user != null)
    {
      _appDbContext.Users.Remove(user);
      _appDbContext.SaveChanges();
    }
  }

  #endregion


  #region IndentityRole Related
  public IdentityRole CreateIdentityRole(string roleName)
  {
    DeleteIdnetityRole(roleName);
    var role = new IdentityRole
    {
      Name = roleName,
      NormalizedName = roleName.ToUpper()
    };

    _appDbContext.Roles.Add(role);
    _appDbContext.SaveChanges();

    return role;
  }

  public void UpdateIdentityRole(IdentityRole role)
  {
    _appDbContext.Roles.Update(role);
    _appDbContext.SaveChanges();
  }

  public IdentityRole? GetIdentityRole(string roleName)
  {
    return _appDbContext.Roles.FirstOrDefault(r => r.Name != null && r.Name.Equals(roleName));
  }

  public void DeleteIdnetityRole(string roleName)
  {
    var role = GetIdentityRole(roleName);

    if (role != null)
    {
      _appDbContext.Remove(role);
      _appDbContext.SaveChanges();
    }
  }

  #endregion

  #region IdentityUserRole Related
  public IdentityUserRole<string> CreateIdentityUserRole(string roleName, string userId)
  {
    var existingRole = GetIdentityRole(roleName) ?? throw new ArgumentNullException(nameof(roleName));
    var existingUser = GetIdentityUser(userId) ?? throw new ArgumentNullException(nameof(userId));

    var userRole = new IdentityUserRole<string>
    {
      RoleId = existingRole.Id,
      UserId = existingUser.Id
    };

    _appDbContext.UserRoles.Add(userRole);
    _appDbContext.SaveChanges();

    return userRole;
  }

  public IdentityUserRole<string> ConfigureIdentityUserAsAdmin()
  {
    return CreateIdentityUserRole(AdminRoles.First(), UserSettingHelper.UserId);
  }

  public IdentityUserRole<string>? GetIdentityUserRole(string roleName, string userId)
  {
    var role = GetIdentityRole(roleName);
    if (role != null)
    {
      return _appDbContext.UserRoles.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == role.Id);
    }
    return null;
  }

  public void DeleteIdentityUserRole(string roleName, string userId)
  {
    var userRole = GetIdentityUserRole(roleName, userId);

    if (userRole != null)
    {
      _appDbContext.Remove(userRole);
      _appDbContext.SaveChanges();
    }
  }

  #endregion

  #region UserEntity Related 
  public UserEntity CreateUserEntity()
  {
    DeleteUserEntity(UserSettingHelper.UserId);

    var user = new UserEntity
    {
      Id = UserSettingHelper.UserId.ToString(),
      FirstName = UserSettingHelper.FirstName,
      LastName = UserSettingHelper.LastName,
      CreatedBy = UserSettingHelper.UserId.ToString(),
      CreatedAt = DateTime.UtcNow,
      UpdatedBy = UserSettingHelper.UserId.ToString(),
      UpdatedAt = DateTime.UtcNow
    };

    _appDbContext.AppUser.Add(user);
    _appDbContext.SaveChanges();

    return user;
  }

  public UserEntity? GetUserEntity(string userId)
  {
    return _appDbContext.AppUser.FirstOrDefault(x => x.Id == userId);
  }

  public void DeleteUserEntity(string userId)
  {
    var user = GetUserEntity(userId);

    if (user != null)
    {
      _appDbContext.AppUser.Remove(user);
      _appDbContext.SaveChanges();
    }

  }

  #endregion

  #region RefreshTokenEntity Related 
  public RefreshTokenEntity CreateRefreshTokenEntity()
  {
    DeleteRefreshTokenEntity(UserSettingHelper.UserId);

    var refreshToken = new RefreshTokenEntity
    {
      Id = Guid.CreateVersion7(),
      UserId = UserSettingHelper.UserId.ToString(),
      Token = StringUtil.GenerateRandomString(32),
      CreatedBy = UserSettingHelper.UserId.ToString(),
      CreatedAt = DateTime.UtcNow,
      UpdatedBy = UserSettingHelper.UserId.ToString(),
      UpdatedAt = DateTime.UtcNow
    };

    _appDbContext.RefreshToken.Add(refreshToken);
    _appDbContext.SaveChanges();

    return refreshToken;
  }


  public RefreshTokenEntity? GetRefreshTokenEntity(string userId)
  {
    return _appDbContext.RefreshToken.FirstOrDefault(r => r.UserId == userId);
  }

  public void DeleteRefreshTokenEntity(string userId)
  {
    var refreshToken = GetRefreshTokenEntity(userId);

    if (refreshToken != null)
    {
      _appDbContext.RefreshToken.Remove(refreshToken);
      _appDbContext.SaveChanges();
    }
  }

  #endregion

  #region  Helper Methods 
  public string? UpdateResetPasswordToken(string userId)
  {
    var user = GetIdentityUser(userId);

    if (user == null)
      return null as string;


    var resetPasswordToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;

    if (resetPasswordToken == null)
      return null as string;

    var appuser = GetUserEntity(userId);

    if (appuser == null)
      return null as string;

    appuser.ForgotPasswordToken = resetPasswordToken;

    _appDbContext.Update(appuser);
    _appDbContext.SaveChanges();

    return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));
  }

  public bool? UpdateRoleClaim(string roleName, string scopeClaim)
  {
    var role = GetIdentityRole(roleName) ?? throw new ArgumentNullException(nameof(roleName));

    var roleClaim = new Claim(JwtCustomClaimNames.Scope, scopeClaim);

    var result = _roleManager.AddClaimAsync(role, roleClaim);

    return result.Result.Succeeded;
  }

  public void SeedDatabase()
  {
    var identityUser = CreateIdentityUser();
    // We know the default identity role exists - see DbInitializer SeedRolesData method
    CreateIdentityUserRole(DefaultRole, identityUser.Id);
    CreateUserEntity();
    CreateRefreshTokenEntity();
  }

  #endregion

}
