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
    public class DesignController : ApiController
    {
        private DesignService _designService = null;

        public DesignController()
        {
            _designService = new DesignService();
        }

        // GET api/Design   
        //[Authorize]
        //[ApiAuthorize(AccessLevel = "design")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _designService.getDesign(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Design/1
        //[Authorize]
        //[ApiAuthorize(AccessLevel = "design")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _designService.getDesignById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Design/GetDesignLookupByQuality")]
        public IHttpActionResult GetDesignLookupByQuality(long id)
        {
            var result = _designService.getDesignLookUpByQuality(id);
            return Ok(result);
        }


        // POST api/Design
        //[Authorize]
        //[ApiAuthorize(AccessLevel = "design")]
        [HttpPost]
        public IHttpActionResult PostDesign(VMDesign design)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _designService.postDesign(design);
            return Ok(result);
        }

        // PUT api/Design/1
        //[Authorize]
        //[ApiAuthorize(AccessLevel = "design")]
        [HttpPut]
        public IHttpActionResult PutDesign(VMDesign design)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _designService.putDesign(design);
            return Ok(result);
        }

        // DELETE api/Design/1
        //[Authorize]
        //[ApiAuthorize(AccessLevel = "design")]
        [HttpDelete]
        public IHttpActionResult DeleteDesign(long id)
        {
            var result = _designService.deleteDesign(id);
            return Ok(result);
        }
    }
}
