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
    public class MatThickNessController : ApiController
    {
        private MatThickNessService _matThickNessService = null;

        public MatThickNessController()
        {
            _matThickNessService = new MatThickNessService();
        }

        // GET api/MatThickNess   
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _matThickNessService.getMatThickNess(pageSize, page, search);
            return Ok(result);
        }

        // GET api/MatThickNess/1
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _matThickNessService.getMatThickNessById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/MatThickNess/GetMattressThickNessLookUp")]
        public IHttpActionResult GetMatThickNessLookUp()
        {
            var result = _matThickNessService.getMatThickNessLookUp();
            return Ok(result);
        }

        // POST api/MatThickNess
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpPost]
        public IHttpActionResult PostMatThickNess(VMMatThickNess matThickNess)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _matThickNessService.postMatThickNess(matThickNess);
            return Ok(result);
        }

        // PUT api/MatThickNess/1
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpPut]
        public IHttpActionResult PutMatThickNess(VMMatThickNess matThickNess)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _matThickNessService.putMatThickNess(matThickNess);
            return Ok(result);
        }

        // DELETE api/MatThickNess/1
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpDelete]
        public IHttpActionResult DeleteMattressThickNess(long id)
        {
            var result = _matThickNessService.deleteMatThickNess(id);
            return Ok(result);
        }
    }
}
