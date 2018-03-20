using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
using IMSWebApi.ViewModel.SalesInvoice;
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

        // PUT api/TrnSaleOrder/1
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/TrnSaleOrder/ApproveSO/{id}")]
        public IHttpActionResult ApproveSO(long id)
        {
            var result = _trnSaleOrderService.approveSO(id);
            return Ok(result);
        }

        // PUT api/TrnSaleOrder/1
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpPut]
        [Route("api/TrnSaleOrder/CancelSO/{id}")]
        public IHttpActionResult CancelSO(long id)
        {
            var result = _trnSaleOrderService.cancelSO(id);
            return Ok(result);
        }

        // PUT api/TrnSaleOrder/1
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/TrnSaleOrder/CompleteSO/{id}")]
        public IHttpActionResult CompleteSO(long id)
        {
            var result = _trnSaleOrderService.completeSO(id);
            return Ok(result);
        }

        // GET api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpGet]
        [Route("api/TrnSaleOrder/GetSalesOrdersForLoggedInUser")]
        public IHttpActionResult GetSalesOrdersForLoggedInUser(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnSaleOrderService.getSalesOrdersForLoggedInUser(pageSize, page, search);
            return Ok(result);
        }

        // PUT api/TrnSaleOrder/1
        [ApiAuthorize(AccessLevel = "customerLogin")]
        [HttpPut]
        [Route("api/TrnSaleOrder/CancelSOForCustomerUser/{id}")]
        public IHttpActionResult CancelSOForCustomerUser(long id)
        {
            var result = _trnSaleOrderService.cancelSO(id);
            return Ok(result);
        }

    }
}
