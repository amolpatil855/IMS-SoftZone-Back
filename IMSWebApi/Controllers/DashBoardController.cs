using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class DashboardController : ApiController
    {
        private DashboardService _dashboardService = null;

        public DashboardController()
        {
            _dashboardService = new DashboardService();
        }

        // GET api/Dashboard
        //[ApiAuthorize(AccessLevel = "collection")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _dashboardService.getDashboardData();
            return Ok(result);
        }

        // GET api/Dashboard
        [HttpGet]
        [Route("api/Dashboard/GetRecordsForTotalOutstandingAmt")]
        public IHttpActionResult GetRecordsForTotalOutstandingAmt(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getRecordsForTotalOutstandingAmt(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Dashboard
        [HttpGet]
        [Route("api/Dashboard/GetRecordsForSOCount")]
        public IHttpActionResult GetRecordsForSOCount(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getRecordsForSOCount(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Dashboard
        [HttpGet]
        [Route("api/Dashboard/GetRecordsForPOCount")]
        public IHttpActionResult GetRecordsForPOCount(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getRecorsForPOCount(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Dashboard
        [HttpGet]
        [Route("api/Dashboard/GetFabricsItemsBelowReOrderLevel")]
        public IHttpActionResult GetFabricsItemsBelowReOrderLevel(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getFabricsItemsBelowReOrderLevel(pageSize, page, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Dashboard/GetFoamItemsBelowReOrderLevel")]
        public IHttpActionResult GetFoamItemsBelowReOrderLevel(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getFoamItemsBelowReOrderLevel(pageSize, page, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Dashboard/GetMattressItemsBelowReOrderLevel")]
        public IHttpActionResult GetMattressItemsBelowReOrderLevel(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getMattressItemsBelowReOrderLevel(pageSize, page, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Dashboard/GetRugItemsBelowReOrderLevel")]
        public IHttpActionResult GetRugItemsBelowReOrderLevel(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getRugItemsBelowReOrderLevel(pageSize, page, search);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Dashboard/GetWallpaperItemsBelowReOrderLevel")]
        public IHttpActionResult GetWallpaperItemsBelowReOrderLevel(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _dashboardService.getWallpaperItemsBelowReOrderLevel(pageSize, page, search);
            return Ok(result);
        }

        #region Not needed now
        //// GET api/Dashboard
        //[HttpGet]
        //[Route("api/Dashboard/GetRecordsForYTDSale")]
        //public IHttpActionResult GetRecordsForYTDSale(int pageSize = 0, int page = 0, string search = null)
        //{
        //    var result = _dashboardService.getRecordsForYTDSale(pageSize, page, search);
        //    return Ok(result);
        //}

        //// GET api/Dashboard
        //[HttpGet]
        //[Route("api/Dashboard/GetRecordsForCurrentMonthSale")]
        //public IHttpActionResult GetRecordsForCurrentMonthSale(int pageSize = 0, int page = 0, string search = null)
        //{
        //    var result = _dashboardService.getRecordsForCurrentMonthSale(pageSize, page, search);
        //    return Ok(result);
        //} 
        #endregion
    }
}
