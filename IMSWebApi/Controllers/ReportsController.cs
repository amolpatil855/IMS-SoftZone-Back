using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    public class ReportsController : ApiController
    {
        private ReportsService _reportsService = null;

        public ReportsController()
        {
            _reportsService = new ReportsService();
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetFabricProducts")]
        public IHttpActionResult GetFabricProducts(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getFabricProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetRugProducts")]
        public IHttpActionResult GetRugProducts(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getRugProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetWallpaperProducts")]
        public IHttpActionResult GetWallpaperProducts(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getWallpaperProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetFoamProducts")]
        public IHttpActionResult GetFoamProducts(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getFoamProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetAccessoryProducts")]
        public IHttpActionResult GetAccessoryProducts(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getAccessoryProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetMattressProducts")]
        public IHttpActionResult GetMattressProducts(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getMattressProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetFabricProductsForExport")]
        public IHttpActionResult GetFabricProductsForExport()
        {
            var result = _reportsService.getFabricProductsForExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetRugProductsForExport")]
        public IHttpActionResult GetRugProductsForExport()
        {
            var result = _reportsService.getRugProductsForExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetWallpaperProductsForExport")]
        public IHttpActionResult GetWallpaperProductsForExport()
        {
            var result = _reportsService.getWallpaperProductsForExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetFoamProductsForExport")]
        public IHttpActionResult GetFoamProductsForExport()
        {
            var result = _reportsService.getFoamProductsForExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetAccessoryProductsForExport")]
        public IHttpActionResult GetAccessoryProductsForExport()
        {
            var result = _reportsService.getAccessoryProductsForExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "reports")]
        [HttpGet]
        [Route("api/Reports/GetMattressProductsForExport")]
        public IHttpActionResult GetMattressProductsForExport()
        {
            var result = _reportsService.getMattressProductsForExport();
            return Ok(result);
        }
    }
}
