namespace UserIdentity.Application.Exceptions
{
	public class InvalidOperationException : Exception
	{
		public InvalidOperationException(string message) : base(message)
		{

		}

		public InvalidOperationException(string operation, string classNme)
		{
			throw new InvalidOperationException(classNme + ": The operation - " + operation + " - is not allowed");
		}
	}
}
