namespace UserIdentity.Application.Interfaces.Utilities
{
	public interface ILogHelper<T>
	{
		void LogEvent(string message, LogLevel logLevel);
	}
}
