using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure;
using Astra.DatabaseContext;
using Astra.Models;


namespace Astra.Repositories
{
  public class RatingsRepository : IRatingsRepository
  {
    private AstraContext _context = null;

    public RatingsRepository(AstraContext context)
    {
      _context = context;
    }

    public void RateResource(string userMid, int resourceId, double userRating)
    {
      var existingRating = _context.Ratings.Where(x => x.ResourceID == resourceId && x.UserMID == userMid);

      if (!existingRating.Any())
      {
        _context.Ratings.Add(new Rating { UserMID = userMid, UserRating = userRating, ResourceID = resourceId });
      }
      else
      {
        if (userRating == -1)
        {
          _context.Ratings.Remove(existingRating.First());
        }
        else
        {
          existingRating.First().UserRating = userRating;
        }
      }

      _context.SaveChanges();
    }

    public double GetAverageRatingForResource(int resourceId)
    {
      double total = 0.0;

      if (resourceId != 0)
      {

        var ratings = from resources in _context.Ratings
                      where resources.ResourceID == resourceId
                      select new { rating = resources.UserRating };

        foreach (var item in ratings)
        {
          total += item.rating;
        }
        total = total / ratings.Count();
        if (!ratings.Any())
        {
          return 0.0;
        }
      }

      return total;
    }

    public double GetUserRating(int resourceID, string userMID)
    {

      var rating = _context.Ratings.Where(x => x.ResourceID == resourceID && x.UserMID == userMID);

      if (rating.Any())
      {
        return (double)rating.First().UserRating;
      }
      return 0;
    }

    public List<Rating> GetAllRatingsForResource(int resourceId)
    {
      var ratings = _context.Ratings.Where(x => x.ResourceID == resourceId).ToList();
      return ratings;
    }

    public List<Rating> GetAllRatingsByUser(string userMID)
    {
      var userRatings = _context.Ratings.Where(x => x.UserMID == userMID).ToList();
      return userRatings;
    }
    public void Dispose()
    {
      // Do nothing
    }
    public bool DeleteAllRatingsForUser(string mid)
    {
      var ratings = _context.Ratings.Where(x=>x.UserMID==mid);
      int recordsToDelete = ratings.Count();
      foreach (var item in ratings)
      {
        _context.Ratings.Remove(item);
      }
      if (_context.SaveChanges() == recordsToDelete)
      return true;
      else
      return false;
    }

    public void DeleteRating(int ratingId)
    {
      var searchRating = _context.Ratings.Where(x => x.RatingId == ratingId);
      if (!searchRating.Any())
      {
        throw new Exception("Could not locate a rating with the provided id.");
  }

      Rating removeRating = searchRating.First();
      _context.Ratings.Remove(removeRating);
      _context.SaveChanges();
    }
  }
  public interface IRatingsRepository : IDisposable
  {
    void RateResource(string userMid, int resourceId, double rating);
    double GetAverageRatingForResource(int resourceId);
    double GetUserRating(int resourceId, string userMID);
    List<Rating> GetAllRatingsForResource(int resourceId);
    List<Rating> GetAllRatingsByUser(string userMID);
    bool DeleteAllRatingsForUser(string mid);
    void DeleteRating(int ratingId);
  }
}