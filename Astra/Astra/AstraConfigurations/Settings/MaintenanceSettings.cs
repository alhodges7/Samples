using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MTUtil.TypeManagement;

namespace Astra.AstraConfigurations.Settings
{
  public class MaintenanceSettings : ConfigurationSection
  {

    [ConfigurationProperty("purgeDebugLogDays", IsRequired = false, DefaultValue="30")]
    public int PurgeDebugLogDays
    {
      get
      {
        int days = TypeUtils.ToInt(this["purgeDebugLogDays"]);
        if (days <= 0)
          days = 30;
        return days;
      }
      set
      {
        this["purgeDebugLogDays"] = value;
      }
    }   

    [ConfigurationProperty("purgeInfoLogDays", IsRequired = false, DefaultValue="60")]
    public int PurgeInfoLogDays
    {
      get
      {
        int days = TypeUtils.ToInt(this["purgeInfoLogDays"]);
        if (days <= 0)
          days = 60;
        return days;
      }
      set
      {
        this["purgeInfoLogDays"] = value;
      }
    }

    [ConfigurationProperty("purgeErrorLogDays", IsRequired = false, DefaultValue="120")]
    public int PurgeErrorLogDays
    {
      get
      {
        int days = TypeUtils.ToInt(this["purgeErrorLogDays"]);
        if (days <= 0)
          days = 120;
        return days;
      }
      set
      {
        this["purgeErrorLogDays"] = value;
      }
    }

  }
}