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
        private QualityService _qualityService = null;
        private MatThicknessService _matThicknessService = null;
        private MatSizeService _matSizeService = null;
        private AccessoryService _accessoryService = null;
        private CollectionService _collectionService = null;
        private PatternService _patternService = null;

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
                case "FWRQualityFlatRate":
                    result = _qualityService.UploadFWRQualityFlatRate(filebase);
                    break;
                case "FWRQualityCutRoleRate":
                    result = _qualityService.UploadFWRQualityCutRoleRate(filebase);
                    break;
                case "MattressQuality":
                    result = _qualityService.UploadMattressQuality(filebase);
                    break;
                case "MattressThickness":
                    result = _matThicknessService.UploadMatThickness(filebase);
                    break;
                case "MattressSize":
                    result = _matSizeService.UploadMatSize(filebase);
                    break;
                case "Accessory":
                    result = _accessoryService.UploadAccessories(filebase);
                    break;
                case "Collection":
                    result = _collectionService.UploadCollections(filebase);
                    break;
                case "Pattern":
                    result = _patternService.UploadPatterns(filebase);
                    break;
                case "FoamQuality":
                    result = _qualityService.UploadFoamQuality(filebase);
                    break;
                default:
                    break;
            }            
            return Ok(result);
        }
    }
}
