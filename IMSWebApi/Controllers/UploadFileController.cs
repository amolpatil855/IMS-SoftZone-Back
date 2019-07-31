using IMSWebApi.Enums;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
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
    public class UploadFileController : ApiController
    {
        private UploadFWRShadeService _uploadShade = null;
        private FWRDesignService _uploadDesign = null;
        private QualityService _qualityService = null;
        private MatThicknessService _matThicknessService = null;
        private MatSizeService _matSizeService = null;
        private AccessoryService _accessoryService = null;
        private CollectionService _collectionService = null;
        private PatternService _patternService = null;
        private FomDensityService _uploadFoamDensity = null;
        private FomSizeService _uploadFoamSize = null;
        private FomSuggestedMMService _uploadFoamSuggestedMM = null;
        private TailorService _uploadTailor = null;
 

        public UploadFileController()
        {
            _uploadShade = new UploadFWRShadeService();
            _uploadDesign = new FWRDesignService();
            _qualityService = new QualityService();
            _matThicknessService = new MatThicknessService();
            _matSizeService = new MatSizeService();
            _accessoryService = new AccessoryService();
            _collectionService = new CollectionService();
            _patternService = new PatternService();
            _uploadFoamDensity = new FomDensityService();
            _uploadFoamSize = new FomSizeService();
            _uploadFoamSuggestedMM = new FomSuggestedMMService();
            _uploadTailor = new TailorService();
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
            string filepath = string.Empty;

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);

            switch (TableName)
            {
                case "MstFWRShade":
                    filepath = _uploadShade.UploadShade(filebase);
                    break;
                case "MstFWRDesign":
                    filepath = _uploadDesign.UploadDesign(filebase);
                    break;
                case "FWRQualityFlatRate":
                    filepath = _qualityService.UploadFWRQualityFlatRate(filebase);
                    break;
                case "FWRQualityCutRoleRate":
                    filepath = _qualityService.UploadFWRQualityCutRoleRate(filebase);
                    break;
                case "MattressQuality":
                    filepath = _qualityService.UploadMattressQuality(filebase);
                    break;
                case "MattressThickness":
                    filepath = _matThicknessService.UploadMatThickness(filebase);
                    break;
                case "MattressSize":
                    filepath = _matSizeService.UploadMatSize(filebase);
                    break;
                case "Accessory":
                    filepath = _accessoryService.UploadAccessories(filebase);
                    break;
                case "Collection":
                    filepath = _collectionService.UploadCollections(filebase);
                    break;
                case "Pattern":
                    filepath = _patternService.UploadPatterns(filebase);
                    break;
                case "FoamQuality":
                    filepath = _qualityService.UploadFoamQuality(filebase);
                    break;
                case "MstFomDensity":
                    filepath = _uploadFoamDensity.UploadFoamDensity(filebase);
                    break;
                case "MstFomSize":
                    filepath = _uploadFoamSize.UploadFoamSize(filebase);
                    break;
                case "MstFomSuggestedMM":
                    filepath = _uploadFoamSuggestedMM.UploadFoamSuggestedMM(filebase);
                    break;
                case "Tailor":
                    filepath = _uploadTailor.UploadTailor(filebase);
                    break;
                case "PatternDetails":
                    filepath = _uploadTailor.UploadTailorPatternDetails(filebase);
                    break;
                default:
                    break;
            }
            //HttpResponseMessage HttpResponseMessage = GetFileDownloaded(filepath);
            //return Ok(result);
            return Ok(new ResponseMessage(1, filepath, ResponseType.Success));
        }

        [HttpGet]
        //content disposition of a invalidfile as Attachment for downloading
        public HttpResponseMessage GetFileDownloaded(string filepath)
        {
            string reqFile = filepath;
            //string bookName = "sample." + format.ToLower();
            //converting Pdf file into bytes array  
            var dataBytes = File.ReadAllBytes(reqFile);
            //adding bytes to memory stream   
            var dataStream = new MemoryStream(dataBytes);

            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = reqFile;
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return httpResponseMessage;
        }  
    }
}
