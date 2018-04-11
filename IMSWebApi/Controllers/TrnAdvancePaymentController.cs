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
    public class TrnAdvancePaymentController : ApiController
    {
        private TrnAdvancePaymentService _trnAdvancePaymentService = null;

        public TrnAdvancePaymentController()
        {
            _trnAdvancePaymentService = new TrnAdvancePaymentService();
        }

        // GET api/TrnAdvancePayment
        [ApiAuthorize(AccessLevel = "advancepayment")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null, string quotationType = null)
        {
            var result = _trnAdvancePaymentService.getAdvancePayments(pageSize, page, search, quotationType);
            return Ok(result);
        }

        // GET api/TrnAdvancePayment/1
        [ApiAuthorize(AccessLevel = "advancepayment")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnAdvancePaymentService.getAdvancePaymentById(id);
            return Ok(result);
        }

        // POST api/TrnAdvancePayment
        [ApiAuthorize(AccessLevel = "advancepayment")]
        [HttpPost]
        public IHttpActionResult PostTrnSaleOrder(VMTrnAdvancePayment advancePayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnAdvancePaymentService.postAdvancePayment(advancePayment);
            return Ok(result);
        }

        // PUT api/TrnAdvancePayment
        [ApiAuthorize(AccessLevel = "advancepayment")]
        [HttpPut]
        public IHttpActionResult PutTrnAdvancePayment(VMTrnAdvancePayment advancePayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnAdvancePaymentService.putAdvancePayment(advancePayment);
            return Ok(result);
        }
    }
}
