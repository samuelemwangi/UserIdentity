using System.ComponentModel;

namespace UserIdentity.Application.Enums;

public enum RequestSource
{
  [Description("UI")]
  UI,

  [Description("API")]
  API
}
