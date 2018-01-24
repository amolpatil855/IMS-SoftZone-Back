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
    public class FomSizeController : ApiController
    {
        private FomSizeService _fomSizeService = null;

        public FomSizeController()
        {
            _fomSizeService = new FomSizeService();
        }

        // GET api/FomSize   
        [ApiAuthorize(AccessLevel = "foamsize")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _fomSizeService.getFomSize(pageSize, page, search);
            return Ok(result);
        }

        // GET api/FomSize/1
        [ApiAuthorize(AccessLevel = "foamsize")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _fomSizeService.getFomSizeById(id);
            return Ok(result);
        }

        // POST api/FomSize
        [ApiAuthorize(AccessLevel = "foamsize")]
        [HttpPost]
        public IHttpActionResult PostFomSize(VMFomSize fomSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _fomSizeService.postFomSize(fomSize);
            return Ok(result);
        }

        // PUT api/FomSize
        [ApiAuthorize(AccessLevel = "foamsize")]
        [HttpPut]
        public IHttpActionResult PutFomSize(VMFomSize fomSize)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _fomSizeService.putFomSize(fomSize);
            return Ok(result);
        }

        // DELETE api/FomSize/1
        [ApiAuthorize(AccessLevel = "foamsize")]
        [HttpDelete]
        public IHttpActionResult DeleteFomSize(long id)
        {
            var result = _fomSizeService.deleteFomSize(id);
            return Ok(result);
        }
    }
}
