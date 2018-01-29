using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
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
        [ApiAuthorize(AccessLevel = "stockmanagement")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockService.getTrnProductStock(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStock/1
        [ApiAuthorize(AccessLevel = "stockmanagement")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnProductStockService.getTrnProductStockById(id);
            return Ok(result);
        }

        // POST api/TrnProductStock
        [ApiAuthorize(AccessLevel = "stockmanagement")]
        [HttpPost]
        public IHttpActionResult PostTrnProductStock(VMTrnProductStock trnProductStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnProductStockService.postTrnProductStock(trnProductStock);
            return Ok(result);
        }

        // PUT api/TrnProductStock/1
        [ApiAuthorize(AccessLevel = "stockmanagement")]
        [HttpPut]
        public IHttpActionResult PutTrnProductStock(VMTrnProductStock trnProductStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnProductStockService.putTrnProductStock(trnProductStock);
            return Ok(result);
        }
    }
}
