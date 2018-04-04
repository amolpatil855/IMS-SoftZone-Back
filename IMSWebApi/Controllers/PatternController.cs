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
    public class PatternController : ApiController
    {
        private PatternService _patternService = null;

        public PatternController()
        {
            _patternService = new PatternService();
        }

        // GET api/Pattern   
        [ApiAuthorize(AccessLevel = "pattern")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _patternService.getPatterns(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Pattern/1
        [ApiAuthorize(AccessLevel = "pattern")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _patternService.getPatternById(id);
            return Ok(result);
        }

        // POST api/Pattern
        [ApiAuthorize(AccessLevel = "pattern")]
        [HttpPost]
        public IHttpActionResult PostPattern(VMPattern pattern)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _patternService.postPattern(pattern);
            return Ok(result);
        }

        // PUT api/Pattern
        [ApiAuthorize(AccessLevel = "pattern")]
        [HttpPut]
        public IHttpActionResult PutPattern(VMPattern pattern)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _patternService.putPattern(pattern);
            return Ok(result);
        }

        // DELETE api/Pattern/1
        [ApiAuthorize(AccessLevel = "pattern")]
        [HttpDelete]
        public IHttpActionResult DeletePattern(long id)
        {
            var result = _patternService.deletePattern(id);
            return Ok(result);
        }
    }
}
