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
        [Route("api/TrnPurchaseOrder/GetCollectionBySuppliernCategoryId")]
        public IHttpActionResult GetCollectionBySuppliernCategoryId(long supplierId,long categoryId)
        {
            var result = _trnPurchaseOrderService.getCollectionBySuppliernCategoryId(supplierId,categoryId);
            return Ok(result);
        }

        // GET api/TrnPurchaseOrder
        //[ApiAuthorize(AccessLevel = "purchaseorder")]
        //[HttpGet]
        //[Route("api/TrnPurchaseOrder/GetPOItemsBySOId")]
        //public IHttpActionResult getPOItemsBySOId(long saleOrderId, long supplierId)
        //{
        //    var result = _trnPurchaseOrderService.getPOItemsBySOId(saleOrderId, supplierId);
        //    return Ok(result);
        //}

        // GET api/TrnPurchaseOrder
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnPurchaseOrderService.getPurchaseOrders(pageSize, page, search);
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
            var result = _trnPurchaseOrderService.postPurchaseOrder(purchaseOrder);
            return Ok(result);
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
            var result = _trnPurchaseOrderService.putPurchaseOrder(purchaseOrder);
            return Ok(result);
        }

        // DELETE api/TrnPurchaseOrder/1
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpDelete]
        public IHttpActionResult DeleteTrnPurchaseOrder(long id)
        {
            return Ok();
        }

        // GET api/TrnPurchaseOrder/1
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/TrnPurchaseOrder/ApprovePO/{id}")]
        public IHttpActionResult ApprovePO(long id)
        {
            var result = _trnPurchaseOrderService.approvePO(id);
            return Ok(result);
        }

        // GET api/TrnPurchaseOrder
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpGet]
        [Route("api/TrnPurchaseOrder/GetSupplierListForPO")]
        public IHttpActionResult GetSupplierListForPO()
        {
            var result = _trnPurchaseOrderService.getSupplierListForPO();
            return Ok(result);
        }

        // GET api/TrnPurchaseOrder/1
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/TrnPurchaseOrder/CancelPO/{id}")]
        public IHttpActionResult CancelPO(long id)
        {
            var result = _trnPurchaseOrderService.cancelPO(id);
            return Ok(result);
        }
    }
}
