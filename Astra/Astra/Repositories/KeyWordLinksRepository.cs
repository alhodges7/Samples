using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Astra.CompositeRepository;
using Astra.Controllers;
using Astra.DatabaseContext;
using Astra.Models;

namespace Astra.Repositories
{
    public class KeyWordLinksRepository : IKeyWordLinksRepository
    {
        private AstraContext _context = null;

        public KeyWordLinksRepository(AstraContext context)
        {
          _context = context;
        }

        public IEnumerable<ResourceToKeyWordLink> GetAllResourceToKeyWordLinks()
        {
          var resourcetokeywordlinks = _context.ResourceToKeyWordLinks.Include(r => r.KeyWord).Include(r => r.Resource);
          return resourcetokeywordlinks.AsEnumerable();
        }

        public ResourceToKeyWordLink GetResourceToKeyWordLink(int id)
        {
          ResourceToKeyWordLink resourcetokeywordlink = _context.ResourceToKeyWordLinks.Find(id);

          return resourcetokeywordlink;
        }

				public ResourceToKeyWordLink GetFullyLoadedResourceLink(int id)
				{
          ResourceToKeyWordLink rl = _context.ResourceToKeyWordLinks.Include("KeyWord").First<Astra.Models.ResourceToKeyWordLink>(wl => wl.LinkID == id);
					return rl;
				}

        public ResourceToKeyWordLink CreateKeyWordLink(Resource r, KeyWord keyWord)
        {
          ResourceToKeyWordLink link = new ResourceToKeyWordLink();
          link.ResourceID = r.ResourceID;
          link.KeyWordID = keyWord.KeyWordID;

          _context.ResourceToKeyWordLinks.Add(link);
          _context.SaveChanges();
          return link;
        }

        public void DeleteResourceToKeyWordLink(int id)
        {
          ResourceToKeyWordLink resourcetokeywordlink = _context.ResourceToKeyWordLinks.Find(id);

          _context.ResourceToKeyWordLinks.Remove(resourcetokeywordlink);
          _context.SaveChanges();
        }

        public IEnumerable<ResourceToKeyWordLink> GetLinksByKeyword(int keywordId)
        {
          return _context.ResourceToKeyWordLinks.Where(x => x.KeyWordID == keywordId).Include(r => r.KeyWord).Include(r => r.Resource).ToList();
        }

        public void Dispose()
        {
          // Intentionally empty.
        }

        public IEnumerable<ResourceToKeyWordLink> AllIncluding(Expression<Func<ResourceToKeyWordLink, bool>> filter = null, string includeProperties = "")
        {
          IQueryable<ResourceToKeyWordLink> query = _context.ResourceToKeyWordLinks;

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



        public IEnumerable<KeyWordTally> GetKeywordLinkTallies()
        {
          return _context.Database.SqlQuery<KeyWordTally>(@"SELECT KeyWordID, COUNT(ResourceID) LinkCount
              FROM ResourceToKeyWordLinks 
              GROUP BY KeyWordID").ToList();
        }


        public IEnumerable<KeyWord> GetSelectedWordsForResource(int resourceId)
        {
          Astra.Models.Resource resource = null;

          using (var repositories = new ScopedCompositeRepository().Repositories)
          {
            List<Astra.Models.KeyWord> selectedWords = new List<Astra.Models.KeyWord>();

            resource = repositories.ResourceRepository.AllCommittedIncluding(x => x.ResourceID == resourceId).First();

            foreach (Astra.Models.ResourceToKeyWordLink link in resource.KeyWordLinks)
            {
              Astra.Models.ResourceToKeyWordLink fullLoadedLink = repositories.KeyWordLinksRepository.GetFullyLoadedResourceLink(link.LinkID);
              selectedWords.Add(fullLoadedLink.KeyWord);
            }

            return selectedWords;
          }
        }
    }

    public interface IKeyWordLinksRepository : IDisposable
    {
      IEnumerable<ResourceToKeyWordLink> AllIncluding(Expression<Func<ResourceToKeyWordLink, bool>> filter = null, string includeProperties = "");
      IEnumerable<ResourceToKeyWordLink> GetAllResourceToKeyWordLinks();
      ResourceToKeyWordLink GetResourceToKeyWordLink(int id);
			ResourceToKeyWordLink GetFullyLoadedResourceLink(int id);
      IEnumerable<ResourceToKeyWordLink> GetLinksByKeyword(int keywordId);
      IEnumerable<KeyWordTally> GetKeywordLinkTallies();
      IEnumerable<KeyWord> GetSelectedWordsForResource(int resourceId);
      ResourceToKeyWordLink CreateKeyWordLink(Resource r, KeyWord keyWord);
      void DeleteResourceToKeyWordLink(int id);
    }
}