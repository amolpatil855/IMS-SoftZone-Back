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
    public class TrnCurtainSelectionController : ApiController
    {
        private TrnCurtainSelectionService _trnCurtainSelectionService = null;

        public TrnCurtainSelectionController()
        {
            _trnCurtainSelectionService = new TrnCurtainSelectionService();
        }

        // GET api/TrnCurtainSelection
        [ApiAuthorize(AccessLevel = "curtainselection")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnCurtainSelectionService.getCurtainSelections(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnCurtainSelection/1
        [ApiAuthorize(AccessLevel = "curtainselection")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnCurtainSelectionService.getCurtainSelectionById(id);
            return Ok(result);
        }

        // POST api/TrnCurtainSelection
        [ApiAuthorize(AccessLevel = "curtainselection")]
        [HttpPost]
        public IHttpActionResult PostTrnCurtainSelection(VMTrnCurtainSelection curtainselection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnCurtainSelectionService.postCurtainSelection(curtainselection);
            return Ok(result);
        }

        // PUT api/TrnCurtainSelection
        [ApiAuthorize(AccessLevel = "curtainselection")]
        [HttpPut]
        public IHttpActionResult PutTrnCurtainSelection(VMTrnCurtainSelection curtainSelection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnCurtainSelectionService.putCurtainSelection(curtainSelection);
            return Ok(result);
        }
    }
}
