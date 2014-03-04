using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Astra.DatabaseContext;
using Astra.Models;

namespace Astra.Repositories
{
  public class MiscRepository : IMiscRepository
  {
    private AstraContext _context = null;

    public MiscRepository(AstraContext context)
    {
      _context = context;
    }

    public IEnumerable<ResourceToKeyWordLink> GetResourceToKeyWordLinksForResource(int resourceId)
    {
      return _context.ResourceToKeyWordLinks.Where(l => l.ResourceID == resourceId).ToList();
    }

    public void Dispose()
    {
      _context.Dispose();
    }


    public void AddImage(int resourceId, ResourceImage newImage, bool isCoverImage)
    {
      Resource targetResource = _context.Resources.Where(x => x.ResourceID == resourceId).First();

      if (isCoverImage)
      {
        if (targetResource.CoverImage != null)
        {
          _context.Images.Remove(targetResource.CoverImage);
        }

        targetResource.CoverImage = newImage;
      }
      else
      {
        targetResource.Images.Add(newImage);
      }
      _context.SaveChanges();
    }

    public void DeleteImage(int imageId)
    {
      var resourcesWithCoverImage = _context.Resources.Where(x => x.CoverImageId == imageId);

      foreach (var item in resourcesWithCoverImage)
      {
        item.CoverImageId = null;
        item.CoverImage = null;
      }

      var resourcesWithImages = _context.Resources.Where(x => x.Images.Where(y => y.ID == imageId).Any()).Include("Images");

      foreach (var item in resourcesWithImages)
      {
        item.Images.Remove(item.Images.Where(x => x.ID == imageId).First());
      }

      ResourceImage targetImage = _context.Images.Where(x => x.ID == imageId).FirstOrDefault();
      _context.Images.Remove(targetImage);

      _context.SaveChanges();
    }

    public ResourceImage FindImage(int imageId)
    {
      return _context.Images.Where(x => x.ID == imageId).FirstOrDefault();
    }


    public void DeleteCoverImage(int resourceId)
    {
      Resource targetResource = _context.Resources.Include("CoverImage").Where(x => x.ResourceID == resourceId).FirstOrDefault();
      if (targetResource != null)
      {
        ResourceImage targetImage = _context.Images.Where(x => x.ID == targetResource.CoverImage.ID).FirstOrDefault();
        _context.Images.Remove(targetImage);

        targetResource.CoverImage = null;
      }

      _context.SaveChanges();
    }
  }

  public interface IMiscRepository: IDisposable
  {
    IEnumerable<ResourceToKeyWordLink> GetResourceToKeyWordLinksForResource(int resourceId);
    void AddImage(int resourceId, ResourceImage newImage, bool isCoverImage);
    void DeleteImage(int imageId);
    void DeleteCoverImage(int resourceId);
    ResourceImage FindImage(int imageId);
  }
}