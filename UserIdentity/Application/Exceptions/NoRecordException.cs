namespace UserIdentity.Application.Exceptions
{
    public class NoRecordException : Exception
    {
        public NoRecordException(string message) : base(message)
        {

        }
        public NoRecordException(string id, string className) : base(className + ": No record exists for the provided identifier - " + id)
        {

        }
    }
}
