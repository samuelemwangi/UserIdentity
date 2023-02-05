namespace UserIdentity.Application.Exceptions
{
	public class RecordDeletionException: Exception
	{
		public RecordDeletionException(String message) : base(message)
		{

		}

		public RecordDeletionException(String id, String className) 
		{
			throw new RecordDeletionException(className + ": An error occured while deleting a record identified by - " + id);
		}
	}
}
