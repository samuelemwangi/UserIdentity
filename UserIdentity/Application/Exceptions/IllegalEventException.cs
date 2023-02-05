namespace UserIdentity.Application.Exceptions
{
    public class IllegalEventException : Exception
    {
        public IllegalEventException(string message) : base(message)
        {

        }

        public IllegalEventException(string operation, string classNme) : base(classNme + ": The event - " + operation + " - is not allowed")
        {
        }
    }
}
