using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Astra.CompositeRepository;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Models.ViewModels;
using Astra.Controllers;

namespace Astra.Repositories
{
  public class SuggestionRepository : ISuggestionRepository
  {
    private AstraContext _context;

    #region Web Grid Helpers
    /// <summary>
    /// Private helper function used to Create the Ordering Function for the Dictionary of linq sorting functions.
    /// Returns a Func that takes an IQueryable and a bool, and sorts the IQueryable (ascending or descending 
    /// based on the bool). The sort is performed on the property identified by the key selector.
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    /// <typeparam name="TKey">key type</typeparam>
    /// <param name="keySelector">lambda expression</param>
    /// <returns>IDictionary of Tkey and Func delegate references</returns>
    private static Func<IQueryable<T>, bool, IOrderedQueryable<T>> CreateOrderingFunc<T, TKey>(Expression<Func<T, TKey>> keySelector)
    {
      return
          (source, ascending) =>
          ascending ? source.OrderBy(keySelector)
                    : source.OrderByDescending(keySelector);
    }

    //Private Dictionary used to hold the linq sorting functions
    private readonly IDictionary<string, Func<IQueryable<ResourceSuggestion>, bool, IOrderedQueryable<ResourceSuggestion>>>
        _suggestionOrderingsDictionary = new Dictionary<string, Func<IQueryable<ResourceSuggestion>, bool, IOrderedQueryable<ResourceSuggestion>>>
                                    {
                                        {"CreatedByMID", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.CreatedByMID)},
                                        {"CreatedOn", CreateOrderingFunc<ResourceSuggestion, DateTime>(x=>x.CreatedOn)},
                                        {"Description", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.Description)},
                                        {"ISBN", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.ISBN)},
                                        {"ISBN10", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.ISBN10)},
                                        {"ISBN13", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.ISBN13)},
                                        {"LibrariansNote", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.LibrariansNote)},
                                        {"Price", CreateOrderingFunc<ResourceSuggestion, decimal>(x=>x.Price)},
                                        {"ReasonNeeded", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.ReasonNeeded)},
                                        {"ResourceSuggestionId", CreateOrderingFunc<ResourceSuggestion, int>(x=>x.ResourceSuggestionId)},
                                        {"Status", CreateOrderingFunc<ResourceSuggestion, int>(x=>x.Status)},
                                        {"URL", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.URL)},
                                        {"Title", CreateOrderingFunc<ResourceSuggestion, string>(x=>x.Title)}
                                    };
    #endregion

    #region Constructor
    public SuggestionRepository(AstraContext context)
    {
      _context = context;
    }
    #endregion

    public ResourceSuggestionPager GetPagedSuggestionList(string userMID = "", int page = 1, int pageSize = 500, string sortField = "CreatedOn", string sortOrder = "ASC", int suggestionStatus = SuggestionsController.ANY_SELECTION_VALUE, SuggestionsController.DISPLAY_TYPES currentDisplayType = SuggestionsController.DISPLAY_TYPES.TABLES)
    {

        //start query with the ResourceSuggestion context
        IQueryable<ResourceSuggestion> suggestionQuery = _context.ResourceSuggestions;

        //limit query to only the requested user
        if (userMID != string.Empty)
        {
          suggestionQuery = suggestionQuery.Where(x => x.CreatedByMID.ToUpper() == userMID.ToUpper());
        }

        //limit query to those with the correct status type
        if (suggestionStatus != SuggestionsController.ANY_SELECTION_VALUE)
        {
          suggestionQuery = suggestionQuery.Where(x => (x.Status == suggestionStatus));
        }

        // apply sorting
        Func<IQueryable<ResourceSuggestion>, bool, IOrderedQueryable<ResourceSuggestion>> applyOrdering = _suggestionOrderingsDictionary[sortField];
        suggestionQuery = applyOrdering(suggestionQuery, (sortOrder == "Ascending") || (sortOrder == "ASC"));

        List<ResourceSuggestion> currentSuggestionList = null; //variable for current list of suggestions

        if (currentDisplayType == SuggestionsController.DISPLAY_TYPES.GRID)
        {
          //get suggestion list, but only in the correct range
          currentSuggestionList = suggestionQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        else
        {
          currentSuggestionList = suggestionQuery.ToList();
        }

      ResourceSuggestionPager output = new ResourceSuggestionPager();
      output.SuggestionList = currentSuggestionList;
      output.CurrentPageNumber = page;
      output.PageSize = pageSize;
      output.TotalSuggestionsInCurrentSet = CountAllSuggestionsForUserAndStatus(userMID, suggestionStatus);

      return output;
    }

    public List<ResourceSuggestion> GetAllSuggestionsForUser(string userMID)
    {
      if (userMID != null)
      {
        var suggestions = _context.ResourceSuggestions.Where(x => x.CreatedByMID.ToUpper() == userMID.ToUpper()).ToList();
        return suggestions;
      }
      return null;
    }


    public int CountAllSuggestions()
    {
      return _context.ResourceSuggestions.Count();
    }

    public int CountSuggestionsWithStatus(int status = SuggestionsController.ANY_SELECTION_VALUE)
    {
      if (status == SuggestionsController.ANY_SELECTION_VALUE)
        return _context.ResourceSuggestions.Count();
      else
        return _context.ResourceSuggestions.Where(x => (x.Status == status)).Count();
    }

    public int CountAllSuggestionsForUser(string userMID)
    {
      return _context.ResourceSuggestions.Where(x => x.CreatedByMID.ToUpper() == userMID.ToUpper()).Count();
    }


    public int CountAllSuggestionsForUserAndStatus(string userMID = "", int status = SuggestionsController.ANY_SELECTION_VALUE)
    {
      //start query with the ResourceSuggestion context
      IQueryable<ResourceSuggestion> suggestionQuery = _context.ResourceSuggestions;

      //limit query to only the requested user
      if (userMID != string.Empty)
      {
        suggestionQuery = suggestionQuery.Where(x => x.CreatedByMID.ToUpper() == userMID.ToUpper());
      }

      //limit query to those with the correct status type
      if (status != SuggestionsController.ANY_SELECTION_VALUE)
      {
        suggestionQuery = suggestionQuery.Where(x => (x.Status == status));
      }

      return suggestionQuery.Count();
    }


    public IEnumerable<ResourceSuggestion> All
    {
      get
      {
        return _context.ResourceSuggestions.ToList();
      }
    }

    public IEnumerable<ResourceSuggestion> AllIncluding(Expression<Func<ResourceSuggestion, bool>> filter = null, string includeProperties = "")
    {
      IQueryable<ResourceSuggestion> query = _context.ResourceSuggestions;

      if (filter != null)
      {
        query = query.Where(filter);
      }

      foreach (var includeProperty in
        includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }

      return query.ToList();
    }

    public ResourceSuggestion Find(int id)
    {
      var suggestion = _context.ResourceSuggestions.Where(x => (x.ResourceSuggestionId == id)).FirstOrDefault();

      return suggestion;
    }

    public void InsertOrUpdate(ResourceSuggestion suggestion)
    {
      if (suggestion.ResourceSuggestionId == default(int))
      {
        // New entity
        _context.ResourceSuggestions.Add(suggestion);
      }
      else
      {
        // Existing entity
        _context.Entry(suggestion).State = EntityState.Modified;
      }
      _context.SaveChanges();
    }

    public void Delete(int id)
    {
      var suggestion = _context.ResourceSuggestions.Find(id);
      _context.ResourceSuggestions.Remove(suggestion);
      _context.SaveChanges();
    }

    public void Save()
    {
      _context.SaveChanges();
    }

    public void Dispose()
    {
      _context.Dispose();
    }


    public void SetSuggestionStatus(int suggestionId, int newStatus)
    {
      ResourceSuggestion targetSuggestion = _context.ResourceSuggestions.Where(x => x.ResourceSuggestionId == suggestionId).First();
      targetSuggestion.Status = newStatus;
      _context.SaveChanges();
    }
  }

  public interface ISuggestionRepository : IDisposable
  {
    ResourceSuggestionPager GetPagedSuggestionList(string userMID = "", int page = 1, int pageSize = 5, string sortField = "CreatedOn", string sortOrder = "ASC", int suggestionStatus = SuggestionsController.ANY_SELECTION_VALUE, SuggestionsController.DISPLAY_TYPES currentDisplayType = SuggestionsController.DISPLAY_TYPES.TABLES);
    List<ResourceSuggestion> GetAllSuggestionsForUser(string userMID);
    int CountAllSuggestions();
    int CountSuggestionsWithStatus(int status = SuggestionsController.ANY_SELECTION_VALUE);
    int CountAllSuggestionsForUser(string userMID);
    int CountAllSuggestionsForUserAndStatus(string userMID = "", int status = SuggestionsController.ANY_SELECTION_VALUE);
    IEnumerable<ResourceSuggestion> All { get; }
    IEnumerable<ResourceSuggestion> AllIncluding(Expression<Func<ResourceSuggestion, bool>> filter = null, string includeProperties = "");
    void SetSuggestionStatus(int suggestionId, int newStatus); 
    ResourceSuggestion Find(int id);
    void InsertOrUpdate(ResourceSuggestion suggestion);
    void Delete(int id);
    void Save();
  }
}