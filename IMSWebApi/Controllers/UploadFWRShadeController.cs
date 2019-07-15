using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    public class UploadFWRShadeController : ApiController
    {
        private UploadFWRShadeService _uploadShade = null;

        public UploadFWRShadeController()
        {
            _uploadShade = new UploadFWRShadeService();
        }

        //[ApiAuthorize(AccessLevel = "shade")]
        // POST api/UploadFWRShade
        [HttpPost]
        public IHttpActionResult UploadFile(string MstFWRShade)
        {
            HttpResponseMessage fileresult = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files[0] == null || httpRequest.Files[0].ContentLength == 0)
            {
                return BadRequest("Invalid File");
            }

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);

            var result = _uploadShade.UploadShade(filebase);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UploadCategory(string Category)
        {
            HttpResponseMessage fileresult = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files[0] == null || httpRequest.Files[0].ContentLength == 0)
            {
                return BadRequest("Invalid File");
            }

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);

            var result = _uploadShade.uploadCategory(filebase);
            return Ok(result);
        }
    }
}
