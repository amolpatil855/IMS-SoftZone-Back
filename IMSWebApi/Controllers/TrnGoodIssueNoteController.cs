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
    public class TrnGoodIssueNoteController : ApiController
    {
        private TrnGoodIssueNoteService _trnGoodIssueNoteService = null;

        public TrnGoodIssueNoteController()
        {
            _trnGoodIssueNoteService = new TrnGoodIssueNoteService();
        }

        // GET api/TrnGoodIssueNote
        [ApiAuthorize(AccessLevel = "gin")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnGoodIssueNoteService.getGoodIssueNotes(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnGoodIssueNote/1
        [ApiAuthorize(AccessLevel = "gin")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnGoodIssueNoteService.getGoodIssueNoteById(id);
            return Ok(result);
        }

        // PUT api/TrnGoodIssueNote
        [ApiAuthorize(AccessLevel = "gin")]
        [HttpPut]
        public IHttpActionResult PutTrnGoodIssueNote(VMTrnGoodIssueNote goodIssueNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnGoodIssueNoteService.putGoodIssueNote(goodIssueNote);
            return Ok(result);
        }

        // GET api/TrnGoodIssueNote/1
        [ApiAuthorize(AccessLevel = "gin")]
        [HttpGet]
        [Route("api/TrnGoodIssueNote/GetGINsForItemsWithStockAvailable")]
        public IHttpActionResult GetGINsForItemsWithStockAvailable(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnGoodIssueNoteService.getGINsForItemsWithStockAvailable(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnGoodIssueNote/1
        [ApiAuthorize(AccessLevel = "gin")]
        [HttpGet]
        [Route("api/TrnGoodIssueNote/GetGINItemsWithAvailableInStockByginId")]
        public IHttpActionResult getGINItemsWithAvailableInStockByginId(long ginId)
        {
            var result = _trnGoodIssueNoteService.getGINItemsWithAvailableInStockByginId(ginId);
            return Ok(result);
        }
    }
}
