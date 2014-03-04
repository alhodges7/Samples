using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using MTUtil.TypeManagement;

namespace Astra.AstraConfigurations.Settings
{  
  public class SharePointIntegrationSettings : ConfigurationSection
  {
    
    [ConfigurationProperty("url", IsRequired=false)]
    public string SharePointUrl
    {
      get 
      { 
        return TypeUtils.ToString(this["url"]); 
      }
      set 
      {
        this["url"] = value.ToString(); 
      }
    }

    [ConfigurationProperty("useDefaultCredentials", DefaultValue="false", IsRequired=true)]
    public bool UseDefaultCredentials
    {
      get 
      { 
        return TypeUtils.ToBool(this["useDefaultCredentials"]);   
      }
      set 
      { 
        this["useDefaultCredentials"] = value; 
      }
    }
        
    [ConfigurationProperty("Credentials")]
    public SharePointCredentialsSettings SharePointCredentials
    {
      get { return (SharePointCredentialsSettings) this["Credentials"]; }      
    }

    [ConfigurationProperty("SuggestedResources")]
    public SuggestedResourcesSPSettings SuggestedResourcesSettings
    {
      get { return (SuggestedResourcesSPSettings) this["SuggestedResources"]; }      
    }

  } 

}