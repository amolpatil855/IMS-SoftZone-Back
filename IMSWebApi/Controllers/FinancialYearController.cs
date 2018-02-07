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
    public class FinancialYearController : ApiController
    {
        private FinancialYearService _financialYearService = null;

        public FinancialYearController()
        {
            _financialYearService = new FinancialYearService();
        }

        // GET api/FinancialYear
        //[ApiAuthorize(AccessLevel = "collection")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _financialYearService.getFinancialYear(pageSize, page, search);
            return Ok(result);
        }

        // GET api/FinancialYear/1
        //[ApiAuthorize(AccessLevel = "collection")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _financialYearService.getFinancialYearById(id);
            return Ok(result);
        }

        // POST api/FinancialYear
        //[ApiAuthorize(AccessLevel = "collection")]
        [HttpPost]
        public IHttpActionResult PostFinancialYear(VMFinancialYear financialYear)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _financialYearService.postFinancial(financialYear);
            return Ok(result);
        }

        // PUT api/FinancialYear
        //[ApiAuthorize(AccessLevel = "collection")]
        [HttpPut]
        public IHttpActionResult PutFinancialYear(VMFinancialYear financialYear)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _financialYearService.putFinancialYear(financialYear);
            return Ok(result);
        }

        // DELETE api/FinancialYear/1
        //[ApiAuthorize(AccessLevel = "collection")]
        [HttpDelete]
        public IHttpActionResult DeleteFinancialYear(long id)
        {
            var result = _financialYearService.deleteFinancialYear(id);
            return Ok(result);
        }
    }
}
