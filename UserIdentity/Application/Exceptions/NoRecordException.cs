namespace UserIdentity.Application.Exceptions
{
	public class NoRecordException : Exception
	{
		public NoRecordException(String message) : base(message)
		{

		}
		public NoRecordException(String id, String className) : base(className + ": No record exists for the provided identifier - " + id)
		{

		}
	}
}
