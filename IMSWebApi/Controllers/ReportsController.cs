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
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetFabricProductsForML")]
        public IHttpActionResult GetFabricProductsForML(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getFabricProductsForML(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetRugProductsForML")]
        public IHttpActionResult GetRugProductsForML(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getRugProductsForML(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetWallpaperProductsForML")]
        public IHttpActionResult GetWallpaperProductsForML(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getWallpaperProductsForML(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetFoamProductsForML")]
        public IHttpActionResult GetFoamProductsForML(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getFoamProductsForML(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetAccessoryProductsForML")]
        public IHttpActionResult GetAccessoryProductsForML(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getAccessoryProductsForML(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetMattressProductsForML")]
        public IHttpActionResult GetMattressProductsForML(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getMattressProductsForML(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetFabricProductsForMLExport")]
        public IHttpActionResult GetFabricProductsForMLExport()
        {
            var result = _reportsService.getFabricProductsForMLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetRugProductsForMLExport")]
        public IHttpActionResult GetRugProductsForMLExport()
        {
            var result = _reportsService.getRugProductsForMLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetWallpaperProductsForMLExport")]
        public IHttpActionResult GetWallpaperProductsForMLExport()
        {
            var result = _reportsService.getWallpaperProductsForMLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetFoamProductsForMLExport")]
        public IHttpActionResult GetFoamProductsForMLExport()
        {
            var result = _reportsService.getFoamProductsForMLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetAccessoryProductsForMLExport")]
        public IHttpActionResult GetAccessoryProductsForMLExport()
        {
            var result = _reportsService.getAccessoryProductsForMLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [ApiAuthorize(AccessLevel = "masterpricelist")]
        [HttpGet]
        [Route("api/Reports/GetMattressProductsForMLExport")]
        public IHttpActionResult GetMattressProductsForMLExport()
        {
            var result = _reportsService.getMattressProductsForMLExport();
            return Ok(result);
        }

        // Client List

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetFabricProductsForCL")]
        public IHttpActionResult GetFabricProductsForCL(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getFabricProductsForCL(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetRugProductsForCL")]
        public IHttpActionResult GetRugProductsForCL(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getRugProductsForCL(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetWallpaperProductsForCL")]
        public IHttpActionResult GetWallpaperProductsForCL(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getWallpaperProductsForCL(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetFoamProductsForCL")]
        public IHttpActionResult GetFoamProductsForCL(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getFoamProductsForCL(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetAccessoryProductsForCL")]
        public IHttpActionResult GetAccessoryProductsForCL(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getAccessoryProductsForCL(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetMattressProductsForCL")]
        public IHttpActionResult GetMattressProductsForCL(int pageSize = 0, int page = 0)
        {
            var result = _reportsService.getMattressProductsForCL(pageSize, page);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetFabricProductsForCLExport")]
        public IHttpActionResult GetFabricProductsForCLExport()
        {
            var result = _reportsService.getFabricProductsForCLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetRugProductsForCLExport")]
        public IHttpActionResult GetRugProductsForCLExport()
        {
            var result = _reportsService.getRugProductsForCLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetWallpaperProductsForCLExport")]
        public IHttpActionResult GetWallpaperProductsForCLExport()
        {
            var result = _reportsService.getWallpaperProductsForCLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetFoamProductsForCLExport")]
        public IHttpActionResult GetFoamProductsForCLExport()
        {
            var result = _reportsService.getFoamProductsForCLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetAccessoryProductsForCLExport")]
        public IHttpActionResult GetAccessoryProductsForCLExport()
        {
            var result = _reportsService.getAccessoryProductsForCLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "clientpricelist")]
        [Route("api/Reports/GetMattressProductsForCLExport")]
        public IHttpActionResult GetMattressProductsForCLExport()
        {
            var result = _reportsService.getMattressProductsForCLExport();
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "purchaseorderstatuslist")]
        [Route("api/Reports/GetPOorderStatusReport")]
        public IHttpActionResult GetPOorderStatusReport(int pageSize, int page, string status)
        {
            var result = _reportsService.getPOorderStatusReport(pageSize,page,status);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "salesorderstatuslist")]
        [Route("api/Reports/GetSOorderStatusReport")]
        public IHttpActionResult GetSOorderStatusReport(int pageSize, int page, string status)
        {
            var result = _reportsService.getSOorderStatusReport(pageSize, page, status);
            return Ok(result);
        }

        // GET api/Reports   
        [HttpGet]
        [ApiAuthorize(AccessLevel = "invoicestatuslist")]
        [Route("api/Reports/GetSalesInvoicePaymentStatusReport")]
        public IHttpActionResult GetSalesInvoicePaymentStatusReport(int pageSize, int page, string status, string isPaid)
        {
            var result = _reportsService.getSalesInvoicePaymentStatusReport(pageSize, page, status, isPaid);
            return Ok(result);
        }
    }
}
