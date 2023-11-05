namespace UserIdentity.Application.Interfaces.Utilities
{
  public interface IMachineDateTime
  {
    DateTime? DefaultNull { get; }

    DateTime Now { get; }

    String GetTimeStamp();

    String? ResolveDate(DateTime? dateTime);

    Int64 ToUnixEpochDate(DateTime dateTime);

  }
}
