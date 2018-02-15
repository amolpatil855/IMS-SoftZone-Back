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
    public class TrnProductStockDetailController : ApiController
    {
        private TrnProductStockDetailService _trnProductStockDetailService = null;

        public TrnProductStockDetailController()
        {
            _trnProductStockDetailService = new TrnProductStockDetailService();
        }

        // GET api/TrnProductStockDetail
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnProductStockDetailService.getTrnProductStockDetail(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnProductStockDetail/1
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnProductStockDetailService.getTrnProductStockDetailById(id);
            return Ok(result);
        }

        // POST api/TrnProductStockDetail
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpPost]
        public IHttpActionResult PostTrnProductStock(VMTrnProductStockDetail trnProductStockDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnProductStockDetailService.postTrnProductStockDetail(trnProductStockDetail);
            return Ok(result);
        }

        // PUT api/TrnProductStockDetail/1
        [ApiAuthorize(AccessLevel = "productstock")]
        [HttpPut]
        public IHttpActionResult PutTrnProductStock(VMTrnProductStockDetail trnProductStockDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnProductStockDetailService.putTrnProductStockDetail(trnProductStockDetail);
            return Ok(result);
        }
    }
}
