using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationForm.Controllers
{

    public class UserDataController : Controller
    {
        public JsonResult GetPerson()
        {
            List<Person> persons = new List<Person>();
            persons.Add(new Person("Al", "Hodges"));
            persons.Add(new Person("Dave", "Clark"));
            return Json(persons, JsonRequestBehavior.AllowGet);
        }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Person(string first, string last)
        {
            FirstName = first;
            LastName = last;
        }
    }
}
