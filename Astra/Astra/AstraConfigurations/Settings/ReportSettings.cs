using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MTUtil.TypeManagement;

namespace Astra.AstraConfigurations.Settings
{
  public class ReportSettings : ConfigurationSection
  {
    [ConfigurationProperty("reportServerUrl", IsRequired = true, DefaultValue = "http://www.mindtree.com")]
    public string ReportServerUrl
    {
      get
      {
        return this["reportServerUrl"].ToString();
      }
      set
      {
        this["reportServerUrl"] = value;
      }
    }   
  }
}