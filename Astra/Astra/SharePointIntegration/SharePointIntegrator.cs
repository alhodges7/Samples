using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Astra.AstraConfigurations.Settings;
using Astra.Logging;
using Astra.Models;


namespace Astra.SharePointIntegration
{

  /// <summary>
  /// A class for handling all SharePoint integration features between the 
  /// Astra framework an the USDC SharePoint instance.
  /// </summary>
  public class SharePointIntegrator
  {
    private ListsSoapClient _ListsClient;
    private SharePointIntegrationSettings _ConfigSettings;

    /// <summary>
    /// Provides a handle to the AstraConfigurations Settings in Web.Config that
    /// specifically control Sharepoint Integration.
    /// </summary>
    public SharePointIntegrationSettings ConfigSettings
    {
      get 
      {
        if (_ConfigSettings == null)
        {
          _ConfigSettings = (SharePointIntegrationSettings)System.Configuration.ConfigurationManager.GetSection("sharePointIntegration");
        }
        return _ConfigSettings; 
      }      
    }

    /// <summary>
    /// Provides a handle to the Sharepoint Web Service for managing Lists.
    /// </summary>
    public ListsSoapClient ListsClient
    {
      get 
      {
        if (_ListsClient == null)
        {
          AstraLogger.LogDebug("Initializing ListsSoapClient...");
          _ListsClient = new ListsSoapClient();
          if (!this.ConfigSettings.UseDefaultCredentials)
          {
            _ListsClient.ClientCredentials.UserName.UserName = this.ConfigSettings.SharePointCredentials.UserName;
            _ListsClient.ClientCredentials.UserName.Password = this.ConfigSettings.SharePointCredentials.Password;

            if (this.ConfigSettings.SharePointCredentials.ImpersonationLevel != TokenImpersonationLevel.None)
              _ListsClient.ClientCredentials.Windows.AllowedImpersonationLevel = this.ConfigSettings.SharePointCredentials.ImpersonationLevel;
          }
        }
        return _ListsClient; 
      }
      set { _ListsClient = value; }
    }

    /// <summary>
    /// Method to transmit a Resource Suggestion model through to 
    /// the "Suggested Resources" list on the USDC SharePoint server.
    /// </summary>
    /// <param name="sr"></param>
    /// <returns></returns>
    public bool TransmitSuggestedResourceToSharePoint(ResourceSuggestion sr)
    {
      AstraLogger.LogDebug("Inside TransmitSuggestedResourceToSharePoint...");

      bool success = false;
      try
      {
        SuggestedResourceToSP srsp = new SuggestedResourceToSP();
        srsp.ListsClient = this.ListsClient;
        success = srsp.TransmitSuggestedResource(sr);        
      }
      catch (Exception e)
      {
        AstraLogger.LogError(e);
        return false;
      }

      AstraLogger.LogDebug("Leaving TransmitSuggestedResourceToSharePoint...");
      return success;
    }

  }
}