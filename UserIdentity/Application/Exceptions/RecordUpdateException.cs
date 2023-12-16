namespace UserIdentity.Application.Exceptions
{
	public class RecordUpdateException : Exception
	{
		public RecordUpdateException(string message) : base(message)
		{

		}

		public RecordUpdateException(string id, string className) : base(className + ": An error occured while updating a record identified by - " + id)
		{

		}
	}
}
