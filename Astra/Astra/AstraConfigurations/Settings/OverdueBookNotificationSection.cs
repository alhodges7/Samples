using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Astra.AstraConfigurations.Settings
{
  public class OverdueBookNotificationSection : ConfigurationSection
  {
    [ConfigurationProperty("enableService", IsRequired = true)]
    public string EnableService
    {
      get
      {
        return this["enableService"].ToString();
      }
      set
      {
        this["enableService"] = value;
      }
    }

    [ConfigurationProperty("overdueBookDeadline", IsRequired = true)]
    public double OverdueBookDeadline
    {
      get
      {
        return (double)this["overdueBookDeadline"];
      }
      set
      {
        this["overdueBookDeadline"] = value;
      }
    }

    [ConfigurationProperty("serviceFrequency", IsRequired = true)]
    public double ServiceFrequency
    {
      get
      {
        return (double)this["serviceFrequency"];
      }
      set
      {
        this["serviceFrequency"] = value;
      }
    }

    [ConfigurationProperty("lastOverdueCheck", IsRequired = false)]
    public string LastOverdueCheck
    {
      get
      {
        return this["lastOverdueCheck"].ToString();
      }
      set
      {
        this["lastOverdueCheck"] = value;
        AstraConfigurationManager.Save();
        ConfigurationManager.RefreshSection("astraServiceSettings/overdueBookNotification");
      }
    }

    public override bool IsReadOnly()
    {
      return false;
    }

  }
}