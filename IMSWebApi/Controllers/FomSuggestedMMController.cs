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
    public class FomSuggestedMMController : ApiController
    {
        private FomSuggestedMMService _fomSuggestedMMService = null;

        public FomSuggestedMMController()
        {
            _fomSuggestedMMService = new FomSuggestedMMService();
        }

        // GET api/FomSuggestedMM   
        [ApiAuthorize(AccessLevel = "foamsuggestedmm")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _fomSuggestedMMService.getFomSuggestedMMs(pageSize, page, search);
            return Ok(result);
        }

        // GET api/FomSuggestedMM/1
        [ApiAuthorize(AccessLevel = "foamsuggestedmm")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _fomSuggestedMMService.getFomSuggestedMMById(id);
            return Ok(result);
        }

        // POST api/FomSuggestedMM
        [ApiAuthorize(AccessLevel = "foamsuggestedmm")]
        [HttpPost]
        public IHttpActionResult PostFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _fomSuggestedMMService.postFomSuggestedMM(fomSuggestedMM);
            return Ok(result);
        }

        // PUT api/FomSuggestedMM
        [ApiAuthorize(AccessLevel = "foamsuggestedmm")]
        [HttpPut]
        public IHttpActionResult PutFomSuggestedMM(VMFomSuggestedMM fomSuggestedMM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _fomSuggestedMMService.putFomSuggestedMM(fomSuggestedMM);
            return Ok(result);
        }

        // DELETE api/FomSuggestedMM/1
        [ApiAuthorize(AccessLevel = "foamsuggestedmm")]
        [HttpDelete]
        public IHttpActionResult DeleteFomSuggestedMM(long id)
        {
            var result = _fomSuggestedMMService.deleteFomSuggestedMM(id);
            return Ok(result);
        }
    }
}
