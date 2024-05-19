using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Utilities
{
	public class FileSystemKeyProvider : IKeyProvider
	{
		public async Task<string> GetKeyAsync(string keyName)
		{
			var path = keyName.Split("/").Aggregate(Path.Combine(Environment.CurrentDirectory), Path.Combine);

			if (!File.Exists(path))
			{
				throw new FileNotFoundException($"File not found at {path}");
			}

			return await File.ReadAllTextAsync(path);
		}
	}
}
