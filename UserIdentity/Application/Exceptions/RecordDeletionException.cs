namespace UserIdentity.Application.Exceptions
{
	public class RecordDeletionException : Exception
	{
		public RecordDeletionException(string message) : base(message)
		{

		}

		public RecordDeletionException(string id, string className) : base(className + ": An error occured while deleting a record identified by - " + id)
		{

		}
	}
}
