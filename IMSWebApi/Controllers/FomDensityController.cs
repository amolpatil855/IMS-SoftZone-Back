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
    public class FomDensityController : ApiController
    {
        private FomDensityService _fomDensityService = null;

        public FomDensityController()
        {
            _fomDensityService = new FomDensityService();
        }

        // GET api/FomDensity   
        [ApiAuthorize(AccessLevel = "foamdensity")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _fomDensityService.getFomDensity(pageSize, page, search);
            return Ok(result);
        }

        // GET api/FomDensity/1
        [ApiAuthorize(AccessLevel = "foamdensity")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _fomDensityService.getFomDensityById(id);
            return Ok(result);
        }

        // POST api/FomDensity
        [ApiAuthorize(AccessLevel = "foamdensity")]
        [HttpPost]
        public IHttpActionResult PostFomDensity(VMFomDensity fomDensity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _fomDensityService.postFomDensity(fomDensity);
            return Ok(result);
        }

        // PUT api/FomDensity/1
        [ApiAuthorize(AccessLevel = "foamdensity")]
        [HttpPut]
        public IHttpActionResult PutFomDensity(VMFomDensity fomDensity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _fomDensityService.putFomDensity(fomDensity);
            return Ok(result);
        }

        // DELETE api/FomDensity/1
        [ApiAuthorize(AccessLevel = "foamdensity")]
        [HttpDelete]
        public IHttpActionResult DeleteFomDensity(long id)
        {
            var result = _fomDensityService.deleteFomDensity(id);
            return Ok(result);
        }
    }
}
