using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using Astra.AstraConfigurations.Settings;
using Astra.Logging;


namespace Astra.AstraConfigurations
{
  public static class AstraConfigurationManager
  {
    private static OverdueBookNotificationSection _overdueBooks;
    private static NewResourceNotificationSection _newResources;
    private static System.Configuration.Configuration _configuration;

    /// <summary>
    /// Gets the OverdueBooks Configuration Settings
    /// </summary>
    /// <returns></returns>
    public static OverdueBookNotificationSection OverdueBookSettings()
    {
      if (_configuration == null)
      {
        _configuration = WebConfigurationManager.OpenWebConfiguration("~") as System.Configuration.Configuration;
      }
      if (_overdueBooks == null)
      {
        _overdueBooks = (OverdueBookNotificationSection)_configuration.GetSection("astraServiceSettings/overdueBookNotification");
      }
      return _overdueBooks;
    }

    /// <summary>
    /// Gets the NewResource Configuration Settings
    /// </summary>
    /// <returns>NewResourceNotificationSection</returns>
    public static NewResourceNotificationSection NewResourceSettings()
    {
      if (_configuration == null)
      {
        _configuration = WebConfigurationManager.OpenWebConfiguration("~") as System.Configuration.Configuration;
      }
      if (_newResources == null)
      {
        _newResources = (NewResourceNotificationSection)_configuration.GetSection("astraServiceSettings/newResourceNotification");
      }
      return _newResources;
    }

    /// <summary>
    /// Saves any changes to the config.xml
    /// </summary>
    public static void Save()
    {
      if (_configuration != null)
      {
        try
        {
          _configuration.Save(ConfigurationSaveMode.Modified);
        }
        catch (Exception e)
        {
          AstraLogger.LogError(e);
        }
       
      }
    }
  }
}