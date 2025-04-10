using System.Text.Json;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UserIdentity.Persistence.Configurations.Utilities;

public class ConverterUtil
{
	public static ValueConverter<Dictionary<string, string>?, string> DictionaryConverter = new(
						v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
						v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)
						);
}
