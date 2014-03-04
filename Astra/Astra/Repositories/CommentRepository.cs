using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Helper;

namespace Astra.Repositories
{
  public class CommentRepository : ICommentRepository
  {
    private AstraContext _context = null;

    public CommentRepository(AstraContext context)
    {
      _context = context;
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public Comment GetUserCommentForResource(int resourceId, string userMid)
    {
      var comments = from comment in _context.Comments
                     where comment.ResourceID == resourceId && comment.UserMID.ToUpper() == userMid.ToUpper()
                     select comment;

      var query = comments.FirstOrDefault();
      if (query != null)
      {
        return new Comment()
        {
          CommentId = query.CommentId,
          CreatedOn = query.CreatedOn,
          Rating = query.Rating,
          ResourceID = query.ResourceID,
          UserComment = query.UserComment,
          UserMID = query.UserMID,
          Resource = query.Resource
        };

      }
      return null;

    }
    public List<Comment> GetAllCommentsByUser(string userMID)
    {
      var comments = _context.Comments.Where(x => x.UserMID.ToUpper() == userMID.ToUpper()).ToList();

      return comments;
    }

    public int GetNumberOfCommentsByUserForResource(int resourceId, string userMid)
    {
      return _context.Comments.Where(x => x.ResourceID == resourceId && x.UserMID.ToUpper() == userMid.ToUpper()).Count();
    }

    public bool UserHasCommentedOnResource(int resourceId, string userMid)
    {
      return 0 < _context.Comments.Where(x => x.ResourceID == resourceId && x.UserMID.ToUpper() == userMid.ToUpper()).Count();
    }

    public int GetTotalNumberOfComments(int resourceId)
    {
      return _context.Comments.Where(x => x.ResourceID == resourceId).Count();
    }

    public void AddComment(int resourceId, string userMid, string comment)
    {
      if (comment == string.Empty)
        return;

      Comment newComment = new Comment()
      {
        ResourceID = resourceId,
        UserComment = comment,
        UserMID = userMid
      };

      _context.Comments.Add(newComment);
      _context.Resources.Where(x => x.ResourceID == resourceId).First().Comments.Add(newComment);
      _context.SaveChanges();
    }


    public void EditComment(int resourceId, int commentId, string newComment, string userMid, bool userIsAdmin)
    {
      var search = _context.Comments.Where(x => x.CommentId == commentId);
      if (!search.Any())
      {
        throw new Exception(string.Format("Could not locate comment with id {0}.", commentId));
      }

      var comment = search.First();

      if (comment.UserMID != userMid && !userIsAdmin)
      {
        throw new Exception("This user is not authorized to edit this comment.");
      }

      comment.UserComment = newComment;
      _context.SaveChanges();
    }

    public void RemoveComment(int commentId)
    {
      var searchComment = _context.Comments.Where(x => x.CommentId == commentId);

      if (!searchComment.Any())
      {
        throw new Exception("Could not locate comment with id " + commentId);
      }

      Comment removeComment = searchComment.First();
      if (Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN) || MembershipHelper.CurrentUserName() == removeComment.UserMID)
      {
        _context.Comments.Remove(removeComment);
        _context.SaveChanges();
      }
    }

    public void DeleteCommentsForUser(string mid)
    {
      var comments = _context.Comments.Where(x => x.UserMID == mid);
      foreach (var item in comments)
      {
        _context.Comments.Remove(item);
      }
      _context.SaveChanges();
    }
    public void DeleteUserComment(int commentId)
    {
      if (commentId!=null)
      {
        var comment = _context.Comments.Where(x => x.CommentId == commentId).Single();
        _context.Comments.Remove(comment);
        _context.SaveChanges();
      }
     
    }
  }

  public interface ICommentRepository : IDisposable
  {
    Comment GetUserCommentForResource(int resourceId, string userMid);
    List<Comment> GetAllCommentsByUser(string userMID);
    int GetNumberOfCommentsByUserForResource(int resourceId, string userMid);
    bool UserHasCommentedOnResource(int resourceId, string userMid);
    int GetTotalNumberOfComments(int resourceId);
    void AddComment(int resourceId, string userMid, string comment);
    void EditComment(int resourceId, int commentId, string newComment, string userMid, bool userIsAdmin);
    void DeleteCommentsForUser(string mid);
    void DeleteUserComment(int commentId);
    void RemoveComment(int commentId);
  }
}