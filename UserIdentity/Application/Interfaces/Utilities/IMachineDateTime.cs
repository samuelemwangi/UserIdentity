namespace UserIdentity.Application.Interfaces.Utilities
{
	public interface IMachineDateTime
	{
		DateTime? DefaultNull { get; }

		DateTime Now { get; }

		string GetTimeStamp();

		string? ResolveDate(DateTime? dateTime);

		long ToUnixEpochDate(DateTime dateTime);

	}
}
