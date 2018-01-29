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
    public class TrnProductStockController : ApiController
    {
        private TrnProductStockService _trnProductStockService = null;

        public TrnProductStockController()
        {
            _trnProductStockService = new TrnProductStockService();
        }

        // GET api/TrnProductStock
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getTrnProductStock(pageSize, page, search);
            return Ok(result);
        }




    }
}
