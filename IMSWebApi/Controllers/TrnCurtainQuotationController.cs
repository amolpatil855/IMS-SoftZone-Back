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
    public class TrnCurtainQuotationController : ApiController
    {
        private TrnCurtainQuotationService _trnCurtainQuotationService = null;

        public TrnCurtainQuotationController()
        {
            _trnCurtainQuotationService = new TrnCurtainQuotationService();
        }

        // GET api/TrnCurtainQuotation
        [ApiAuthorize(AccessLevel = "curtainquotation")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnCurtainQuotationService.getCurtainQuotations(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnCurtainQuotation/1
        [ApiAuthorize(AccessLevel = "curtainquotation")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnCurtainQuotationService.getCurtainQuotationById(id);
            return Ok(result);
        }

        // POST api/TrnCurtainQuotation
        [ApiAuthorize(AccessLevel = "curtainquotation")]
        [HttpPost]
        public IHttpActionResult PostTrnMaterialQuotation(VMTrnCurtainQuotation curtainQuotation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnCurtainQuotationService.postCurtainQuotation(curtainQuotation);
            return Ok(result);
        }

        // PUT api/TrnCurtainQuotation
        [ApiAuthorize(AccessLevel = "curtainquotation")]
        [HttpPut]
        public IHttpActionResult PutTrnMaterialQuotation(VMTrnCurtainQuotation curtainQuotation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnCurtainQuotationService.putCurtainQuotation(curtainQuotation);
            return Ok(result);
        }

        // PUT api/TrnCurtainQuotation
        [ApiAuthorize(Roles = "Administrator")]
        [Route("api/TrnCurtainQuotation/ApproveCurtainQuotation/{id}")]
        [HttpPut]
        public IHttpActionResult ApproveCurtainQuotation(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnCurtainQuotationService.approveCurtainQuotation(id);
            return Ok(result);
        }
    }
}
