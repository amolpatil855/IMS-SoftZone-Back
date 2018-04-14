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
    [Authorize]
    public class TrnWorkOrderController : ApiController
    {
        private TrnWorkOrderService _trnWorkOrderService = null;

        public TrnWorkOrderController()
        {
            _trnWorkOrderService = new TrnWorkOrderService();
        }

        // GET api/TrnWorkOrder
        [ApiAuthorize(AccessLevel = "workorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnWorkOrderService.getWorkOrders(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnWorkOrder/1
        [ApiAuthorize(AccessLevel = "workorder")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnWorkOrderService.getWorkOrderById(id);
            return Ok(result);
        }
    }
}
