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
    public class TrnPurchaseOrderController : ApiController
    {
        private TrnPurchaseOrderService _trnPurchaseOrderService = null;

        public TrnPurchaseOrderController()
        {
            _trnPurchaseOrderService = new TrnPurchaseOrderService();
        }

        // GET api/TrnPurchaseOrder
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnPurchaseOrderService.getPurchaseOrder(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnPurchaseOrder/1
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnPurchaseOrderService.getPurchaseOrderById(id);
            return Ok(result);
        }

        // POST api/TrnPurchaseOrder
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpPost]
        public IHttpActionResult PostTrnPurchaseOrder(VMTrnPurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // PUT api/TrnPurchaseOrder
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpPut]
        public IHttpActionResult PutTrnPurchaseOrder(VMTrnPurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // DELETE api/TrnPurchaseOrder/1
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpDelete]
        public IHttpActionResult DeleteTrnPurchaseOrder(long id)
        {
            return Ok();
        }
    }
}
