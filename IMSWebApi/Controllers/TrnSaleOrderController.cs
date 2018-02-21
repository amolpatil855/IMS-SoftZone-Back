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
    public class TrnSaleOrderController : ApiController
    {
        private TrnSaleOrderService _trnSaleOrderService = null;

        public TrnSaleOrderController()
        {
            _trnSaleOrderService = new TrnSaleOrderService();
        }

        // GET api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnSaleOrderService.getSaleOrders(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnSaleOrder/1
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnSaleOrderService.getSaleOrderById(id);
            return Ok(result);
        }

        // POST api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpPost]
        public IHttpActionResult PostTrnSaleOrder(VMTrnSaleOrder saleorder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnSaleOrderService.postSaleOrder(saleorder);
            return Ok(result);
        }

        // PUT api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpPut]
        public IHttpActionResult PutTrnSaleOrder(VMTrnSaleOrder saleorder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnSaleOrderService.putSaleOrder(saleorder);
            return Ok(result);
        }

        // DELETE api/TrnSaleOrder/1
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpDelete]
        public IHttpActionResult DeleteTrnSaleOrder(long id)
        {
            return Ok();
        }
    }
}
