using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using System.IO;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.AstraConfigurations.Settings;
using Astra.Logging;

namespace Astra.Repositories
{

  public enum EmailSystemMode
  {
    Off,
    Active,
    Debug
  }

  public class UserMailMessageRepository : IUserMailMessageRepository
  {
    private AstraContext _context = null;
    private EmailSettings _EmailSettings;    
    
    public EmailSettings EmailSettings
    {
      get 
      {
        if (_EmailSettings == null)
        {
          _EmailSettings = (EmailSettings)System.Configuration.ConfigurationManager.GetSection("emailSettings");
        }
        return _EmailSettings; 
      }
      set { _EmailSettings = value; }
    }

    public UserMailMessageRepository(AstraContext context)
    {
      _context = context;
    }

    public void SendSystemMessage(string message, string recipientMid, string emailMessage)
    {
      if (string.IsNullOrEmpty(emailMessage))
      {
       emailMessage = "You have received a new message in your Astra Inbox. Please login to Astra at http://e2ms00002:8042/UserMessages to view this message";
      }
      
      _context.UserMessages.Add(new Models.UserMailMessage()
      {
        Type = Models.UserMailMessageType.SystemToUser,
        MessageContent = message,
        RecipientMid = recipientMid
      });
      SendEmail(recipientMid, emailMessage);
      _context.SaveChanges();
    }

    public void SendSystemMessage(string message, List<string> recipients, string emailMessage)
    {
      foreach (string recipientMid in recipients)
      {
        SendSystemMessage(message, recipientMid, emailMessage);
      }
    }

    public string GetUserEmail(string mid)
    {
      var profile = _context.UserProfiles.Where(x => x.MID == mid).FirstOrDefault();
      return profile.Email;
    }

    public void SendEmail(string recipientMid, string emailMessageContent)
    {
      if (this.EmailSettings.EmailSystemMode == EmailSystemMode.Off)
        return;

      string actualUserEmailAddr = GetUserEmail(recipientMid);
      if (string.IsNullOrEmpty(actualUserEmailAddr))
        return;
            
      SmtpClient client = new SmtpClient(this.EmailSettings.SmtpServer);

      if (this.EmailSettings.PortNumber > 0)
        client.Port = this.EmailSettings.PortNumber;

      client.EnableSsl = this.EmailSettings.EnableSSL;
      client.UseDefaultCredentials = this.EmailSettings.UseDefaultCredentials;

      if(!this.EmailSettings.UseDefaultCredentials)
        client.Credentials = new NetworkCredential(this.EmailSettings.UserName, this.EmailSettings.Password);

      client.DeliveryMethod = this.EmailSettings.DeliveryMethod;

      AstraLogger.LogDebug("SMTP Server '" + client.Host + "' configured for port " + client.Port.ToString() + ".  Delivery Method is " + client.DeliveryMethod.ToString() +
        ".  UseDefaultCredentials is " + client.UseDefaultCredentials.ToString() + ".  Username is " + this.EmailSettings.UserName + ".");


      MailMessage email = new MailMessage();
      email.From = new MailAddress(this.EmailSettings.FromAddress);
      email.Subject = "Astra";
      email.Body = emailMessageContent;
      email.IsBodyHtml = true;
      email.To.Add(GetSendToAddress(actualUserEmailAddr));

      AstraLogger.LogDebug("About to send email to " + email.To[0].Address + ".");

      try
      {
        client.Send(email);
      }
      catch (Exception e)
      {
        AstraLogger.LogError(e);
      }

    }
    private MailAddress GetSendToAddress(string actualUserEmailAddr)
    {
      // check our mode...
      switch (this.EmailSettings.EmailSystemMode)
      {
        case EmailSystemMode.Active:
          return new MailAddress(actualUserEmailAddr);          

        case EmailSystemMode.Debug:
          return new MailAddress(this.EmailSettings.DebugToAddress);
          
        default:
          return null;
      }      
    }

    public IEnumerable<UserMailMessage> GetAllMessagesForUser(string user)
    {
      return _context.UserMessages.Where(x => x.RecipientMid == user).ToList();
    }

    public IEnumerable<UserMailMessage> GetUnreadMessagesForUser(string user)
    {
      return _context.UserMessages.Where(x => x.RecipientMid == user && x.ReadState == Models.UserMailMessageReadState.Unread).ToList();
    }

    public IEnumerable<UserMailMessage> GetReadMessagesForUser(string user)
    {
      return _context.UserMessages.Where(x => x.RecipientMid == user && x.ReadState == Models.UserMailMessageReadState.Read).ToList();
    }

    public void Delete(int id)
    {
      var message = _context.UserMessages.Find(id);
      _context.UserMessages.Remove(message);
      _context.SaveChanges();
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void MarkAllMessagesRead(string recipient)
    {
      var query = _context.UserMessages.Where(x => x.RecipientMid == recipient && x.ReadState == UserMailMessageReadState.Unread);

      foreach (var item in query)
      {
        item.ReadState = UserMailMessageReadState.Read;
      }

      _context.SaveChanges();
    }
    public string RenderPartialViewToString(string viewName, object model)
    {
      // assign the model of the controller from which this method was called to the instance of the passed controller (a new instance, by the way

      if (!string.IsNullOrEmpty(viewName))
      {
        viewName = new ControllerContext().RouteData.GetRequiredString("NewResourcesNotification");
        // initialize a string builder
        using (StringWriter sw = new StringWriter())
        {
          // find and load the view or partial view, pass it through the controller factory
          ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(new ControllerContext(), viewName);
          ViewContext viewContext = new ViewContext(new ControllerContext(), viewResult.View, new ControllerContext().Controller.ViewData, new ControllerContext().Controller.TempData, sw);

          // render it
          viewResult.View.Render(viewContext, sw);
          
       //return the razorized view/partial-view as a string
        return sw.ToString();
        }   
       
      } 
      return string.Empty;
    }
  }

  public interface IUserMailMessageRepository : IDisposable
  {
    void SendSystemMessage(string message, string recipient, string emailMessage);
    void SendSystemMessage(string message, List<string> recipients, string emailMessage);
    IEnumerable<UserMailMessage> GetAllMessagesForUser(string user);
    string RenderPartialViewToString(string viewName, object model);
    IEnumerable<UserMailMessage> GetUnreadMessagesForUser(string user);
    IEnumerable<UserMailMessage> GetReadMessagesForUser(string user);
    void Delete(int id);
    void MarkAllMessagesRead(string recipient);
    string GetUserEmail(string recipient);
    void SendEmail(string recipientMid, string emailMessageContent);
  }
}