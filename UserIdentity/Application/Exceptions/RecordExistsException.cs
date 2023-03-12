namespace UserIdentity.Application.Exceptions
{
	public class RecordExistsException : Exception
	{
		public RecordExistsException(String message) : base(message)
		{

		}

		public RecordExistsException(String id, String className) : base(className + ":A record identified with - " + id + " - exists")
		{

		}
	}
}
