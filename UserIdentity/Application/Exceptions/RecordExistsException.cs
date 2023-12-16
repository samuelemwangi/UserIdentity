namespace UserIdentity.Application.Exceptions
{
	public class RecordExistsException : Exception
	{
		public RecordExistsException(string message) : base(message)
		{

		}

		public RecordExistsException(string id, string className) : base(className + ": A record identified with - " + id + " - exists")
		{

		}
	}
}
