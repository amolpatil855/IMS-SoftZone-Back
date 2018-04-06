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
    public class TrnSalesInvoiceController : ApiController
    {
        private TrnSalesInvoiceService _trnSalesInvoiceService = null;

        public TrnSalesInvoiceController()
        {
            _trnSalesInvoiceService = new TrnSalesInvoiceService();
        }

        // GET api/TrnSalesInvoice
        [ApiAuthorize(AccessLevel = "invoice")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnSalesInvoiceService.getSalesInvoices(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnSalesInvoice/1
        [ApiAuthorize(AccessLevel = "invoice")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnSalesInvoiceService.getSalesInvoiceById(id);
            return Ok(result);
        }

        // PUT api/TrnSalesInvoice
        [ApiAuthorize(AccessLevel = "invoice")]
        [HttpPut]
        public IHttpActionResult PutTrnSalesInvoice(VMTrnSalesInvoice salesInvoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnSalesInvoiceService.putSalesInvoice(salesInvoice);
            return Ok(result);
        }

        // GET api/TrnSalesInvoice/1
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/TrnSalesInvoice/ApproveSalesInvoice/{id}")]
        public IHttpActionResult ApproveSalesInvoice(long id)
        {
            var result = _trnSalesInvoiceService.approveSalesInvoice(id);
            return Ok(result);
        }
    }
}
