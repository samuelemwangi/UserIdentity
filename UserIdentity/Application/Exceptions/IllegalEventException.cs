namespace UserIdentity.Application.Exceptions
{
    public class IllegalEventException : Exception
    {
        public IllegalEventException(String message) : base(message)
        {

        }

        public IllegalEventException(String operation, String classNme) : base(classNme + ": The event - " + operation + " - is not allowed")
        {
        }
    }
}
