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
    public class QualityController : ApiController
    {
        private QualityService _qualityService = null;
        
        public QualityController()
        {
            _qualityService = new QualityService();
        }

        // GET api/Quality   
        [ApiAuthorize(AccessLevel = "quality")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _qualityService.getQualities(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Quality/1
        [ApiAuthorize(AccessLevel = "quality")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _qualityService.getQualityById(id);
            return Ok(result);
        }

        // POST api/Quality
        [ApiAuthorize(AccessLevel = "quality")]
        [HttpPost]
        public IHttpActionResult PostQuality(VMQuality quality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _qualityService.postQuality(quality);
            return Ok(result);
        }

        // PUT api/Quality/1
        [ApiAuthorize(AccessLevel = "quality")]
        [HttpPut]
        public IHttpActionResult PutQuality(VMQuality quality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _qualityService.putQuality(quality);
            return Ok(result);
        }

        // DELETE api/Quality/1
        [ApiAuthorize(AccessLevel = "quality")]
        [HttpDelete]
        public IHttpActionResult DeleteQuality(long id)
        {
            var result = _qualityService.deleteQuality(id);
            return Ok(result);
        }
    }
}
