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
    public class TrnMaterialQuotationController : ApiController
    {
        private TrnMaterialQuotationService _trnMaterialQuotationService = null;

        public TrnMaterialQuotationController()
        {
            _trnMaterialQuotationService = new TrnMaterialQuotationService();
        }

        // GET api/TrnMaterialQuotation
        [ApiAuthorize(AccessLevel = "materialquotation")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnMaterialQuotationService.getMaterialQuotations(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnMaterialQuotation/1
        [ApiAuthorize(AccessLevel = "materialquotation")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnMaterialQuotationService.getMaterialQuotationById(id);
            return Ok(result);
        }

        // POST api/TrnMaterialQuotation
        [ApiAuthorize(AccessLevel = "materialquotation")]
        [HttpPost]
        public IHttpActionResult PostTrnMaterialQuotation(VMTrnMaterialQuotation materialQuotation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnMaterialQuotationService.postMaterialQuotation(materialQuotation);
            return Ok(result);
        }

        // PUT api/TrnMaterialQuotation
        [ApiAuthorize(AccessLevel = "materialquotation")]
        [HttpPut]
        public IHttpActionResult PutTrnMaterialQuotation(VMTrnMaterialQuotation materialQuotation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnMaterialQuotationService.putMaterialQuotation(materialQuotation);
            return Ok(result);
        }

        // PUT api/TrnMaterialQuotation
        [ApiAuthorize(Roles = "Administrator")]
        [Route("api/TrnMaterialQuotation/ApproveMaterialQuotation/{id}")]
        [HttpPut]
        public IHttpActionResult ApproveMaterialQuotation(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnMaterialQuotationService.approveMaterialQuotation(id);
            return Ok(result);
        }

        // PUT api/TrnMaterialQuotation
        [ApiAuthorize(AccessLevel = "materialquotation")]
        [Route("api/TrnMaterialQuotation/CancelMaterialQuotation/{id}")]
        [HttpPut]
        public IHttpActionResult CancelMaterialQuotation(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnMaterialQuotationService.cancelMaterialQuotation(id);
            return Ok(result);
        }
    }
}
