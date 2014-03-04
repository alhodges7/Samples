using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Text;
using Astra.AstraConfigurations;
using Astra.AstraConfigurations.Settings;
using Astra.CompositeRepository;
using Astra.Models;
using Astra.Logging;
using Astra.Models.ViewModels;
using Astra.Controllers;
using Astra.Helper;
namespace Astra.NotificationServices
{
  public class NewResourceNotificationService
  {
    private NewResourceNotificationSection _configuration;
    public NewResourceNotificationService()
    {

      if (!CancellationTokenLibrary.NotificationsCancelationToken.IsCancellationRequested)
      {
        _configuration = AstraConfigurationManager.NewResourceSettings();
        StartService();
      }
    }
    private void StartService()
    {

      AstraLogger.LogDebug("Staring New Resource Notification Service...");

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
      DateTime.TryParse(_configuration.LastNewResourceCheck, out lastCheck);
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
        Thread.CurrentThread.Name = "NewResourceNotificationService";
      }
    }
    private void DoWork()
    {
      AstraLogger.LogDebug("Doing work New Resource Notification Service...");
      try
      {
        List<Resource> newResources;
        List<UserProfile> users;
        using (var repository = new ScopedCompositeRepository().Repositories)
        {
          newResources = repository.ResourceRepository.CheckForNewResources(_configuration.LastNewResourceCheck).ToList();
          users = repository.UserProfileRepository.All.ToList();
        }
        if (newResources.Any())
        {
          if (users.Any() && newResources.Any())
          {
           
            foreach (var user in users)
            {
             
              if (user.MID != "root")
              {
                SendNotification(user, newResources);
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        AstraLogger.LogError(e);
      }

      AstraLogger.LogDebug("Finishing work in New Resource Notification Service...");
    }
    private void SendNotification(UserProfile user, List<Resource> newResources)
    {
      string message = string.Empty;
      foreach (var book in newResources)
      {
        message += "<a href=\"Resources/Summary/?resourceId="+ book.ResourceID + "\">" + book.Title + " " + Environment.NewLine;
      }
      using (var repository = new ScopedCompositeRepository().Repositories.UserMessageRepository)
      {
        repository.SendSystemMessage("Dear " + user.FirstName + ", new resources have been added:" +
        Environment.NewLine +
        Environment.NewLine + "         " + message, user.MID, BuildEmailNotificationMessage(user.FirstName, newResources));
      }
    }

    private string BuildEmailNotificationMessage(string firstname, List<Resource> newResources)
    {
      StringBuilder sb = new StringBuilder();

      sb.Append("<!DOCTYPE html><html><head>");
      sb.Append("<meta name=\"" + "viewport " + "content=\"" + "width=device-width" + "/><title>New Resources Notification</title></head>");
      sb.Append("<body><div style=\"" + "text-align:center\"");
      sb.Append("<table style=\"" + "width: inherit; padding-top: 0in; padding-right: 5.4pt; padding-bottom: 0in; padding-left: 5.4pt; border-top-color: gray; border-right-color: gray; border-bottom-color: currentColor; border-left-color: gray; border-top-width: 1pt; border-right-width: 1pt; border-bottom-width: medium; border-left-width: 1pt; border-top-style: solid; border-right-style: solid; border-bottom-style: none; border-left-style: solid;\"" + ">");
      sb.Append("<tr><td><h2 style=\"" + "Color:#CC0074; text-align:left\"" + ">Dear, " + firstname + "</h2></td></tr>");
      sb.Append("<tr><td><h4>The following Resources have been added to the Astra Library:</h4></td></tr>");
      sb.Append("<tr><td><hr /></td></tr>");

      foreach (Resource r in newResources)
      {
        sb.Append("<tr><td>");
        if (r.CoverImage != null && r.CoverImage.ImageThumbnail != null)
        {
          sb.Append("<div style=\"" + "font-weight: bold\""+"><a href=\"Resources/Summary/?resourceId="+ r.ResourceID + "\">" + r.CoverImage.ImageThumbnail + " " + r.Title + "</a></div>");
        }
        else
        {
          sb.Append("<div style=\"" + "font-weight: bold\"" + "><a href=\"" + "Resources/Summary/?resourceId=" + r.ResourceID + "\">" + r.Title + "</a></div>");

        }
        sb.Append("</td></tr>");
      }
      sb.Append("<tr><td><hr /></td></tr></table></div></body></html>");
      return sb.ToString();
    }
  }

}