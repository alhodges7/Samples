using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using MTUtil.TypeManagement;

namespace Astra.AstraConfigurations.Settings
{
  public class SuggestedResourcesSPSettings : ConfigurationElement
  {
    [ConfigurationProperty("passThroughSuggestions", DefaultValue = "false", IsRequired = false)]
    public bool PassThroughSuggestions
    {
      get
      {
        return TypeUtils.ToBool(this["passThroughSuggestions"]);
      }
      set
      {
        this["passThroughSuggestions"] = value;
      }
    }
  }
}