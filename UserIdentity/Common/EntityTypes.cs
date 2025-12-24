using System.ComponentModel;

namespace UserIdentity.Common;

public enum EntityTypes
{
  [Description("User")]
  USER,

  [Description("Role")]
  ROLE,

  [Description("RoleClaims")]
  ROLE_CLAIM,

  [Description("RefreshToken")]
  REFRESH_TOKEN
}
