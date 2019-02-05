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
    public class TrnGoodReceiveNoteController : ApiController
    {
        private TrnGoodReceiveNoteService _trnGoodReceiveNoteService = null;

        public TrnGoodReceiveNoteController()
        {
            _trnGoodReceiveNoteService = new TrnGoodReceiveNoteService();
        }

        // GET api/TrnGoodReceiveNote
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnGoodReceiveNoteService.getGoodReceiveNote(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnGoodReceiveNote/1
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnGoodReceiveNoteService.getGoodReceiveNoteById(id);
            return Ok(result);
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
            var result = _trnGoodReceiveNoteService.postGRN(goodReceiveNote);
            return Ok(result);
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

  
        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        [Route("api/TrnGoodReceiveNote/GetPOListForSelectedItem")]
        public IHttpActionResult GetPOListForSelectedItem(long categoryId, long? collectionId, long? parameterId, string matSizeCode , long? matQualityId, long? matThicknessId)
        {
            var result = _trnGoodReceiveNoteService.getPOListForSelectedItem(categoryId, collectionId, parameterId, matSizeCode, matQualityId, matThicknessId);
            return Ok(result);
        }

        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        [Route("api/TrnGoodReceiveNote/GetCustomMatSizeCodeLookup")]
        public IHttpActionResult GetCustomMatSizeCodeLookup(long categoryId, long collectionId, long matQualityId, long matThicknessId)
        {
            var result = _trnGoodReceiveNoteService.getCustomMatSizeCodeLookup(categoryId, collectionId, matQualityId, matThicknessId);
            return Ok(result);
        }

        [ApiAuthorize(AccessLevel = "grn")]
        [HttpGet]
        [Route("api/TrnGoodReceiveNote/GetPendingPO")]
        public IHttpActionResult GetPendingPO()
        {
            var result = _trnGoodReceiveNoteService.getPendingPO();
            return Ok(result);
        }
    }
}
