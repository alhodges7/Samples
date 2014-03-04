using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Text;
using Astra.CompositeRepository;
using Astra.Models;
using Astra.AstraConfigurations;
using Astra.AstraConfigurations.Settings;

namespace Astra.NotificationServices
{
  public class OverdueCheckService
  {
    private OverdueBookNotificationSection _configuration;
    public OverdueCheckService()
    {
      if (!CancellationTokenLibrary.NotificationsCancelationToken.IsCancellationRequested)
      {
        _configuration = AstraConfigurationManager.OverdueBookSettings();
        StartService();
      }
    }
    private void StartService()
    {
      OverdueBookNotificationSection _configuration = AstraConfigurationManager.OverdueBookSettings();
      SetThreadName();
      do
      {
        if (IsCheckRequired())
        {
          DoWork();
          Thread.Sleep(TimeSpan.FromHours(_configuration.ServiceFrequency));
        }
        Thread.Sleep(TimeSpan.FromHours(1.0));
      } while (!CancellationTokenLibrary.NotificationsCancelationToken.IsCancellationRequested);
    }
    private bool IsCheckRequired()
    {
      DateTime lastCheck = DateTime.MinValue;
      DateTime.TryParse(_configuration.LastOverdueCheck, out lastCheck);
      if (lastCheck != DateTime.MinValue)
      {
        if (DateTime.UtcNow.Subtract(lastCheck).CompareTo(TimeSpan.FromHours(_configuration.ServiceFrequency)) == -1)
        {
          return false;
        }
      }
      return true;
    }
    private static void SetThreadName()
    {
      if (Thread.CurrentThread.Name == null)
      {
        Thread.CurrentThread.Name = "OverdueService";
      }
    }
    private void DoWork()
    {
      List<CheckOut> overdueBooks;

      using (var overdueCheckouts = new ScopedCompositeRepository().Repositories.CheckoutRepository)
      {
        overdueBooks = overdueCheckouts.OverdueBooks();
      }

      if (overdueBooks.Any())
      {
        List<CheckOut> temp = new List<CheckOut>();
        temp.Add(overdueBooks[0]);
        overdueBooks.RemoveAt(0);
        foreach (var person in overdueBooks)
        {
          if (temp[0].UserMID.ToLower() == person.UserMID.ToLower())
          {
            temp.Add(person);
          }
          else
          {
            SendNotification(GetUserName(temp[0].UserMID), temp);
            temp.Clear();
            temp.Add(person);
          }
        }
      }

    }
    private UserProfile GetUserName(string mid)
    {
      UserProfile username;
      using (var user = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        username = user.FindProfileByMID(mid);
      }
      return username;
    }
    private void SendNotification(UserProfile User, List<CheckOut> Books)
    {
      string message = string.Empty;
      foreach (var book in Books)
      {
        message += "<a href=\"Resources/Summary/?resourceId=" + book.Resource.ResourceID + "\">" + book.Resource.Title + " " + Environment.NewLine;
      }
      using (var repository = new ScopedCompositeRepository().Repositories.UserMessageRepository)
      {
        repository.SendSystemMessage("Dear " + User.FirstName + " you have overdue books :" +
        Environment.NewLine +
        Environment.NewLine + "         " + message +
        "Please Return them Immediately", User.MID, BuildEmailNotificationMessage(User.FirstName, Books));
      }
    }

    private string BuildEmailNotificationMessage(string firstname, List<CheckOut> overdueBooks)
    {
      StringBuilder sb = new StringBuilder();

      sb.Append("<!DOCTYPE html><html><head>");
      sb.Append("<meta name=\"" + "viewport " + "content=\"" + "width=device-width" + "/><title>New Resources Notification</title></head>");
      sb.Append("<body><div style=\"" + "text-align:center\"");
      sb.Append("<table style=\"" + "width: inherit; padding-top: 0in; padding-right: 5.4pt; padding-bottom: 0in; padding-left: 5.4pt; border-top-color: gray; border-right-color: gray; border-bottom-color: currentColor; border-left-color: gray; border-top-width: 1pt; border-right-width: 1pt; border-bottom-width: medium; border-left-width: 1pt; border-top-style: solid; border-right-style: solid; border-bottom-style: none; border-left-style: solid;\"" + ">");
      sb.Append("<tr><td colspan=\"2\"></tr></td>");
      sb.Append("<tr><td colspan=\"2\" style=\"text-align:left; vertical-align:middle;\"><h2 style=\"" + "Color:#CC0074;\"" + ">Dear, " + firstname + "</h2></td></tr>");
      sb.Append("<tr><td colspan=\"2\"> <h4>The following books are overdue:</h4></td></tr>");
      sb.Append("<tr><td colspan=\"2\"><hr /></td></tr>");
      foreach (CheckOut r in overdueBooks)
      {
        sb.Append("<tr><td colspan=\"2\">");
        if (r.Resource.CoverImage != null && r.Resource.CoverImage.ImageThumbnail != null)
        {
          sb.Append("<div style=\"" + "font-weight: bold; text-align:center;\" " + "><a href=\"" + "Resources/Summary/?resourceId=" + r.ResourceID + "\">" + r.Resource.CoverImage.ImageThumbnail + " " + r.Resource.Title + "</a></div>");
        }
        else
        {
          sb.Append("<div style=\"" + "font-weight: bold; text-align:center;\"" + "><a href=\"" + "Resources/Summary/?resourceId=" + r.ResourceID + "\">" + r.Resource.Title + "</a></div>");

        }
        sb.Append("</td></tr>");
      }
      sb.Append("<tr><td colspan=\"2\">Please return them immediately! Thanks.</td></tr>");
      sb.Append("<tr><td colspan=\"2\"><hr /></td></tr></table></body></html></div>");
        
      return sb.ToString();
    }
  }


}