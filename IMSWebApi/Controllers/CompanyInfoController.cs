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
    public class CompanyInfoController : ApiController
    {
        private CompanyInfoService _companyInfoService = null;

        public CompanyInfoController()
        {
            _companyInfoService = new CompanyInfoService();
        }

        // Get api/CompanyInfo
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _companyInfoService.getCompanyInfo();
            return Ok(result);
        }

        // Get api/CompanyInfo
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _companyInfoService.getCompanyInfoById(id);
            return Ok(result);
        }

        //POST api/CompanyInfo
        [HttpPost]
        public IHttpActionResult PostMstCompanyInfo(VMCompanyInfo mstCompanyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _companyInfoService.postCompanyInfo(mstCompanyInfo);
            return Ok(result);
        }

        // PUT api/CompanyInfo
        [HttpPut]
        public IHttpActionResult PutMstCompanyInfo(VMCompanyInfo mstCompanyInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _companyInfoService.putCompanyInfo(mstCompanyInfo);
            return Ok(result);
        }
    }
}
