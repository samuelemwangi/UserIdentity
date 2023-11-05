namespace UserIdentity.Application.Exceptions
{
  public class RecordCreationException : Exception
  {
    public RecordCreationException(String message) : base(message)
    {

    }

    public RecordCreationException(String id, String className) : base(className + ": An error occured while creating a record identified by - " + id)
    {

    }
  }
}
