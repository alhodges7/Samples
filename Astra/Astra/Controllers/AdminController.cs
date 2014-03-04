using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Security;
using WebMatrix.WebData;
using MTUtil.Strings;
using MTUtil.TypeManagement;
using Astra.Controllers.Shared;
using Astra.Helper;
using Astra.Models;
using Astra.Models.ViewModels;
using Astra.CompositeRepository;
using Astra.Repositories;
using Astra.Logging;
using Astra.Models.ResourceTypes;
using Astra.Filters;

namespace Astra.Controllers
{

  public class AdminController : BaseController
  {
    public AdminController()
    {
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult Index()
    {
      return View();
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult Users()
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        return View(repositories.Repositories.UserProfileRepository);
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult Select(string mid)
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        var userdetails = repositories.Repositories.UserProfileRepository.AllIncluding(user => user.MID == mid);
        return View(userdetails);
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult ResourceList()
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        var resources = from r in repositories.Repositories.ResourceRepository.AllCommitted
                        orderby r.Title
                        select new ResourceListViewModel
                        {
                          ResourceId = r.ResourceID,
                          NumberOfCopies = r.Copies,
                          Title = r.Title,
                        };
        var resourceTypes = repositories.Repositories.ResourceTypeRepository.All;
        IEnumerable<ResourceType> enums = resourceTypes;
        List<SelectListItem> ddl = new List<SelectListItem>();
        ddl.Add(new SelectListItem { Selected = true, Text = "Select Type", Value = "Select Type" });
        foreach (var item in enums)
        {
          ddl.Add(new SelectListItem { Selected = false, Text = item.Name, Value = item.ResourceTypeID.ToString() });
        }
        ViewBag.ResourceTypes = ddl;
        return View(resources);
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult ResetUserPassword(int userProfileId)
    {
      UserProfile targetUser = null;
      string tempPassword = StringUtils.Randomize(6).ToUpper();

      using (var repositories = new ScopedCompositeRepository())
      {
        targetUser = repositories.Repositories.UserProfileRepository.Find(userProfileId);
        string token = WebSecurity.GeneratePasswordResetToken(targetUser.MID);
        WebSecurity.ResetPassword(token, tempPassword);
      }

      ViewBag.midLastReset = targetUser.MID;
      ViewBag.tempPassword = tempPassword;

      return View();
    }

    public JsonResult GetActiveCheckOutsForUser(string selectedValue)
    {
      var result = new JsonResult();
      using (var repositories = new ScopedCompositeRepository())
      {
        var checkouts = repositories.Repositories.CheckoutRepository.GetActiveCheckOutsForUser(selectedValue);
        var json = from a in checkouts
                   select new
                   {
                     Title = a.Resource.Title,
                     CheckOutId = a.CheckOutID
                   };
        result.Data = json.ToList();

        result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      }
      return result;
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public JsonResult SystemMessageCreateForUser()
    {
      string targetMID = TypeUtils.ToString(Request.QueryString["mid"]);

      AstraLogger.LogDebug(this.GetType(), "Attempting to send system message to " + targetMID);

      JsonResult result = new JsonResult();
      try
      {
        string messageText = TypeUtils.ToString(Request.Form);
        if (string.IsNullOrEmpty(messageText))
        {
          AstraLogger.LogError("Cannot send System Message.");
          result.Data = "FAILURE";
          return result;
        }

        messageText = HttpUtility.UrlDecode(messageText);

        using (var repositories = new ScopedCompositeRepository())
        {
          UserProfile targetUser = repositories.Repositories.UserProfileRepository.FindProfileByMID(targetMID);
          repositories.Repositories.UserMessageRepository.SendSystemMessage(messageText, targetUser.MID, null);
        }
      }
      catch (Exception e)
      {
        result.Data = "FAILURE";
        AstraLogger.LogError(e);
        return result;
      }

      result.Data = "SUCCESS";
      AstraLogger.LogDebug("Send system message succeeded!");
      return result;
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult Create(string resourceType)
    {
      var temp = new Resource();
      using (var repository = new ScopedCompositeRepository())
      {
        repository.Repositories.ResourceRepository.ClearUncommittedResources(ResourceRepository.DEFAULT_UNCOMMIT_DELETE_SPAN);
        ViewBag.Type = resourceType;
        switch (resourceType)
        {
          case "Book":
            {
              Book book = null;
              book = (Book)repository.Repositories.ResourceRepository.CreateStubResource(resourceType);
              return View(book);
            }
          case "DVD":
            {
              DVD dvd = null;
              dvd = (DVD)repository.Repositories.ResourceRepository.CreateStubResource(resourceType);
              return View(dvd);
            }
          case "EBook":
            {
              EBook ebook = null;
              ebook = (EBook)repository.Repositories.ResourceRepository.CreateStubResource(resourceType);
              return View(ebook);
            }
          case "Hardware":
            {
              Hardware hardware = null;
              hardware = (Hardware)repository.Repositories.ResourceRepository.CreateStubResource(resourceType);
              return View(hardware);
            }
          case "Software":
            {
              Software software = null;
              software = (Software)repository.Repositories.ResourceRepository.CreateStubResource(resourceType);
              return View(software);
            }
          case "White Paper":
            {
              WhitePaper whitePaper = null;
              whitePaper = (WhitePaper)repository.Repositories.ResourceRepository.CreateStubResource(resourceType);
              return View(whitePaper);
            }
          default:
            break;
        }
      }
      ViewBag.SelectedKeyWords = new List<KeyWord>();

      return View(temp);
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    [HttpPost]
    public ActionResult Create([ModelBinder(typeof(ResourceModelBinder))] Resource resource)
    {
      AstraLogger.LogDebug(this, "Inside AdminController.Create.");

      if (resource == null)
        AstraLogger.LogError(this, "Resource is NULL.");


      if (ModelState.IsValid)
      {
        using (var repositories = new ScopedCompositeRepository())
        {
          var existingResource = repositories.Repositories.ResourceRepository.Find<Resource>(resource.ResourceID);
          resource.Committed = true;
          resource.CoverImageId = existingResource.CoverImageId;
          repositories.Repositories.ResourceRepository.InsertOrUpdate(resource);
          repositories.Repositories.ResourceRepository.Save();
          repositories.Repositories.ResourceRepository.SyncKeyWords(resource);
          repositories.Repositories.ResourceRepository.Save();
          return RedirectToAction("ResourceList");
        }
      }
      else
      {
        return View(resource);
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    [Audit]
    public ActionResult EditResource(int resourceId = 0)
    {
      if (resourceId > 0)
      {
        using (var repositories = new ScopedCompositeRepository())
        {
          var resourceTypes = repositories.Repositories.ResourceTypeRepository.All;
          IEnumerable<ResourceType> enums = resourceTypes;
          ViewBag.ResourceTypes = enums;

          Resource resource = repositories.Repositories.ResourceRepository.AllCommittedIncluding(x => x.ResourceID == resourceId, "CoverImage,Images").FirstOrDefault();
          ViewBag.SelectedKeyWords = this.SelectedKeyWordsForResource(repositories, resource);
          return View(resource);
        }
      }
      return RedirectToAction("Summary", "Resources", resourceId);
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    [HttpPost]
    public ActionResult EditResource([ModelBinder(typeof(ResourceModelBinder))]Resource resource)
    {
      if (ModelState.IsValid)
      {
        using (var repositories = new ScopedCompositeRepository())
        {
          repositories.Repositories.ResourceRepository.InsertOrUpdate(resource);
          repositories.Repositories.ResourceRepository.Save();
          repositories.Repositories.ResourceRepository.SyncKeyWords(resource);
          repositories.Repositories.ResourceRepository.Save();
        }
        return RedirectToAction("Summary", "Resources", new { resourceId = resource.ResourceID });
      }
      else
      {
        return View(resource);
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN + "," + MembershipHelper.ROLE_LIBRARIAN)]
    public ActionResult DeleteResource(int resourceId)
    {
      ViewBag.Message = UserMessage.UserMessageType.GOOD;

      using (var repositories = new ScopedCompositeRepository())
      {
        return View(repositories.Repositories.ResourceRepository.Find<Resource>(resourceId));
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN + "," + MembershipHelper.ROLE_LIBRARIAN)]
    [HttpPost]
    public ActionResult DeleteConfirmed(int resourceId)
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        repositories.Repositories.ResourceRepository.Delete(resourceId);
        repositories.Repositories.ResourceRepository.Save();
      }

      return RedirectToAction("ResourceList", "Admin");
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    [HttpGet]
    public ActionResult AdminCheckoutDialog(int resourceId = 0)
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        List<CheckOut> chkOutList = repositories.Repositories.CheckoutRepository.GetActiveCheckOutsForResource(resourceId);
        List<UserProfile> users = repositories.Repositories.UserProfileRepository.All.ToList();

        // remove users that already checked out resource
        for (int user = users.Count - 1; user >= 0; user--)
        {
          foreach (CheckOut chkOut in chkOutList)
          {
            if (users[user].MID == chkOut.UserMID)
            {
              users.Remove(users[user]);
              break;
            }
          }
        }

        //create an anonymous type for easily representing name and mid
        var usersList = from ul in users
                        select new
                        {
                          Name = ul.FirstName + " " + ul.LastName,
                          MID = ul.MID
                        };

        SelectList usersSelectList = new SelectList(usersList, "MID", "Name", "---Select User---");
        ViewBag.Users = usersSelectList;
        ViewBag.ActionMessage = "Check Out";
        Resource resource = repositories.Repositories.ResourceRepository.Find<Resource>(resourceId);
        return PartialView("_AdminCheckInOrOut", resource);
      }
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    [HttpPost]
    public ActionResult AdminCheckoutDialog(int resId, string mid)
    {
      CheckOut checkOutObj = new CheckOut();
      checkOutObj.ResourceID = resId;
      checkOutObj.UserMID = mid;
      return RedirectToAction("CheckOut", "CheckOuts", checkOutObj);
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    [HttpGet]
    public ActionResult AdminCheckinDialog(int resourceId)
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        var qryUsers = from co in repositories.Repositories.CheckoutRepository.All
                       join a in repositories.Repositories.UserProfileRepository.All on co.UserMID equals a.MID
                       where co.ResourceID == resourceId && co.CheckOutStatus == CheckOutStatus.CheckedOut
                       select new
                       {
                         Name = a.FirstName + " " + a.LastName,
                         MID = co.UserMID
                       };

        SelectList usersSelectList = new SelectList(qryUsers.ToList(), "MID", "Name", "---Select User---");
        ViewBag.Users = usersSelectList;
        ViewBag.ActionMessage = "Check In";
        Resource resource = repositories.Repositories.ResourceRepository.Find<Resource>(resourceId);
        return PartialView("_AdminCheckInOrOut", resource);
      }
    }

    public ActionResult Resources()
    {
      return View();
    }

    public ActionResult ResourceTypes()
    {
      return RedirectToAction("Index", "ResourceTypes", null);
    }

    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public ActionResult Reports()
    {
      List<Report> reports = null;
      using (var scopedRepository = new ScopedCompositeRepository())
      {
        reports = scopedRepository.Repositories.ReportRepository.All();
      }

      return View(reports);
    }

    public ActionResult Settings()
    {
      return View();
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public ActionResult DeleteRating(int ratingId)
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        repositories.Repositories.RatingsRepository.DeleteRating(ratingId);
      }
      return null;
    }

    [HttpPost]
    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public ActionResult DeleteAllCommentsForUser(string usermid)
    {
      using (var comments = new ScopedCompositeRepository())
      {
        comments.Repositories.CommentRepository.DeleteCommentsForUser(usermid);
      }

      return new JsonResult
      {
        Data = "User reviews successfully deleted.",
      };
    }

    [HttpPost]
    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public ActionResult DeleteUserComment(int commentId)
    {
      using (var comments = new ScopedCompositeRepository().Repositories.CommentRepository)
      {
        comments.DeleteUserComment(commentId);
      }
      return null;
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public ActionResult DeleteAllRatingsForUser(string usermid)
    {
      string responseMessage = string.Empty;
      bool success;
      using (var ratings = new ScopedCompositeRepository())
      {
        success = ratings.Repositories.RatingsRepository.DeleteAllRatingsForUser(usermid);
        TempData["success"] = success;
        if (success)
        {
          responseMessage = "Ratings successfully cleared.";
        }
        else
        {
          responseMessage = "Ratings not successfully cleared.";
        }
      }
      return new JsonResult
      {
        Data = responseMessage,
      };
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public ActionResult ToggleLibrarianPermission(string usermid)
    {
      string responseMessage = string.Empty;
      using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        bool userIsLibrarian = Roles.IsUserInRole(usermid, MembershipHelper.ROLE_LIBRARIAN);
        repository.ToggleLibrarianPermission(usermid);
        if (userIsLibrarian)
        {
          responseMessage = "User is no longer a librarian.";
        }
        else
        {
          responseMessage = "User is now a librarian.";
        }
      }

      return new JsonResult
      {
        Data = responseMessage
      };
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public ActionResult PerUserAdminFunction(string mid)
    {
      PerUserAdminFunctionViewModel UserSummary;
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        var userInfo = repositories.UserProfileRepository.FindProfileByMID(mid);
        var userCheckouts = repositories.CheckoutRepository.GetCheckOutHistoryForUser(mid);
        var userRatings = repositories.RatingsRepository.GetAllRatingsByUser(mid);
        var comments = repositories.CommentRepository.GetAllCommentsByUser(mid);
        var userSuggestions = repositories.SuggestionRepository.GetAllSuggestionsForUser(mid);
        var userReservations = repositories.ReservationRepository.GetAllReservationsForUser(mid);

        UserSummary = new PerUserAdminFunctionViewModel(userInfo, userCheckouts, comments, userRatings, userSuggestions, userReservations);
      }

      return View(UserSummary);
    }

    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public JsonResult ToggleUserActiveState(string usermid)
    {
      bool userIsActive;
      string responseMessage = string.Empty;
      using (var repositories = new ScopedCompositeRepository())
      {
        repositories.Repositories.UserProfileRepository.ToggleUserActiveState(usermid);
        userIsActive = repositories.Repositories.UserProfileRepository.FindProfileByMID(usermid).IsActive;
      }
      if (userIsActive)
      {
        responseMessage = "User successfully activated.";
      }
      else
      {
        responseMessage = "User successfully deactivated.";
      }
      var result = new JsonResult();
      result.Data = responseMessage;
      result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return result;

    }

    // Remember- when you die in Astra, you die in real life.
    [Authorize(Roles = MembershipHelper.ROLE_ADMIN)]
    public JsonResult DeleteUser(string usermid)
    {
      string responseMessage = string.Empty;
      if (usermid != null)
      {
        using (var repositories = new ScopedCompositeRepository())
        {
          UserProfile up = repositories.Repositories.UserProfileRepository.FindProfileByMID(usermid);

          if (up != null)
          {
            repositories.Repositories.UserProfileRepository.DeleteUser(usermid);
            responseMessage = "User successfully deleted.";
          }
          else
          {
            responseMessage = "Could not delete: user not found.";
          }
        }
      }
      var result = new JsonResult();
      result.Data = responseMessage;
      return result;
    }

    private List<KeyWord> SelectedKeyWordsForResource(ScopedCompositeRepository repository, Resource r)
    {
      Astra.Controllers.ResourcesController resourcesController = new Astra.Controllers.ResourcesController();
      List<Astra.Models.KeyWord> selectedWords = new List<Astra.Models.KeyWord>();

      if (r.ResourceID != 0)
      {
        Resource resourcesFull = resourcesController.GetFullLoadedResource(r.ResourceID);

        foreach (Astra.Models.ResourceToKeyWordLink link in resourcesFull.KeyWordLinks)
        {
          Astra.Models.ResourceToKeyWordLink fullLoadedLink = repository.Repositories.KeyWordLinksRepository.GetFullyLoadedResourceLink(link.LinkID);
          selectedWords.Add(fullLoadedLink.KeyWord);
        }
      }
      return selectedWords;
    }

  }
}
