using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.ExtensionMethods
{
  public static class MyExtensions
  {
    public static string ToStandardFormat(this DateTime someDateTime)
    {
      return Astra.Helper.OtherHelpers.StandardDateTimeFormat(someDateTime);
    }
  }
}