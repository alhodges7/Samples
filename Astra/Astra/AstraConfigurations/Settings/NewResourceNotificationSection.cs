using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Astra.AstraConfigurations.Settings
{
  public class NewResourceNotificationSection : ConfigurationSection
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
    [ConfigurationProperty("lastNewResourceCheck", IsRequired = false)]
    public string LastNewResourceCheck
    {
      get
      {

        return this["lastNewResourceCheck"].ToString();
      }
      set
      {
        this["lastNewResourceCheck"] = value;
        AstraConfigurationManager.Save();
        ConfigurationManager.RefreshSection("astraServiceSettings/newResourceNotification");
      }
    }
    public override bool IsReadOnly()
    {
      return false;
    }
  }
}