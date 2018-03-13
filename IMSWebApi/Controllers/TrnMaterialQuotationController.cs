using IMSWebApi.Services;
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
        //[ApiAuthorize(AccessLevel = "materialquotation")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnMaterialQuotationService.getMaterialQuotations(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnMaterialQuotation/1
        //[ApiAuthorize(AccessLevel = "materialquotation")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnMaterialQuotationService.getMaterialQuotationById(id);
            return Ok(result);
        }
    }
}
