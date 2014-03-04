using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using MTUtil.TypeManagement;
using Astra.Repositories;

namespace Astra.AstraConfigurations.Settings
{
  public class EmailSettings : ConfigurationSection
  {
    [ConfigurationProperty("smtpServer", IsRequired = true)]
    public string SmtpServer
    {
      get
      {
        return this["smtpServer"].ToString();
      }
      set
      {
        this["smtpServer"] = value;
      }
    }

    [ConfigurationProperty("useDefaultCredentials", IsRequired = true, DefaultValue="true")]
    public bool UseDefaultCredentials
    {
      get
      {
        return TypeUtils.ToBool(this["useDefaultCredentials"].ToString());
      }
      set
      {
        this["useDefaultCredentials"] = value.ToString();
      }
    }

    [ConfigurationProperty("enableSSL", DefaultValue = "true")]
    public bool EnableSSL
    {
      get
      {
        return TypeUtils.ToBool(this["enableSSL"].ToString());
      }
      set
      {
        this["enableSSL"] = value.ToString();
      }
    }


    [ConfigurationProperty("userName", IsRequired = true)]
    public string UserName
    {
      get
      {
        return this["userName"].ToString();
      }
      set
      {
        this["userName"] = value;
      }
    }


    [ConfigurationProperty("password", IsRequired = true)]
    public string Password
    {
      get
      {
        return this["password"].ToString();
      }
      set
      {
        this["password"] = value;
      }
    }


    [ConfigurationProperty("fromAddress", IsRequired = false, DefaultValue = "astra@mindtree.com")]
    public string FromAddress
    {
      get
      {
        return this["fromAddress"].ToString();
      }
      set
      {
        this["fromAddress"] = value;
      }
    }

    [ConfigurationProperty("debugToAddress", IsRequired = false, DefaultValue = "Fernando_Marrero@mindtree.com")]
    public string DebugToAddress
    {
      get
      {
        return this["debugToAddress"].ToString();
      }
      set
      {
        this["debugToAddress"] = value;
      }
    }


    [ConfigurationProperty("portNumber", IsRequired = false)]
    public int PortNumber
    {
      get
      {
        return TypeUtils.ToInt(this["portNumber"].ToString());
      }
      set
      {
        this["portNumber"] = value.ToString();
      }
    }

    [ConfigurationProperty("deliveryMethod", IsRequired = false, DefaultValue="Network")]
    public SmtpDeliveryMethod DeliveryMethod
    {
      get
      {
        return TypeUtils.ToEnum<SmtpDeliveryMethod>(this["deliveryMethod"].ToString());
      }
      set
      {
        this["deliveryMethod"] = value.ToString();
      }
    }

    [ConfigurationProperty("emailSystemMode", IsRequired = true, DefaultValue = "Off")]
    public EmailSystemMode EmailSystemMode
    {
      get
      {
        return TypeUtils.ToEnum<EmailSystemMode>(this["emailSystemMode"].ToString());
      }
      set
      {
        this["emailSystemMode"] = value.ToString();
      }
    }   


  }
}