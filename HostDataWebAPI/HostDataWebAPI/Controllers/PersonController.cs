using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace HostDataWebAPI.Controllers
{
    public class PersonController : ApiController
    {
        public string Get()
        {
            return ("Al");
        }
    }
}
