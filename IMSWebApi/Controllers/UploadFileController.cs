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
    public class UploadFileController : ApiController
    {
        private UploadFWRShadeService _uploadShade = null;
        private FWRDesignService _uploadDesign = null;

        public UploadFileController()
        {
            _uploadShade = new UploadFWRShadeService();
            _uploadDesign = new FWRDesignService();
        }

        //[ApiAuthorize(AccessLevel = "shade")]
        // POST api/UploadFile
        [HttpPost]
        public IHttpActionResult UploadFile(string TableName)
        {
            var httpRequest = HttpContext.Current.Request;
            var result = 0;
            if (httpRequest.Files[0] == null || httpRequest.Files[0].ContentLength == 0)
            {
                return BadRequest("Invalid File");
            }

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);

            switch (TableName)
            {
                case "MstFWRShade":
                    result = _uploadShade.UploadShade(filebase);
                    break;
                case "MstFWRDesign":
                    result = _uploadDesign.UploadDesign(filebase);
                    break;
                case "MstQuality":
                    result = 0;
                    break;
                default:
                    break;
            }            
            return Ok(result);
        }
    }
}
