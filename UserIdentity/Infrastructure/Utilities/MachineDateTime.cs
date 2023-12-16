using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Utilities
{
	public class MachineDateTime : IMachineDateTime
	{
		public static int CurrentYear => DateTime.UtcNow.Year;

		public static int CurrentMonth => DateTime.UtcNow.Month;

		public DateTime? DefaultNull => null;

		public DateTime Now => DateTime.UtcNow;

		public string GetTimeStamp()
		{
			DateTime dateTime = Now;

			return dateTime.ToString("yyyyMMddHHmmssffff");
		}

		public string? ResolveDate(DateTime? dateTime)
		{
			return dateTime == null ? "" : dateTime?.ToLongDateString();
		}

		public long ToUnixEpochDate(DateTime dateTime)
		{
			return (long)Math.Round((dateTime.ToUniversalTime() -
																											 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
																											.TotalSeconds);
		}
	}
}
