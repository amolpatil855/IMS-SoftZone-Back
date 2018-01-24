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
    public class MatThicknessController : ApiController
    {
        private MatThicknessService _matThicknessService = null;

        public MatThicknessController()
        {
            _matThicknessService = new MatThicknessService();
        }

        // GET api/MatThickness   
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _matThicknessService.getMatThickness(pageSize, page, search);
            return Ok(result);
        }

        // GET api/MatThickness/1
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _matThicknessService.getMatThicknessById(id);
            return Ok(result);
        }
        
        // POST api/MatThickness
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpPost]
        public IHttpActionResult PostMatThickness(VMMatThickness matThickness)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _matThicknessService.postMatThickness(matThickness);
            return Ok(result);
        }

        // PUT api/MatThickness/1
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpPut]
        public IHttpActionResult PutMatThickness(VMMatThickness matThickness)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _matThicknessService.putMatThickness(matThickness);
            return Ok(result);
        }

        // DELETE api/MatThickness/1
        [ApiAuthorize(AccessLevel = "mattressthickness")]
        [HttpDelete]
        public IHttpActionResult DeleteMattressThickness(long id)
        {
            var result = _matThicknessService.deleteMatThickness(id);
            return Ok(result);
        }
    }
}
