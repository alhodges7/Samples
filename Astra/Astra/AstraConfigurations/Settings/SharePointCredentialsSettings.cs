using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Security.Principal;
using MTUtil.TypeManagement;

namespace Astra.AstraConfigurations.Settings
{
  public class SharePointCredentialsSettings : ConfigurationElement
  {
    [ConfigurationProperty("userName")]
    public string UserName
    {
      get
      {
        return TypeUtils.ToString(this["userName"]);
      }
      set
      {
        this["userName"] = value.ToString();
      }
    }

    [ConfigurationProperty("password")]
    public string Password
    {
      get
      {
        return TypeUtils.ToString(this["password"]);
      }
      set
      {
        this["password"] = value.ToString();
      }
    }

    
    [ConfigurationProperty("impersonationLevel")]
    public TokenImpersonationLevel ImpersonationLevel
    {
      get 
      { 
        string level = TypeUtils.ToString(this["impersonationLevel"]); 
        if(string.IsNullOrEmpty(level))
          return TokenImpersonationLevel.None;
        else
          return TypeUtils.ToEnum<TokenImpersonationLevel>(level);
      }
      set { this["impersonationLevel"] = value.ToString(); }
    }


  }
}