﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using PolyzenKit.Domain.AppEntities;
using PolyzenKit.Persistence.Configurations;

using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Configurations.Identity;

namespace UserIdentity.Persistence
{
	public class AppDbContext(
		DbContextOptions<AppDbContext> dbContextOptions
		) : IdentityDbContext<IdentityUser>(dbContextOptions)
	{
		private readonly string _entityKeyPrefix = "";

		public DbSet<User> AppUser { get; internal set; }

		public DbSet<RefreshToken> RefreshToken { get; internal set; }

		public DbSet<AppEntity> AppEntity { get; internal set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
			modelBuilder.ApplyConfiguration(new AppEntityConfiguration());

			modelBuilder.Entity<User>().ToTable(_entityKeyPrefix + "user_details");
			modelBuilder.Entity<RefreshToken>().ToTable(_entityKeyPrefix + "refresh_tokens");
			modelBuilder.Entity<AppEntity>().ToTable(_entityKeyPrefix + "app_entities");

			modelBuilder.Entity<IdentityUser>().ToTable(_entityKeyPrefix + "users");
			modelBuilder.Entity<IdentityRole>().ToTable(_entityKeyPrefix + "roles");
			modelBuilder.Entity<IdentityUserClaim<string>>().ToTable(_entityKeyPrefix + "user_claims");
			modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable(_entityKeyPrefix + "role_claims");
			modelBuilder.Entity<IdentityUserLogin<string>>().ToTable(_entityKeyPrefix + "user_logins").HasKey(i => new { i.LoginProvider, i.ProviderKey });
			modelBuilder.Entity<IdentityUserRole<string>>().ToTable(_entityKeyPrefix + "user_roles").HasKey(i => new { i.UserId, i.RoleId });
			modelBuilder.Entity<IdentityUserToken<string>>().ToTable(_entityKeyPrefix + "user_tokens").HasKey(i => new { i.UserId, i.LoginProvider, i.Name });
		}
	}
}
