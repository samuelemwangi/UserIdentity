using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PolyzenKit.Persistence.Configurations;

using UserIdentity.Domain.Identity;
using UserIdentity.Persistence.Configurations.Utilities;

namespace UserIdentity.Persistence.Configurations.Identity;

public class RegisteredAppConfiguration : IEntityTypeConfiguration<RegisteredAppEntity>
{
	public void Configure(EntityTypeBuilder<RegisteredAppEntity> builder)
	{
		builder.ConfigureIntPrimaryKey();
		builder.HasIndex(e => e.AppName);

		builder.Property(e => e.CallbackHeaders)
			.HasConversion(ConverterUtil.DictionaryConverter)
			.HasMaxLength(800);
	}
}
