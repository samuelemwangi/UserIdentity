using System.ComponentModel;

namespace UserIdentity.Application.Enums
{
  public enum RequestStatus
  {
    [Description("Request Successful")]
    SUCCESSFUL,

    [Description("Request Failed")]
    FAILED,

    [Description("Request Aborted")]
    ABORTED
  }
}
