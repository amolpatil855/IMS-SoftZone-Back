using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.CustomAttributes;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        [ApiAuthorize(AccessLevel ="All")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        // POST api/values
        public int Add(int id,string name)
        {
            return id;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}