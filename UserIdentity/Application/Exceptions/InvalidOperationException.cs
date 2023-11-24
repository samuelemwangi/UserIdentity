namespace UserIdentity.Application.Exceptions
{
	public class InvalidOperationException : Exception
	{
		public InvalidOperationException(String message) : base(message)
		{

		}

		public InvalidOperationException(String operation, String classNme) : base(classNme + ": The operation - " + operation + " - is not allowed")
		{

		}
	}
}
