using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using UserIdentity.Domain.WaitLists;

namespace UserIdentity.Persistence.Configurations;

public class WaitListConfiguration : IEntityTypeConfiguration<WaitListEntity>
{
  public void Configure(EntityTypeBuilder<WaitListEntity> builder)
  {
    builder.HasKey(e => e.Id);
    builder.Property(e => e.Id).ValueGeneratedOnAdd();

    builder.HasIndex(e => e.UserEmail);
  }
}
