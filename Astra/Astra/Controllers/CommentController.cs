using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.Helper;
using Astra.Models;
using WebMatrix.WebData;
using Astra.Controllers.Shared;
using Astra.CompositeRepository;
using Astra.Misc;

namespace Astra.Controllers
{
  public class CommentController : BaseController
  {
    public ActionResult Index()
    {
      return View();
    }
    
    [ValidateInput(false)]    
    [Authorize]
    public void AddComment(int resourceId, string UserComment, string mid)
    {
      UserComment = OtherHelpers.SanitizeHtml(UserComment);
      if (OtherHelpers.IsEmptyHtmlOrString(UserComment))
      {
        return;
      }

      if (!MembershipHelper.MIDExists(mid))
      {
        return;
      }

      using (var repository = new ScopedCompositeRepository().Repositories.CommentRepository)
      {
          repository.AddComment(resourceId, mid, UserComment);        
      }
    }

    [ValidateInput(false)]
    [Authorize]
    public void EditComment(int resourceId, int commentId, string UserComment)
    {
      UserComment = OtherHelpers.SanitizeHtml(UserComment);
      if (OtherHelpers.IsEmptyHtmlOrString(UserComment))
        if (OtherHelpers.IsEmptyHtmlOrString(UserComment))
        {
          return;
        }
      
      using (var repository = new ScopedCompositeRepository().Repositories.CommentRepository)
      {
        repository.EditComment(resourceId, commentId, UserComment,
          MembershipHelper.CurrentUserName(),
          MembershipHelper.UserHasRoles(MembershipHelper.CurrentUserName(), new string[] { MembershipHelper.ROLE_ADMIN }));
      }
    }

    [Authorize]
    public void RemoveComment(int commentId)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.CommentRepository)
      {
        repository.RemoveComment(commentId);
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }

  }
}
