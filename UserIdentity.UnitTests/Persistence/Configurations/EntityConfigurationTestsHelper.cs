using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UserIdentity.UnitTests.Persistence.Configurations
{
  internal class EntityConfigurationTestsHelper<T> where T : class
  {

    public static (EntityTypeBuilder<T>, EntityType) GetEntityTypeBuilder()
    {
      EntityType enityType = new EntityType(nameof(T), typeof(T), new Model(), false, ConfigurationSource.Convention);
      var builder = new EntityTypeBuilder<T>(enityType);
      return (builder, enityType);
    }

  }
}
