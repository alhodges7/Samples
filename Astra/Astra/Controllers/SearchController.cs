using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using Astra.Controllers.Shared;
using Astra.Misc;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Models.ViewModels;
using Astra.Repositories;
using MTUtil.Strings;
using MTUtil.TypeManagement;
using PagedList;
using Astra.CompositeRepository;
using Astra.Helper;

namespace Astra.Controllers
{
    public class SearchController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

    }
}
