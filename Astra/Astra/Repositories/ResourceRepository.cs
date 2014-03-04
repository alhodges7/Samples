using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.IO;
using Astra.CompositeRepository;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Models.ResourceTypes;
using Astra.Models.ViewModels;
using Astra.AstraConfigurations;

namespace Astra.Repositories
{
  public class ResourceRepository : IResourceRepository
  {
    private AstraContext _context;
    //good
    public static TimeSpan DEFAULT_UNCOMMIT_DELETE_SPAN
    {
      get
      {
        // By default, we delete any uncommitted newResources that are over an hour old.
        return new TimeSpan(1, 0, 0);
      }
    }
    //good
    public ResourceRepository(AstraContext context)
    {
      _context = context;
    }

    public IEnumerable<Resource> All
    {
      get
      {
        return _context.Resources.Include("ResourceType").ToList();
      }
    }

    public IEnumerable<Resource> AllIncluding(Expression<Func<Resource, bool>> filter = null, string includeProperties = "")
    {
      IQueryable<Resource> query = _context.Resources;

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
    //No Update Needed Maybe!!!
    public IEnumerable<Resource> AllCommitted
    {
      get { return _context.Resources.Where(x => x.Committed == true).Include("ResourceType").ToList(); }
    }
    //Updated
    private IQueryable<T> QueryableAllCommitted<T>() where T : Resource
    {

      return _context.Resources.OfType<T>().Where(x => x.Committed == true);

    }

    public IEnumerable<Resource> AllCommittedIncluding(Expression<Func<Resource, bool>> filter = null, string includeProperties = "")
    {
      var query = _context.Resources.Where(x => x.Committed == true);

      if (filter != null)
      {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }

      return query.ToList();
    }
    //Updated
    public T Find<T>(int id) where T : Resource
    {
      var resource = _context.Resources.OfType<T>().Where(x => x.ResourceID == id).Include("CoverImage").FirstOrDefault();

      return resource;
    }
    //Needs to be fixed
    public List<Resource> FindByKeyWord(List<string> keyWords)
    {
      List<Resource> searchResult = new List<Resource>();
      if (!keyWords.Any())
      {
          return searchResult;
      }
      
      var searchKeys = new HashSet<string>(keyWords.Select(s => s.ToLower()));

      var searchQuery = ( from resources in this.QueryableAllCommitted<Resource>()
                          from keys in _context.ResourceToKeyWordLinks.Where(e => e.ResourceID == resources.ResourceID)
                          from keymatch in _context.KeyWords.Where(c => c.KeyWordID == keys.KeyWordID && searchKeys.Contains(c.Word.ToLower())).DefaultIfEmpty()
                          from titlewords in searchKeys.Where(t => resources.Title.ToLower().Contains(t) || keymatch.KeyWordID == keys.KeyWordID)
                          select resources).Include("CoverImage").Distinct().Where(x => x.Committed == true).OrderBy(x => x.Title);

      searchResult = searchQuery.ToList();
      foreach (Resource item in searchResult)
      {
        item.CalculateRelevanceAsSearchResult(keyWords);
      }

      return searchResult;
    }
    //Needs Update
    public void InsertOrUpdate(Resource resource)
    {
      if (resource.ResourceID == default(int))
      {
        // New entity
        _context.Resources.Add(resource);
      }
      else
      {
        // Existing entity
        var entry = _context.Entry(resource);

        if (entry.State == EntityState.Detached)
        {
          var attachedResource = _context.Resources.Find(resource.ResourceID);

          var attachedEntry = _context.Entry(attachedResource);
          attachedEntry.CurrentValues.SetValues(resource);
        }
        else
        {
          entry.State = EntityState.Modified; // This should attach entity
        }
      }

      _context.SaveChanges();
    }
    //Needs Update
    public void Delete(int id)
    {
      var resourceSearch = _context.Resources.Where(x => x.ResourceID == id).Include("CoverImage").Include("Images");
      if (resourceSearch.Count() != 1)
      {
        throw new Exception("Could not locate resource for deletion.");
      }
      var targetResource = resourceSearch.First();

      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        if (targetResource.CoverImage != null)
        {
          repositories.MiscRepository.DeleteImage(targetResource.CoverImage.ID);
        }
        foreach (var image in targetResource.Images)
        {
          repositories.MiscRepository.DeleteImage(image.ID);
        }
      }

      _context.Resources.Remove(targetResource);

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

    public List<Resource> GetAvailableResources()
    {
      var resourceList = _context.Resources.Include("CheckOutList").ToList();
      var filteredList = resourceList.Where(x => x.IsAvailable).ToList();

      return filteredList;
    }

    /// <summary>
    /// Advanced Search for Books
    /// </summary>
    /// <param name="searchValues"></param>
    /// <returns> List of Books</returns>
    public List<Resource> AdvancedSearch(Resource searchValues)
    {
      if (searchValues.GetType() == typeof(Book))
      {
        return SearchForBook((Book)searchValues);
      }
      else if (searchValues.GetType() == typeof(DVD))
      {
        return SearchForDVD((DVD)searchValues);
      }
      else if (searchValues.GetType() == typeof(EBook))
      {
        return SearchForEBook((EBook)searchValues);
      }
      else if (searchValues.GetType() == typeof(Hardware))
      {
        return SearchForHardware((Hardware)searchValues);
      }
      else if (searchValues.GetType() == typeof(Software))
      {
        return SearchForSoftware((Software)searchValues);
      }
      else if (searchValues.GetType() == typeof(WhitePaper))
      {
        return SearchForWhitePaper((WhitePaper)searchValues);
      }

      return null;
    }

    private List<Resource> SearchForBook(Book searchValues)
    {

      List<string> keywords = new List<string>();
      IQueryable<Resource> searchResult = null;

      foreach (var item in searchValues.KeyWords)
      {
        keywords.Add(item);
      }
      if (keywords.Contains("(Any)"))
      {
        keywords.Remove("(Any)");
      }
      //checks ISBN first if not found will run full search
      if (searchValues.ISBN10 != null)
      {
        searchResult = this.QueryableAllCommitted<Book>().Where(r => r.ISBN10 == searchValues.ISBN10);
      }
      else if (searchValues.ISBN13 != null)
      {
        searchResult = this.QueryableAllCommitted<Book>().Where(r => r.ISBN13 == searchValues.ISBN13);
      }
      else if (searchValues.ISBN13 == null && searchValues.ISBN10 == null && !IsSearchEmpty(searchValues))
      {
        searchResult = (from resources in this.QueryableAllCommitted<Book>()
                      .Where(r => searchValues.Title == null || r.Title.ToLower().Contains(searchValues.Title.ToLower()))
                      .Where(r => searchValues.Author == null || r.Author.ToLower().Contains(searchValues.Author.ToLower()))
                      .Where(r => searchValues.ResourceTypeID == null || r.ResourceTypeID == searchValues.ResourceTypeID)
                      from keys in _context.ResourceToKeyWordLinks.Where(e => keywords.Count == 0 || e.ResourceID == resources.ResourceID)
                      from keydesc in _context.KeyWords.Where(c => keywords.Count == 0 || (c.KeyWordID == keys.KeyWordID && keywords.Contains(c.Word)))
                      select resources).Distinct().OrderBy(x => x.Title);
      }
      else
      {
        return new List<Resource>();
      }
      return searchResult.Include("CoverImage").ToList();
    }
    private List<Resource> SearchForDVD(DVD searchValues)
    {

      List<string> keywords = new List<string>();
      IQueryable<Resource> searchResult = null;

      foreach (var item in searchValues.KeyWords)
      {
        keywords.Add(item);
      }
      if (keywords.Contains("(Any)"))
      {
        keywords.Remove("(Any)");
      }
      //checks ISBN first if not found will run full search

      if (!IsSearchEmpty(searchValues))
      {
        searchResult = (from resources in this.QueryableAllCommitted<DVD>()
                      .Where(r => searchValues.Title == null || r.Title.ToLower().Contains(searchValues.Title.ToLower()))
                      .Where(r => searchValues.Directors == null || r.Directors.ToLower().Contains(searchValues.Directors.ToLower()))
                      .Where(r => searchValues.Language == null || r.Language.ToLower().Contains(searchValues.Language.ToLower()))
                      .Where(r => searchValues.Studio == null || r.Studio.ToLower().Contains(searchValues.Studio.ToLower()))
                      .Where(r => searchValues.ResourceTypeID == null || r.ResourceTypeID == searchValues.ResourceTypeID)
                      from keys in _context.ResourceToKeyWordLinks.Where(e => keywords.Count == 0 || e.ResourceID == resources.ResourceID)
                      from keydesc in _context.KeyWords.Where(c => keywords.Count == 0 || (c.KeyWordID == keys.KeyWordID && keywords.Contains(c.Word)))
                      select resources).Distinct().OrderBy(x => x.Title);
      }
      else
      {
        return new List<Resource>();
      }
      return searchResult.Include("CoverImage").ToList();
    }//done
    private List<Resource> SearchForEBook(EBook searchValues)
    {

      List<string> keywords = new List<string>();
      IQueryable<Resource> searchResult = null;

      foreach (var item in searchValues.KeyWords)
      {
        keywords.Add(item);
      }
      if (keywords.Contains("(Any)"))
      {
        keywords.Remove("(Any)");
      }
      //checks ISBN first if not found will run full search
      if (searchValues.ISBN10 != null)
      {
        searchResult = this.QueryableAllCommitted<EBook>().Where(r => r.ISBN10 == searchValues.ISBN10);
      }
      else if (searchValues.ISBN13 != null)
      {
        searchResult = this.QueryableAllCommitted<EBook>().Where(r => r.ISBN13 == searchValues.ISBN13);
      }
      else if (searchValues.ISBN13 == null && searchValues.ISBN10 == null && !IsSearchEmpty(searchValues))
      {
        searchResult = (from resources in this.QueryableAllCommitted<EBook>()
                      .Where(r => searchValues.Title == null || r.Title.ToLower().Contains(searchValues.Title.ToLower()))
                      .Where(r => searchValues.Author == null || r.Author.ToLower().Contains(searchValues.Author.ToLower()))
                      from keys in _context.ResourceToKeyWordLinks.Where(e => keywords.Count == 0 || e.ResourceID == resources.ResourceID)
                      from keydesc in _context.KeyWords.Where(c => keywords.Count == 0 || (c.KeyWordID == keys.KeyWordID && keywords.Contains(c.Word)))
                      select resources).Distinct().OrderBy(x => x.Title);
      }
      else
      {
        return new List<Resource>();
      }
      return searchResult.Include("CoverImage").ToList();
    }//done
    private List<Resource> SearchForHardware(Hardware searchValues)
    {

      List<string> keywords = new List<string>();
      IQueryable<Resource> searchResult = null;

      foreach (var item in searchValues.KeyWords)
      {
        keywords.Add(item);
      }
      if (keywords.Contains("(Any)"))
      {
        keywords.Remove("(Any)");
      }

      if (!IsSearchEmpty(searchValues))
      {
        searchResult = (from resources in this.QueryableAllCommitted<Hardware>()
                      .Where(r => searchValues.Title == null || r.Title.ToLower().Contains(searchValues.Title.ToLower()))
                      .Where(r => searchValues.Modelno == null || r.Modelno.ToLower() == searchValues.Modelno.ToLower())
                      .Where(r => searchValues.Description == null || r.Description.ToLower().Contains(searchValues.Description.ToLower()))
                      from keys in _context.ResourceToKeyWordLinks.Where(e => keywords.Count == 0 || e.ResourceID == resources.ResourceID)
                      from keydesc in _context.KeyWords.Where(c => keywords.Count == 0 || (c.KeyWordID == keys.KeyWordID && keywords.Contains(c.Word)))
                      select resources).Distinct().OrderBy(x => x.Title);
      }
      else
      {
        return new List<Resource>();
      }
      return searchResult.Include("CoverImage").ToList();
    }
    private List<Resource> SearchForSoftware(Software searchValues)
    {

      List<string> keywords = new List<string>();
      IQueryable<Resource> searchResult = null;

      foreach (var item in searchValues.KeyWords)
      {
        keywords.Add(item);
      }
      if (keywords.Contains("(Any)"))
      {
        keywords.Remove("(Any)");
      }
      //checks ISBN first if not found will run full search

      if (!IsSearchEmpty(searchValues))
      {
        searchResult = (from resources in this.QueryableAllCommitted<Software>()
                      .Where(r => searchValues.Title == null || r.Title.ToLower().Contains(searchValues.Title.ToLower()))
                      .Where(r => searchValues.Description == null || r.Description.ToLower().Contains(searchValues.Description.ToLower()))
                      from keys in _context.ResourceToKeyWordLinks.Where(e => keywords.Count == 0 || e.ResourceID == resources.ResourceID)
                      from keydesc in _context.KeyWords.Where(c => keywords.Count == 0 || (c.KeyWordID == keys.KeyWordID && keywords.Contains(c.Word)))
                      select resources).Distinct().OrderBy(x => x.Title);
      }
      else
      {
        return new List<Resource>();
      }
      return searchResult.Include("CoverImage").ToList();
    }
    private List<Resource> SearchForWhitePaper(WhitePaper searchValues)
    {

      List<string> keywords = new List<string>();
      IQueryable<Resource> searchResult = null;

      foreach (var item in searchValues.KeyWords)
      {
        keywords.Add(item);
      }
      if (keywords.Contains("(Any)"))
      {
        keywords.Remove("(Any)");
      }
      //checks ISBN first if not found will run full search

      if (!IsSearchEmpty(searchValues))
      {
        searchResult = (from resources in this.QueryableAllCommitted<WhitePaper>()
                     .Where(r => searchValues.Title == null || r.Title.ToLower().Contains(searchValues.Title.ToLower()))
                     .Where(r => searchValues.Description == null || r.Description.ToLower().Contains(searchValues.Description.ToLower()))
                        from keys in _context.ResourceToKeyWordLinks.Where(e => keywords.Count == 0 || e.ResourceID == resources.ResourceID)
                        from keydesc in _context.KeyWords.Where(c => keywords.Count == 0 || (c.KeyWordID == keys.KeyWordID && keywords.Contains(c.Word)))
                        select resources).Distinct().OrderBy(x => x.Title);
      }
      else
      {
        return new List<Resource>();
      }
      return searchResult.Include("CoverImage").ToList();
    }


    private bool IsSearchEmpty(Resource input)
    {

      if (input.GetType() == typeof(Book))
      {
        Book searchvalues = (Book)input;
        if (searchvalues.Title == null && searchvalues.Author == null && searchvalues.KeyWords.Count == 0)
        {
          return true;
        }
      }
      else if (input.GetType() == typeof(DVD))
      {
        DVD searchvalues = (DVD)input;
        if (searchvalues.Title == null && searchvalues.Directors == null && searchvalues.Language == null && searchvalues.Studio == null && searchvalues.KeyWords.Count == 0)
        {
          return true;
        }
      }
      else if (input.GetType() == typeof(EBook))
      {
        EBook searchvalues = (EBook)input;
        if (searchvalues.Title == null && searchvalues.Author == null && searchvalues.KeyWords.Count == 0)
        {
          return true;
        }
      }
      else if (input.GetType() == typeof(Hardware))
      {
        Hardware searchvalues = (Hardware)input;
        if (searchvalues.Title == null && searchvalues.Modelno == null && searchvalues.Description == null && searchvalues.KeyWords.Count == 0)
        {
          return true;
        }
      }
      else if (input.GetType() == typeof(Software))
      {
        Software searchvalues = (Software)input;
        if (searchvalues.Title == null && searchvalues.Description == null  && searchvalues.KeyWords.Count == 0)
        {
          return true;
        }
      }
      else if (input.GetType() == typeof(WhitePaper))
      {
        WhitePaper searchvalues = (WhitePaper)input;
        if (searchvalues.Title == null && searchvalues.Description == null && searchvalues.KeyWords.Count == 0)
        {
          return true;
        }
      }

      return false;
    }

    public Resource CreateStubResource(string resourceType)
    {
      Resource newResource = null;

      switch (resourceType)
      {
        case "Book":
          {
            Book book = new Book
            {
              Title = "TEMP_TITLE",
              Author = "TEMP_AUTHOR",
              Committed = false,
              ResourceTypeID = 1
            };
            AddNewResourceAndSaveChanges(book);
            return book;
          }
        case "DVD":
          {
            DVD dvd = new DVD
            {
              Title = "TEMP_TITLE",
              Committed = false,
              ResourceTypeID = 2,
            };
            AddNewResourceAndSaveChanges(dvd);
            return dvd;
          }
        case "EBook":
          {
            EBook ebook = new EBook
            {
              Title = "TEMP_TITLE",
              Author = "TEMP_AUTHOR",
              Committed = false,
              ResourceTypeID = 3,
            };
            AddNewResourceAndSaveChanges(ebook);
            return ebook;
          }
        case "Hardware":
          {
            Hardware hardware = new Hardware
            {
              Title = "TEMP_TITLE",
              Committed = false,
              ResourceTypeID = 4,
            };
            AddNewResourceAndSaveChanges(hardware);
            return hardware;
          }
        case "Software":
          {
            Software software = new Software
            {
              Title = "TEMP_TITLE",
              Committed = false,
              ResourceTypeID = 5,
            };
            AddNewResourceAndSaveChanges(software);
            return software;
          }
        case "White Paper":
          {
            WhitePaper whitepaper = new WhitePaper
            {
              Title = "TEMP_TITLE",
              Committed = false,
              ResourceTypeID = 6,
            };
            AddNewResourceAndSaveChanges(whitepaper);
            return whitepaper;
          }
      }
      return newResource;
    }

    public void ClearUncommittedResources(TimeSpan deleteSpan)
    {
      var search = _context.Resources.Where(x => !x.Committed).ToList();
      var filteredSearch = search.Where(x => SpanFilter(x.CreatedOn.ToLocalTime(), deleteSpan)).ToList();

      foreach (var item in filteredSearch)
      {
        this.Delete(item.ResourceID);
      }
    }

    private bool SpanFilter(DateTime targetDate, TimeSpan testSpan)
    {
      var targetSpan = DateTime.UtcNow - targetDate;

      return targetSpan > testSpan;
    }

    public List<Resource> CheckForNewResources(string lastNewResourceCheck)
    {
      List<Resource> newResources;
      DateTime lastCheck = DateTime.MinValue;
      if (lastNewResourceCheck != null)
      {
        DateTime.TryParse(lastNewResourceCheck, out lastCheck);
      }
      if (lastCheck == DateTime.MinValue)
      {
        newResources = _context.Resources.ToList();
      }
      else
      {
        newResources = _context.Resources.Where(x => x.CreatedOn > lastCheck).ToList();

      }
      if (newResources.Any())
      {
        AstraConfigurationManager.NewResourceSettings().LastNewResourceCheck = DateTime.UtcNow.ToString();
        // save will be auto now
        return newResources;
      }
      else
      {
        newResources = new List<Resource>();
        AstraConfigurationManager.NewResourceSettings().LastNewResourceCheck = DateTime.UtcNow.ToString();
        // save will be auto now
        return newResources;
      }
    }

    public void AddNewResourceAndSaveChanges(Resource newResource)
    {
      _context.Resources.Add(newResource);
      _context.SaveChanges();
    }

    public int SyncKeyWords(Resource resource)
    {
      //Make sure we have no duplication in the KeyWorks list      
      Dictionary<string, string> words = new Dictionary<string, string>();
      for (int x = resource.KeyWords.Count - 1; x >= 0; x--)
      {
        string word = resource.KeyWords[x];
        if (words.ContainsKey(word.ToLower()))
          resource.KeyWords.RemoveAt(x);
        else
          words[word.ToLower()] = word.ToLower();
      }

      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        // remove any links that aren't in KeyWords collection
        for (int x = resource.KeyWordLinks.Count - 1; x >= 0; x--)
        {
          ResourceToKeyWordLink link = resource.KeyWordLinks[x];
          ResourceToKeyWordLink fullyLoadedLink = repositories.KeyWordLinksRepository.GetFullyLoadedResourceLink(link.LinkID);
          string match = resource.KeyWords.Find(delegate(string kw)
              {
                return kw == fullyLoadedLink.KeyWord.Word;
              });

          if (match == null)
          {
            resource.KeyWordLinks.RemoveAt(x);
            repositories.KeyWordLinksRepository.DeleteResourceToKeyWordLink(fullyLoadedLink.LinkID);
          }
        }

        // add in any links that are new
        foreach (string simpleWord in resource.KeyWords)
        {
          ResourceToKeyWordLink match = resource.KeyWordLinks.Find(delegate(ResourceToKeyWordLink currLink)
          {
            ResourceToKeyWordLink fullyLoadedLink = currLink;
            if (currLink.LinkID > 0)
              fullyLoadedLink = repositories.KeyWordLinksRepository.GetFullyLoadedResourceLink(currLink.LinkID);
            return fullyLoadedLink.KeyWord.Word == simpleWord;
          });

          if (match == null)
          {
            KeyWord actualKeyWord = repositories.KeywordRepository.FindByWord(simpleWord);
            ResourceToKeyWordLink addedLink = repositories.KeyWordLinksRepository.CreateKeyWordLink(resource, actualKeyWord);
          }
        }


      }
      _context.SaveChanges();
      return resource.KeyWordLinks.Count;
    }
   
  }

  public interface IResourceRepository : IDisposable
  {
    IEnumerable<Resource> All { get; }
    IEnumerable<Resource> AllCommitted { get; }
    IEnumerable<Resource> AllIncluding(Expression<Func<Resource, bool>> filter = null, string properties = "");
    IEnumerable<Resource> AllCommittedIncluding(Expression<Func<Resource, bool>> filter = null, string properties = "");
    Resource CreateStubResource(string resourceType);
    void AddNewResourceAndSaveChanges(Resource newResource);
    T Find<T>(int id) where T : Resource;
    List<Resource> FindByKeyWord(List<string> keyWords);
    void InsertOrUpdate(Resource resource);
    void Delete(int id);
    void Save();
    List<Resource> GetAvailableResources();
    List<Resource> AdvancedSearch(Resource searchValues);
    void ClearUncommittedResources(TimeSpan deleteSpan);
    List<Resource> CheckForNewResources(string lastNewBookCheck);
    int SyncKeyWords(Resource repository);
  }
}