using UserIdentity.Application.Interfaces.Utilities;

namespace UserIdentity.Infrastructure.Utilities
{
	public class MachineDateTime : IMachineDateTime
	{
		public static Int32 CurrentYear => DateTime.UtcNow.Year;

		public static Int32 CurrentMonth => DateTime.UtcNow.Month;

		public DateTime? DefaultNull => null;

		public DateTime Now => DateTime.UtcNow;

		public String GetTimeStamp()
		{
			DateTime dateTime = Now;

			return dateTime.ToString("yyyyMMddHHmmssffff");
		}

		public String? ResolveDate(DateTime? dateTime)
		{
			return dateTime == null ? "" : dateTime?.ToLongDateString();
		}

		public Int64 ToUnixEpochDate(DateTime dateTime)
		{
			return (Int64)Math.Round((dateTime.ToUniversalTime() -
															 new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
															.TotalSeconds);
		}
	}
}
