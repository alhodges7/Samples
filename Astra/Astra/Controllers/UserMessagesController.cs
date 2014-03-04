using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Html;
using WebMatrix.WebData;
using Astra.CompositeRepository;
using Astra.Models.ViewModels;
using Astra.Helper;
using Astra.Models;
using Astra.Controllers.Shared;

namespace Astra.Controllers
{
  public class UserMessagesController : BaseController
  {
    [Authorize]
    public ActionResult Index()
    {
      string username = MembershipHelper.CurrentUserName();
      UserMailMessages_IndexViewModel viewModel = null;
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        viewModel = new UserMailMessages_IndexViewModel()
        {
          ReaderUserProfile = repositories.UserProfileRepository.FindProfileByMID(username),
          ReadMessages = repositories.UserMessageRepository.GetReadMessagesForUser(username).OrderByDescending(x => x.SentOn),
          UnreadMessages = repositories.UserMessageRepository.GetUnreadMessagesForUser(username).OrderByDescending(x => x.SentOn),
        };

        repositories.UserMessageRepository.MarkAllMessagesRead(username);
      }

      return View(viewModel);
    }

    [Authorize]
    [ActionName("DeleteMessages")]
    [HttpPost]
    public ActionResult DeleteMessages(string messageIDs)
    {
      if (string.IsNullOrWhiteSpace(messageIDs))
      {
        return new JsonResult()
        {
          Data = new { result = false }
        };
      }
      string[] messageIDsArray = messageIDs.Split(',');

      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        for (int i = 0; i < messageIDsArray.Length; i++)
        {
          repositories.UserMessageRepository.Delete(int.Parse(messageIDsArray[i]));
        }

        return new JsonResult()
        {
          Data = new { result = true }
        };
      }
    }

    public ActionResult SendMessage()
    {
      using (var repo = new ScopedCompositeRepository().Repositories)
      {
        IEnumerable<UserProfile> profiles = repo.UserProfileRepository.AllIncluding();
        List<SelectListItem> selectList = new List<SelectListItem>();
        
        selectList.Add(new SelectListItem { Selected = true, Text = "All", Value = "All" });
        foreach (var profile in profiles)
        {
          SelectListItem sl = new SelectListItem
          {
            Selected = false,
            Text = profile.FirstName + " " + profile.LastName + " (" + profile.MID + ")",
            Value = profile.MID
          };
          selectList.Add(sl);
        }
        ViewBag.RecipientList = selectList;
      }
      return View(new UserMailMessage());
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN + "," + MembershipHelper.ROLE_LIBRARIAN)]
    [ValidateInput(false)]
    [HttpPost]
    public ActionResult SendMessages(List<string> recipientList, string originatorMid, string messageContent)
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        //remove malicious tags first
        messageContent = OtherHelpers.SanitizeHtml(messageContent); 
        if (OtherHelpers.IsEmptyHtmlOrString(messageContent))
        {
          TempData["messageStatus"] = "Body was left empty!";
          return RedirectToAction("SendMessage");
        }
        if (recipientList.Contains("All"))
        {
          //send to all minds
          recipientList = repositories.UserProfileRepository.AllUserMids();
          repositories.UserMessageRepository.SendSystemMessage(messageContent, recipientList, null);
        }
        else if (recipientList.Count > 1 && !recipientList.Contains("All"))
        {
          //send to selected minds
          repositories.UserMessageRepository.SendSystemMessage(messageContent, recipientList, null);
        }
        else
        {
          //send to single mind
          repositories.UserMessageRepository.SendSystemMessage(messageContent, recipientList[0], null);
        }
        TempData["messageStatus"] = "Message(s) successfully sent!";
        return RedirectToAction("SendMessage");
      }
    }


    public string NewResourcesNotification(NewResourcesNotification notification, string usermid)
    {
      using (var repository = new ScopedCompositeRepository().Repositories)
      {
        List<Resource> newResources = repository.ResourceRepository.CheckForNewResources(DateTime.Now.AddYears(-5).ToString());
        UserProfile user = repository.UserProfileRepository.FindProfileByMID("M1021693");

        notification = new NewResourcesNotification();
        notification.User = user;
        notification.NewResources = newResources;
        string html = repository.UserMessageRepository.RenderPartialViewToString("NewResourcesNotification", notification);
        return html;
      }
    }

  }
}
