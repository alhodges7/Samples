using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Security;
using Astra.Controllers.Shared;
using Astra.Misc;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Models.ViewModels;
using Astra.Repositories;
using MTUtil.Strings;
using MTUtil.TypeManagement;
using PagedList;
using WebMatrix.WebData;
using Astra.CompositeRepository;
using Astra.Helper;
using Astra.SharePointIntegration;

namespace Astra.Controllers
{
  public class SuggestionsController : BaseController
  {
    public const int ANY_SELECTION_VALUE = -1;  //suggestions with any status
    public const int PAGE_SIZE = 20;
    public enum EDIT_TYPES { EDIT, DELETE }
    public enum DISPLAY_TYPES { TABLES, GRID }

    //display index for the suggestion controller
    public ActionResult Index(int page = 1, string sort = "CreatedOn", string sortDir = "ASC", int suggestionStatus = ANY_SELECTION_VALUE, DISPLAY_TYPES displayType = DISPLAY_TYPES.GRID)
    {
      //store ViewBag data to be used in cshtml view
      ViewBag.StatusSeletionCode = suggestionStatus;
      ViewBag.CurrentDisplayType = displayType;

      //check which type of user this is
      if (Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN) || Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_LIBRARIAN))
      {
        //decide which description this page should have
        switch (suggestionStatus)
        {
          case ANY_SELECTION_VALUE:
            ViewBag.Description = "All Suggestions";
            break;

          case (int)ResourceSuggestion.SuggestionStates.Pending:
            ViewBag.Description = "Pending Suggestions";
            break;

          case (int)ResourceSuggestion.SuggestionStates.Approved:
            ViewBag.Description = "Approved Suggestions";
            break;

          case (int)ResourceSuggestion.SuggestionStates.Rejected:
            ViewBag.Description = "Rejected Suggestions";
            break;

          default:
            throw new Exception("Unknown Status integer value");
        }

        using (var composite = new ScopedCompositeRepository())
        {
          ResourceSuggestionPager output = composite.Repositories.SuggestionRepository.GetPagedSuggestionList(string.Empty, page, PAGE_SIZE, sort, sortDir, suggestionStatus, displayType);
          return View(output);
        }
      }
      else
      { //user is not an admin or librarian & only sees his/her suggestions
        using (var composite = new ScopedCompositeRepository())
        {
          ViewBag.Description = "My Suggestions";
          string currentUserName = MembershipHelper.CurrentUserName();
          ResourceSuggestionPager output = composite.Repositories.SuggestionRepository.GetPagedSuggestionList(currentUserName, page, PAGE_SIZE, sort, sortDir, suggestionStatus, DISPLAY_TYPES.GRID);
          return View(output);
        }
      }
    }

    /// <summary>
    /// This Post Back Index function is mostly just a wrapper for the Edit and Delete,
    /// functions but it is required because if you want to have form(s) on your, 
    /// index page then the only way to catch the post back of that page is with  
    /// a post back version of of the index ActionResult function
    /// </summary>
    /// <param name="postbackSuggestion">ResourceSuggestion object posted back from the form</param>
    /// <returns>index</returns>
    [HttpPost]
    public ActionResult Index(ResourceSuggestion postbackSuggestion)
    {
      if (Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN) || Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_LIBRARIAN))
      {
        //test to determine if this is an edit or delete request
        if (!string.IsNullOrEmpty(Request.Form["EditType"]))
        {
          string editType = Request.Form["EditType"];
          if (editType == EDIT_TYPES.DELETE.ToString())
          {
            //notify user the suggestion has been deleted
            using (var repositories = new ScopedCompositeRepository())
            {
              string mid = Request.Form["CreatedByMID"];
              string message = "Your suggestion about " + postbackSuggestion.Title + " has been deleted.  "
                             + "\nLast status: " + postbackSuggestion.GetStatusString() + ".  ";

              //check if the librarian / admin wants to send the user feedback
              if (!string.IsNullOrEmpty(Request.Form["Feedback"]))
              {
                message += "\nFeedback: " + Request.Form["Feedback"];
              }
              repositories.Repositories.UserMessageRepository.SendSystemMessage(message, mid, null);
            }

            int currentStatusSeletionCode = int.Parse(Request.Form["StatusSeletionCode"]);
            return DeleteConfirmed(postbackSuggestion.ResourceSuggestionId, currentStatusSeletionCode);
          }
          else if (editType == EDIT_TYPES.EDIT.ToString())
          {
            //notify user the suggestion has been edited
            using (var repositories = new ScopedCompositeRepository())
            {
              string mid = postbackSuggestion.CreatedByMID;
              string message = "Your suggestion about " + postbackSuggestion.Title + " has been edited."
                             + "  \nCurrent Status: " + postbackSuggestion.GetStatusString() + ".  ";

              //check if the librarian / admin wants to send the user feedback
              if (!string.IsNullOrEmpty(Request.Form["Feedback"]))
              {
                message += "\nFeedback: " + Request.Form["Feedback"];
              }
              repositories.Repositories.UserMessageRepository.SendSystemMessage(message, mid, null);
            }

            int currentStatusSeletionCode = int.Parse(Request.Form["StatusSeletionCode"]);
            return Edit(postbackSuggestion, currentStatusSeletionCode);
          }
          else
          {
            throw new Exception("Unknown Edit Type");
          }
        }
        else
          return Index(); //go back to index as a way to ignore null input
      }
      else
      {
        //ordinary users can only delete their own suggestions
        //this should never happen unless someone tampers with the form
        //but putting this test here makes the code more bullet proof
        if (Request.Form["EditType"] != EDIT_TYPES.DELETE.ToString() || MembershipHelper.CurrentUserName() != Request.Form["CreatedByMID"])
        {
          throw new Exception("Ordinary users can only delete their own suggestions");
        }
        else
        {
          return DeleteConfirmed(postbackSuggestion.ResourceSuggestionId);
        }
      }
    }

    public ActionResult Details(int id = 1)
    {
      ResourceSuggestion suggestion = null;
      using (var composite = new ScopedCompositeRepository())
      {
        suggestion = composite.Repositories.SuggestionRepository.Find(id);
      }

      return View(suggestion);
    }


    public ActionResult GridEdit(int id = 1, int suggestionStatus = ANY_SELECTION_VALUE, DISPLAY_TYPES displayType = DISPLAY_TYPES.TABLES)
    {
      //store ViewBag data to be used in cshtml view
      ViewBag.StatusSeletionCode = suggestionStatus;
      ViewBag.CurrentDisplayType = displayType;

      ResourceSuggestion suggestion = null;
      using (var composite = new ScopedCompositeRepository())
      {
        suggestion = composite.Repositories.SuggestionRepository.Find(id);
      }

      return View(suggestion);
    }


    /// <summary>
    /// This Post Back GridEdit function is mostly just a wrapper for the Edit and Delete,
    /// functions but it is required because if you want to have form(s) on the 
    /// same view page then the only way to catch the post back of that page is with  
    /// a post back version of of the same  ActionResult function
    /// </summary>
    /// <param name="postbackSuggestion">ResourceSuggestion object posted back from the form</param>
    /// <returns>ActionResult for GridEdit view</returns>
    [HttpPost]
    public ActionResult GridEdit(ResourceSuggestion postbackSuggestion)
    {
      if (Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN) || Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_LIBRARIAN))
      {
        //test to determine if this is an edit or delete request
        if (!string.IsNullOrEmpty(Request.Form["EditType"]))
        {
          string editType = Request.Form["EditType"];
          if (editType == EDIT_TYPES.DELETE.ToString())
          {
            //notify user the suggestion has been deleted
            using (var repositories = new ScopedCompositeRepository())
            {
              string mid = Request.Form["CreatedByMID"];
              string message = "Your suggestion about " + postbackSuggestion.Title + " has been deleted.  "
                             + "\nLast status: " + postbackSuggestion.GetStatusString() + ".  ";

              //check if the librarian / admin wants to send the user feedback
              if (!string.IsNullOrEmpty(Request.Form["Feedback"]))
              {
                message += "\nFeedback: " + Request.Form["Feedback"];
              }
              repositories.Repositories.UserMessageRepository.SendSystemMessage(message, mid, null);
            }

            int currentStatusSeletionCode = int.Parse(Request.Form["StatusSeletionCode"]);
            return DeleteConfirmed(postbackSuggestion.ResourceSuggestionId, currentStatusSeletionCode, DISPLAY_TYPES.GRID);
          }
          else if (editType == EDIT_TYPES.EDIT.ToString())
          {
            //notify user the suggestion has been edited
            using (var repositories = new ScopedCompositeRepository())
            {
              string mid = postbackSuggestion.CreatedByMID;
              string message = "Your suggestion about " + postbackSuggestion.Title + " has been edited."
                             + "  \nCurrent Status: " + postbackSuggestion.GetStatusString() + ".  ";

              //check if the librarian / admin wants to send the user feedback
              if (!string.IsNullOrEmpty(Request.Form["Feedback"]))
              {
                message += "\nFeedback: " + Request.Form["Feedback"];
              }
              repositories.Repositories.UserMessageRepository.SendSystemMessage(message, mid, null);
            }

            int currentStatusSeletionCode = int.Parse(Request.Form["StatusSeletionCode"]);
            return Edit(postbackSuggestion, currentStatusSeletionCode, DISPLAY_TYPES.GRID);
          }
          else
          {
            throw new Exception("Unknown Edit Type");
          }
        }
        else
          return Index(); //go back to index as a way to ignore null input
      }
      else
      {
        //ordinary users can only delete their own suggestions
        //this should never happen unless someone tampers with the form
        //but putting this test here makes the code more bullet proof
        if (Request.Form["EditType"] != EDIT_TYPES.DELETE.ToString() || MembershipHelper.CurrentUserName() != Request.Form["CreatedByMID"])
        {
          throw new Exception("Ordinary users can only delete their own suggestions");
        }
        else
        {
          return DeleteConfirmed(postbackSuggestion.ResourceSuggestionId);
        }
      }
    }


    [Authorize]
    public ActionResult Create(int suggestionStatus = ANY_SELECTION_VALUE, DISPLAY_TYPES displayType = DISPLAY_TYPES.TABLES)
    {
      ViewBag.StatusSeletionCode = suggestionStatus;
      ViewBag.CurrentDisplayType = displayType;
      return View();
    }

    [HttpPost]
    public ActionResult Create(ResourceSuggestion suggestion)
    {
      suggestion.Description = OtherHelpers.SanitizeHtml(suggestion.Description);
      suggestion.ReasonNeeded = OtherHelpers.SanitizeHtml(suggestion.ReasonNeeded);
      suggestion.LibrariansNote = OtherHelpers.SanitizeHtml(suggestion.LibrariansNote);
      if (OtherHelpers.IsEmptyHtmlOrString(suggestion.Description) || OtherHelpers.IsEmptyHtmlOrString(suggestion.ReasonNeeded))
      {
        suggestion.ReasonNeeded = "<strong>Reason Required</strong>";
        suggestion.Description = "Description Required";
      }

      if (ModelState.IsValid)
      {
        using (var composite = new ScopedCompositeRepository())
        {
          composite.Repositories.SuggestionRepository.InsertOrUpdate(suggestion);

          string message = "A new suggestion about " + suggestion.Title + " was created by " + UsersController.GetUserNameByMID(suggestion.CreatedByMID);
          List<string> adminLibrarianList = composite.Repositories.UserProfileRepository.FindlLibrarianAdminMIDList();
          composite.Repositories.UserMessageRepository.SendSystemMessage(message, adminLibrarianList, null);
        }
        return RedirectToAction("Index", new { suggestionStatus = int.Parse(TempData["StatusSeletionCode"].ToString()), displayType = (DISPLAY_TYPES)TempData["CurrentDisplayType"] });
      }
      else
      {
        using (var composite = new ScopedCompositeRepository())
        {
          string message = "Suggestion failed ModelState.IsValid: " + suggestion.ToString();
          composite.Repositories.UserMessageRepository.SendSystemMessage(message, "M1021311", null);
        }
      }

      return RedirectToAction("Index", new { suggestionStatus = (int)ResourceSuggestion.SuggestionStates.Pending });
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN + "," + MembershipHelper.ROLE_LIBRARIAN)]
    public ActionResult Edit(int id = 1)
    {
      string currentUserName = MembershipHelper.CurrentUserName();

      using (var composite = new ScopedCompositeRepository())
      {
        var suggestion = composite.Repositories.SuggestionRepository.Find(id);
        if (suggestion == null)
        {
          return HttpNotFound();
        }

        //The view page prevents the edit link being shown to anyone who  
        //is not an Admin or Librarian, so if this happens
        //there could be a cross script attack
        if (suggestion.CreatedByMID != MembershipHelper.CurrentUserName() && !Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN) && !Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_LIBRARIAN))
        {
          throw new Exception("Unathorized attempt to Edit Suggestion.");
        }

        return View(suggestion);
      }
    }

    [HttpPost]
    public ActionResult Edit(ResourceSuggestion suggestion, int inputSuggestionStatus = (int)ResourceSuggestion.SuggestionStates.Pending, DISPLAY_TYPES currentDisplayType = DISPLAY_TYPES.TABLES)
    {
      suggestion.Description = OtherHelpers.SanitizeHtml(suggestion.Description);
      suggestion.ReasonNeeded = OtherHelpers.SanitizeHtml(suggestion.ReasonNeeded);
      suggestion.LibrariansNote = OtherHelpers.SanitizeHtml(suggestion.LibrariansNote);
      if (OtherHelpers.IsEmptyHtmlOrString(suggestion.Description) || OtherHelpers.IsEmptyHtmlOrString(suggestion.ReasonNeeded))
      {
        suggestion.ReasonNeeded = "<strong>Reason Required</strong>";
        suggestion.Description = "Description Required";
      }

      if (ModelState.IsValid)
      {

        // We need to get the status of the Suggestion before we save it.  We need this 
        //  in order to fire events when the Suggestion is first approved
        int prevStatus = ResourceSuggestion.SuggestionStates.Pending.GetHashCode();
        using (var composite = new ScopedCompositeRepository())
        {
          prevStatus = composite.Repositories.SuggestionRepository.Find(suggestion.ResourceSuggestionId).Status;
        }

        // save the changes from the post
        using (var composite = new ScopedCompositeRepository())
        {
          composite.Repositories.SuggestionRepository.InsertOrUpdate(suggestion);
          composite.Repositories.SuggestionRepository.Save();

          if (suggestion.Status == ResourceSuggestion.SuggestionStates.Approved.GetHashCode() && prevStatus != ResourceSuggestion.SuggestionStates.Approved.GetHashCode())
            TransmitResourceSuggestionToSharePoint(suggestion);
        }
        return RedirectToAction("Index", new { suggestionStatus = inputSuggestionStatus, displayType = currentDisplayType });
      }
      return RedirectToAction("Index", new { suggestionStatus = inputSuggestionStatus });
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN + "," + MembershipHelper.ROLE_LIBRARIAN)]
    public ActionResult SetStatus(int suggestionId, int newStatus)
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        repositories.SuggestionRepository.SetSuggestionStatus(suggestionId, newStatus);
      }

      //notify user the suggestion has been accepted / rejected
      using (var repositories = new ScopedCompositeRepository())
      {
        ResourceSuggestion editedSuggestion = repositories.Repositories.SuggestionRepository.Find(suggestionId);
        string mid = editedSuggestion.CreatedByMID;
        string message = "Your suggestion about " + editedSuggestion.Title + " has been " + editedSuggestion.GetStatusString() + ".  ";
        repositories.Repositories.UserMessageRepository.SendSystemMessage(message, mid, null);
      }

      return Json(new { result = true });
    }

    public ActionResult Delete(int id = 1)
    {
      using (var composite = new ScopedCompositeRepository())
      {
        var suggestion = composite.Repositories.SuggestionRepository.Find(id);
        if (suggestion == null)
        {
          return HttpNotFound();
        }

        //The view page prevents the delete link being shown to anyone who did not 
        //create the comment or is not an Admin or Librarian, so if this happens
        //there could be a cross script attack
        if (suggestion.CreatedByMID != MembershipHelper.CurrentUserName() && !Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN) && Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_LIBRARIAN))
        {
          throw new Exception("Unathorized attempt to Delete Suggestion.");
        }

        return View(suggestion);
      }
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id, int inputSuggestionStatus = (int)ResourceSuggestion.SuggestionStates.Pending, DISPLAY_TYPES currentDisplayType = DISPLAY_TYPES.TABLES)
    {
      using (var composite = new ScopedCompositeRepository())
      {
        composite.Repositories.SuggestionRepository.Delete(id);
      }
      return RedirectToAction("Index", new { suggestionStatus = inputSuggestionStatus, displayType = currentDisplayType });
    }

    public bool TransmitResourceSuggestionToSharePoint(ResourceSuggestion suggestion)
    {
      SharePointIntegrator spi = new SharePointIntegrator();
      if (!spi.ConfigSettings.SuggestedResourcesSettings.PassThroughSuggestions)
        return true;    // nothing to do!!

      bool success = false;
      success = spi.TransmitSuggestedResourceToSharePoint(suggestion);
      return success;
    }

    protected override void Dispose(bool disposing)
    {
      using (var composite = new ScopedCompositeRepository())
      {
        composite.Repositories.SuggestionRepository.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}