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
    public class FWRDesignController : ApiController
    {
        private FWRDesignService _designService = null;
        
        public FWRDesignController()
        {
            _designService = new FWRDesignService();
        }

        // GET api/FWRDesign   
        [ApiAuthorize(AccessLevel = "design")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _designService.getDesigns(pageSize, page, search);
            return Ok(result);
        }

        // GET api/FWRDesign/1
        [ApiAuthorize(AccessLevel = "design")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _designService.getDesignById(id);
            return Ok(result);
        }

        // POST api/FWRDesign
        [ApiAuthorize(AccessLevel = "design")]
        [HttpPost]
        public IHttpActionResult PostDesign(VMFWRDesign design)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _designService.postDesign(design);
            return Ok(result);
        }

        // PUT api/FWRDesign/1
        [ApiAuthorize(AccessLevel = "design")]
        [HttpPut]
        public IHttpActionResult PutDesign(VMFWRDesign design)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _designService.putDesign(design);
            return Ok(result);
        }

        // DELETE api/FWRDesign/1
        [ApiAuthorize(AccessLevel = "design")]
        [HttpDelete]
        public IHttpActionResult DeleteDesign(long id)
        {
            var result = _designService.deleteDesign(id);
            return Ok(result);
        }
    }
}
