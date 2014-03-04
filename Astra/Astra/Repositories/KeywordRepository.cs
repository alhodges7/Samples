using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.CompositeRepository;
using Astra.DatabaseContext;
using Astra.Models;

namespace Astra.Repositories
{
  public class KeywordRepository : IKeywordRepository
  {
    private AstraContext _context = null;
  
    public KeywordRepository(AstraContext context)
    {
      _context = context;
    }

    public void Dispose()
    {
 	    // Empty
    }

    public IEnumerable<KeyWord> All
    {
      get
      {
        return _context.KeyWords.ToList();
      }
    }

    public bool DeleteKeyWord(int keywordId)
    {      
      
      using (var repository = new ScopedCompositeRepository())
      {
        

        var links = repository.Repositories.KeyWordLinksRepository.GetLinksByKeyword(keywordId).ToList();
        while (links.Count() > 0)
        {
          ResourceToKeyWordLink link = links.Last();          
          repository.Repositories.KeyWordLinksRepository.DeleteResourceToKeyWordLink(link.LinkID);
          links.Remove(link);
        }

        _context.KeyWords.Remove(_context.KeyWords.Where(x => x.KeyWordID == keywordId).First());
        _context.SaveChanges();
      }

      return true;
    }

    public KeyWord Find(int id)
    {
      return _context.KeyWords.Where(x => x.KeyWordID == id).First();
    }

    public KeyWord FindByWord(string word)
    {
      return _context.KeyWords.Where(kw => kw.Word == word).FirstOrDefault();
    }

    public void CreateKeyword(KeyWord newKeyword)
    {
      newKeyword.Word = newKeyword.Word.Trim();
      if (!_context.KeyWords.Where(k => k.Word == newKeyword.Word).Any())
      {
        _context.KeyWords.Add(newKeyword);
        _context.SaveChanges();
      }
    }
  }

  public interface IKeywordRepository : IDisposable
  {
    IEnumerable<KeyWord> All { get; }
    void CreateKeyword(KeyWord newKeyword);
    bool DeleteKeyWord(int keywordId);
    KeyWord Find(int id);
    KeyWord FindByWord(string word);
  }
}