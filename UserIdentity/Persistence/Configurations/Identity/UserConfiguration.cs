using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
  public void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(e => e.EmailConfirmationToken).HasMaxLength(600);

    builder.Property(e => e.FirstName).HasMaxLength(20);

    builder.Property(e => e.LastName).HasMaxLength(20);

    builder.Property(e => e.ForgotPasswordToken).HasMaxLength(600);
  }
}
