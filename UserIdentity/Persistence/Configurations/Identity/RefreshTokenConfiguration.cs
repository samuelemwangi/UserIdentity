using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Configurations.Identity
{
  public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
  {
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
      builder.Property(e => e.Token).HasMaxLength(200);
      builder.HasIndex(e => e.Token);

      builder.Property(e => e.UserId).HasMaxLength(50);
      builder.HasIndex(e => e.UserId);

      builder.Property(e => e.RemoteIpAddress).HasMaxLength(20);
    }
  }
}
