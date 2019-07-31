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
            //var result = 0;
            if (httpRequest.Files[0] == null || httpRequest.Files[0].ContentLength == 0)
            {
                return BadRequest("Invalid File");
            }
            //string filepath = string.Empty;

            var data = Tuple.Create("", 0);

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);

            switch (TableName)
            {
                case "MstFWRShade":
                    data = _uploadShade.UploadShade(filebase);
                    break;
                case "MstFWRDesign":
                    data = _uploadDesign.UploadDesign(filebase);
                    break;
                case "FWRQualityFlatRate":
                    data = _qualityService.UploadFWRQualityFlatRate(filebase);
                    break;
                case "FWRQualityCutRoleRate":
                    data = _qualityService.UploadFWRQualityCutRoleRate(filebase);
                    break;
                case "MattressQuality":
                    data = _qualityService.UploadMattressQuality(filebase);
                    break;
                case "MattressThickness":
                    data = _matThicknessService.UploadMatThickness(filebase);
                    break;
                case "MattressSize":
                    data = _matSizeService.UploadMatSize(filebase);
                    break;
                case "Accessory":
                    data = _accessoryService.UploadAccessories(filebase);
                    break;
                case "Collection":
                    data = _collectionService.UploadCollections(filebase);
                    break;
                case "Pattern":
                    data = _patternService.UploadPatterns(filebase);
                    break;
                case "FoamQuality":
                    data = _qualityService.UploadFoamQuality(filebase);
                    break;
                case "MstFomDensity":
                    data = _uploadFoamDensity.UploadFoamDensity(filebase);
                    break;
                case "MstFomSize":
                    data = _uploadFoamSize.UploadFoamSize(filebase);
                    break;
                case "MstFomSuggestedMM":
                    data = _uploadFoamSuggestedMM.UploadFoamSuggestedMM(filebase);
                    break;
                case "Tailor":
                    data = _uploadTailor.UploadTailor(filebase);
                    break;
                case "PatternDetails":
                    data = _uploadTailor.UploadTailorPatternDetails(filebase);
                    break;
                default:
                    break;
            }
            //HttpResponseMessage HttpResponseMessage = GetFileDownloaded(filepath);
            //return Ok(result);
            return Ok(new ResponseMessage(data.Item2, data.Item1, ResponseType.Success));
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
