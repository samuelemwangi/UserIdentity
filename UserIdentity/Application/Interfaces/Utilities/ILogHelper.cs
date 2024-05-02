namespace UserIdentity.Application.Interfaces.Utilities
{
	public interface ILogHelper<T>
	{
		Task LogEventAsync(string message, LogLevel logLevel);
	}
}
