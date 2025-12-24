using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserIdentity.Domain.Identity;

namespace UserIdentity.Persistence.Configurations.Identity;

public class UserRegisteredAppConfiguration : IEntityTypeConfiguration<UserRegisteredAppEntity>
{
  public void Configure(EntityTypeBuilder<UserRegisteredAppEntity> builder)
  {
    builder.HasKey(e => e.Id);
  }
}
