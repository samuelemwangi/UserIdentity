namespace UserIdentity.Application.Exceptions
{
	public class RecordUpdateException : Exception
	{
		public RecordUpdateException(String message) : base(message)
		{

		}

		public RecordUpdateException(String id, String className) : base(className + ": An error occured while updating a record identified by - " + id)
		{

		}
	}
}
