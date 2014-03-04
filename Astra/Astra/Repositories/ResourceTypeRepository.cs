using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Astra.DatabaseContext;
using Astra.Models;
namespace Astra.Repositories
{ 
    public class ResourceTypeRepository : IResourceTypeRepository
    {
        private AstraContext _context = null;

        public ResourceTypeRepository(AstraContext context)
        {
          _context = context;
        }

        public IEnumerable<ResourceType> All
        {
          get 
          { 
            return _context.ResourceTypes.OrderBy(r => r.Name).ToList(); 
          }
        }

        public IEnumerable<ResourceType> AllIncluding(Expression<Func<ResourceType, bool>> filter = null, string includeProperties = "")
        {
          IQueryable<ResourceType> query = _context.ResourceTypes;

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

        public ResourceType Find(int id)
        {
          return _context.ResourceTypes.Find(id);
        }

        public void InsertOrUpdate(ResourceType resourcetype)
        {
            if (resourcetype.ResourceTypeID == default(int)) {
                // New entity
              _context.ResourceTypes.Add(resourcetype);
            } 
            else 
            {
                // Existing entity
              _context.Entry(resourcetype).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
          var resourcetype = _context.ResourceTypes.Find(id);
          _context.ResourceTypes.Remove(resourcetype);
        }

        public void Save()
        {
          _context.SaveChanges();
        }

        public void Dispose() 
        {
          _context.Dispose();
        }
    }

    public interface IResourceTypeRepository : IDisposable
    {
        IEnumerable<ResourceType> All { get; }
        IEnumerable<ResourceType> AllIncluding(Expression<Func<ResourceType, bool>> filter = null, string includeProperties = "");
        ResourceType Find(int id);
        void InsertOrUpdate(ResourceType resourcetype);
        void Delete(int id);
        void Save();
    }
}