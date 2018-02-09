using IMSWebApi.CustomAttributes;
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
        public TrnPurchaseOrderController()
        {
            
        }

        // GET api/TrnPurchaseOrder
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            return Ok();
        }

        // GET api/TrnPurchaseOrder/1
        [ApiAuthorize(AccessLevel = "purchaseorder")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            return Ok();
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
