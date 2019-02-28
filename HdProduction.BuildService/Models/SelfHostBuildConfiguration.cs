using System;

namespace HdProduction.BuildService.Models
{
  [Flags]
  public enum SelfHostBuildConfiguration
  {
    MySql = 1,
    SqlServer = 2,
    PostgresSql = 4,
    Sqlite = 8,
  }
}