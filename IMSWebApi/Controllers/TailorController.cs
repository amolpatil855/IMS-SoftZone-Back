using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class TailorController : ApiController
    {
        private TailorService _tailorService = null;

        public TailorController()
        {
            _tailorService = new TailorService();
        }

        // GET api/Tailor
        [ApiAuthorize(AccessLevel = "tailor")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _tailorService.getTailors(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Tailor/1
        [ApiAuthorize(AccessLevel = "tailor")]
        [HttpGet]
        public IHttpActionResult Get(Int64 id)
        {
            var result = _tailorService.getTailorById(id);
            return Ok(result);
        }

        // POST api/Tailor
        [ApiAuthorize(AccessLevel = "tailor")]
        [HttpPost]
        public IHttpActionResult postTailor(VMTailor tailor)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_tailorService.postTailor(tailor));
        }

        // PUT api/Tailor/
        [ApiAuthorize(AccessLevel = "tailor")]
        [HttpPut]
        public IHttpActionResult PutTailor(VMTailor tailor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _tailorService.putTailor(tailor);
            return Ok(result);
        }

        // DELETE api/Tailor/1
        [ApiAuthorize(AccessLevel = "tailor")]
        [HttpDelete]
        public IHttpActionResult DeleteTailor(long id)
        {
            var result = _tailorService.deleteTailor(id);
            return Ok(result);
        }
    }
}
