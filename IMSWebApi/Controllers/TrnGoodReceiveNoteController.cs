using IMSWebApi.CustomAttributes;
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
    public class TrnGoodReceiveNoteController : ApiController
    {


        public TrnGoodReceiveNoteController()
        {

        }

        // GET api/TrnGoodReceiveNote
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            return Ok();
        }

        // GET api/TrnGoodReceiveNote/1
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            return Ok();
        }

        // POST api/TrnGoodReceiveNote
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpPost]
        public IHttpActionResult PostTrnGoodReceiveNote(VMTrnGoodReceiveNote goodReceiveNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // PUT api/TrnGoodReceiveNote
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpPut]
        public IHttpActionResult PutTrnGoodReceiveNote(VMTrnGoodReceiveNote goodReceiveNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // DELETE api/TrnGoodReceiveNote/1
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpDelete]
        public IHttpActionResult DeleteTrnGoodReceiveNote(long id)
        {
            return Ok();
        }
    }
}
