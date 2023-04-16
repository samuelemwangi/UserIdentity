namespace UserIdentity.Application.Interfaces.Utilities
{
	public interface ILogHelper<T>
	{
		void LogEvent(String message, LogLevel logLevel);
	}
}
