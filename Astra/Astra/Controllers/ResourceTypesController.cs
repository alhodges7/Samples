using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.Models;
using Astra.Repositories;
using Astra.Controllers.Shared;
using Astra.CompositeRepository;
using Astra.Helper;

namespace Astra.Controllers
{
    [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
    public class ResourceTypesController : BaseController
    {
      //emtpy default constructor
      public ResourceTypesController() : base()
      {
      }

        public ViewResult Index()
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            return View(repository.All);
          }
        }

        public ViewResult Details(int id)
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            return View(repository.Find(id));
          }
        }

        public ActionResult Create()
        {
          return View();
        } 

        [HttpPost]
        public ActionResult Create(ResourceType resourcetype)
        {
          if (ModelState.IsValid) 
          {
            using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
            {
              repository.InsertOrUpdate(resourcetype);
              repository.Save();
            }
            return RedirectToAction("Index");
          } 
          else 
          {
				    return View();
			    }
        }
        
        public ActionResult Edit(int id)
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            return View(repository.Find(id));
          }
        }

        [HttpPost]
        public ActionResult Edit(ResourceType resourcetype)
        {
            if (ModelState.IsValid) 
            {
                using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
                {
                  repository.InsertOrUpdate(resourcetype);
                  repository.Save();
                }
                return RedirectToAction("Index");
            } 
            else 
            {
				      return View();
			      }
        }

        public ActionResult Delete(int id)
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            return View(repository.Find(id));
          }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            repository.Delete(id);
            repository.Save();
          }

            return RedirectToAction("Index");
        }

        #region Helper Methods
        public SelectList ResourceTypesSelectList(ResourceType selectedType)
				{
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            return new SelectList(repository.All, "ResourceTypeID", "Name", selectedType);
          }
				}

				public ResourceType DefaultResourceType()
				{
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
          {
            return repository.Find(1);
          }
				}
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

