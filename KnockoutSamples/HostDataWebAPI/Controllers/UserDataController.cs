using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace HostDataWebAPI.Controllers
{

    public class UserDataController : ApiController
    {
        public JsonResult<IList<Person>> GetPerson()
        {
            IList<Person> persons = new List<Person>();
            persons.Add(new Person("Al", "Hodges"));
            persons.Add(new Person("Dave", "Clark"));

            return Json<IList<Person>>(persons);
        }
    }

    public class Person
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
