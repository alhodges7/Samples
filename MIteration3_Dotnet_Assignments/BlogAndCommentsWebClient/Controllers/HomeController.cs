using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel;

namespace BlogAndCommentsWebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Service1Client client = new Service1Client();
            string output = client.TestNewMethod("4200");

            ViewBag.Message = "Return from BlogAndCommentsWCFLib's TestNewMethod method call: " + output;

            client.Close();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
