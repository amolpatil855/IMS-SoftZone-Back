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
    public class TrnMaterialSelectionController : ApiController
    {
        private TrnMaterialSelectionService _trnMaterialSelectionService = null;

        public TrnMaterialSelectionController()
        {
            _trnMaterialSelectionService = new TrnMaterialSelectionService();
        }

        // GET api/TrnMaterialSelection
        //[ApiAuthorize(AccessLevel = "materialselection")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnMaterialSelectionService.getMaterialSelections(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnMaterialSelection/1
        //[ApiAuthorize(AccessLevel = "materialselection")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnMaterialSelectionService.getMaterialSelectionById(id);
            return Ok(result);
        }

        // POST api/TrnMaterialSelection
        //[ApiAuthorize(AccessLevel = "materialselection")]
        [HttpPost]
        public IHttpActionResult PostTrnMaterialSelection(VMTrnMaterialSelection materialSelection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnMaterialSelectionService.postMaterialSelection(materialSelection);
            return Ok(result);
        }

        // PUT api/TrnMaterialSelection
        //[ApiAuthorize(AccessLevel = "materialselection")]
        [HttpPut]
        public IHttpActionResult PutTrnMaterialSelection(VMTrnMaterialSelection materialSelection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnMaterialSelectionService.putMaterialSelection(materialSelection);
            return Ok(result);
        }

        // GET api/TrnMaterialSelection/1
        //[ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/TrnMaterialSelection/CreateMaterialQuotation/{id}")]
        public IHttpActionResult CreateMaterialQuotation(long id)
        {
            var result = _trnMaterialSelectionService.createMaterialQuotation(id);
            return Ok(result);
        }
    }
}
