using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using UserIdentity.Domain.Identity;
using UserIdentity.Infrastructure.Security.Helpers;
using UserIdentity.IntegrationTests.TestUtils;
using UserIdentity.Persistence;

namespace UserIdentity.IntegrationTests.Persistence
{
	internal class DBContexUtils
	{


		public static void SeedIdentityUser(AppDbContext appDbContext)
		{
			var user = new IdentityUser
			{
				UserName = UserSettings.UserName,
				PhoneNumber = UserSettings.PhoneNumber,
				Id = UserSettings.UserId.ToString(),
				Email = UserSettings.UserEmail,
				EmailConfirmed = true,
				NormalizedEmail = UserSettings.UserEmail.ToUpper(),
				NormalizedUserName = UserSettings.UserName.ToUpper(),
				ConcurrencyStamp = DateTime.Now.Ticks.ToString(),
			};

			user.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(user, UserSettings.UserPassword);

			appDbContext.Users.Add(user);
			appDbContext.SaveChanges();
		}

		public static void SeedIdentityRole(AppDbContext appDbContext, string roleId = "", string roleName = "")
		{
			var role = new IdentityRole
			{
				Id = RoleSettings.RoleId,
				Name = RoleSettings.RoleName,
				NormalizedName = RoleSettings.RoleName.ToUpper(),
				ConcurrencyStamp = DateTime.Now.Ticks.ToString()
			};

			if (!string.IsNullOrEmpty(roleId))
				role.Id = roleId;

			if (!string.IsNullOrEmpty(roleName))
			{
				role.Name = roleName;
				role.NormalizedName = roleName.ToUpper();
			}

			appDbContext.Roles.Add(role);
			appDbContext.SaveChanges();
		}

		public static void SeedIdentityUserRole(AppDbContext appDbContext, string roleId = "")
		{
			var userRole = new IdentityUserRole<string>
			{
				RoleId = RoleSettings.RoleId,
				UserId = UserSettings.UserId.ToString()
			};

			if (!string.IsNullOrEmpty(roleId))
				userRole.RoleId = roleId;

			appDbContext.UserRoles.Add(userRole);
			appDbContext.SaveChanges();
		}

		public static void SeedAppUser(AppDbContext appDbContext)
		{
			var appuser = new User
			{
				Id = UserSettings.UserId.ToString(),
				FirstName = UserSettings.FirstName,
				LastName = UserSettings.LastName,
				CreatedBy = UserSettings.UserId.ToString(),
				CreatedAt = DateTime.UtcNow,
				UpdatedBy = UserSettings.UserId.ToString(),
				UpdatedAt = DateTime.UtcNow
			};

			appDbContext.AppUser.Add(appuser);
			appDbContext.SaveChanges();
		}

		public static void SeedRefreshToken(AppDbContext appDbContext)
		{
			var refreshToken = new RefreshToken
			{
				UserId = UserSettings.UserId.ToString(),
				CreatedBy = UserSettings.UserId.ToString(),
				CreatedAt = DateTime.UtcNow,
				UpdatedBy = UserSettings.UserId.ToString(),
				UpdatedAt = DateTime.UtcNow
			};

			appDbContext.RefreshToken.Add(refreshToken);
			appDbContext.SaveChanges();
		}

		public static string? UpdateResetPasswordToken(AppDbContext appDbContext, UserManager<IdentityUser> userManager)
		{
			var user = userManager.FindByIdAsync(UserSettings.UserId.ToString()).Result;

			if (user == null)
				return null as string;


			var resetPasswordToken = userManager.GeneratePasswordResetTokenAsync(user).Result;

			if (resetPasswordToken == null)
				return null as string;

			var appuser = appDbContext.AppUser.Where(e => e.Id == UserSettings.UserId.ToString()).FirstOrDefault();

			if (appuser == null)
				return null as string;

			appuser.ForgotPasswordToken = resetPasswordToken;

			appDbContext.SaveChanges();

			return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));
		}

		public static bool? UpdateRoleClaim(AppDbContext appDbContext, RoleManager<IdentityRole> roleManager)
		{
			var role = roleManager.FindByIdAsync(RoleSettings.RoleId).Result;

			var roleClaim = new Claim(Constants.Strings.JwtClaimIdentifiers.Scope, ScopeClaimSettings.ScopeClaim);

			var result = roleManager.AddClaimAsync(role, roleClaim);

			return result.Result.Succeeded;
		}

		public static void SeedDatabase(AppDbContext appDbContext)
		{
			SeedIdentityUser(appDbContext);
			SeedIdentityRole(appDbContext);
			SeedIdentityUserRole(appDbContext);
			SeedAppUser(appDbContext);
			SeedRefreshToken(appDbContext);
		}

		public static void ClearAppUser(AppDbContext appDbContext)
		{
			appDbContext.RemoveRange(appDbContext.AppUser);
			appDbContext.SaveChanges();
		}

		public static void DeleteAppUser(AppDbContext appDbContext)
		{
			var appuser = appDbContext.AppUser.Where(e => e.Id == UserSettings.UserId.ToString()).FirstOrDefault();

			if (appuser == null)
				return;
			appuser.IsDeleted = true;
			appDbContext.SaveChanges();
		}

		public static void ClearRefreshToken(AppDbContext appDbContext)
		{
			appDbContext.RemoveRange(appDbContext.RefreshToken);
			appDbContext.SaveChanges();
		}

		public static void DeleteRefreshToken(AppDbContext appDbContext)
		{
			var refreshToken = appDbContext.RefreshToken.Where(e => e.UserId == UserSettings.UserId.ToString()).FirstOrDefault();
			if (refreshToken == null)
				return;
			refreshToken.IsDeleted = true;
			appDbContext.SaveChanges();
		}

		public static void ClearDatabase(AppDbContext appDbContext)
		{
			appDbContext.RemoveRange(appDbContext.UserRoles);
			appDbContext.RemoveRange(appDbContext.Roles);
			appDbContext.RemoveRange(appDbContext.Users);

			appDbContext.RemoveRange(appDbContext.RoleClaims);
			appDbContext.RemoveRange(appDbContext.UserClaims);

			appDbContext.RemoveRange(appDbContext.AppUser);
			appDbContext.RemoveRange(appDbContext.RefreshToken);

			appDbContext.SaveChanges();
		}
	}
}
