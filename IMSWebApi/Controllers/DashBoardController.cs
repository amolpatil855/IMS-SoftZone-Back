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
    }
}
