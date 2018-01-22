using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class CompanyInfoController : ApiController
    {
        private CompanyInfoService _companyInfoService = null;

        public CompanyInfoController()
        {
            _companyInfoService = new CompanyInfoService();
        }

        // Get api/CompanyInfo
        [ApiAuthorize(AccessLevel = "companyinfo")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _companyInfoService.getCompanyInfo();
            return Ok(result);
        }

        // Get api/CompanyInfo
        [ApiAuthorize(AccessLevel = "companyinfo")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _companyInfoService.getCompanyInfoById(id);
            return Ok(result);
        }

        //POST api/CompanyInfo
        [ApiAuthorize(AccessLevel = "companyinfo")]
        [HttpPost]
        public IHttpActionResult PostMstCompanyInfo( )
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;
            var jsonData = httpRequest.Form["mstCompanyInfo"];
            VMCompanyInfo mstCompanyInfo = JsonConvert.DeserializeObject<VMCompanyInfo>(jsonData);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/UploadFile/" + postedFile.FileName);
                    mstCompanyInfo.companyLogo = "UploadFile/"+postedFile.FileName;
                    postedFile.SaveAs(filePath);
                }
            }
            
            var result = _companyInfoService.postCompanyInfo(mstCompanyInfo);
            return Ok(result);
        }

        // PUT api/CompanyInfo
        [ApiAuthorize(AccessLevel = "companyinfo")]
        [HttpPut]
        public IHttpActionResult PutMstCompanyInfo()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;
            var jsonData = httpRequest.Form["mstCompanyInfo"];
            VMCompanyInfo mstCompanyInfo = JsonConvert.DeserializeObject<VMCompanyInfo>(jsonData);
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/UploadFile/" + postedFile.FileName);
                    mstCompanyInfo.companyLogo = "UploadFile/" + postedFile.FileName;
                    postedFile.SaveAs(filePath);
                }
            }
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _companyInfoService.putCompanyInfo(mstCompanyInfo);
            return Ok(result);
        }
    }
}
