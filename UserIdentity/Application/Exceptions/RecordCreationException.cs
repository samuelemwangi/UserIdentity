namespace UserIdentity.Application.Exceptions
{
	public class RecordCreationException : Exception
	{
		public RecordCreationException(string message) : base(message)
		{

		}

		public RecordCreationException(string id, string className) : base(className + ": An error occured while creating a record identified by - " + id)
		{

		}
	}
}
