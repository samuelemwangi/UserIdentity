using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserIdentity.Domain.InviteCodes;

namespace UserIdentity.Persistence.Configurations;

public class InviteCodeConfiguration : IEntityTypeConfiguration<InviteCodeEntity>
{
  public void Configure(EntityTypeBuilder<InviteCodeEntity> builder)
  {
    builder.HasKey(e => e.Id);
    builder.Property(e => e.Id).ValueGeneratedOnAdd();
  }
}
