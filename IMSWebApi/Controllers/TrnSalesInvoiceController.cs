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
    public class TrnSalesInvoiceController : ApiController
    {
        private TrnSalesInvoiceService _trnSalesInvoiceService = null;

        public TrnSalesInvoiceController()
        {
            _trnSalesInvoiceService = new TrnSalesInvoiceService();
        }

        // GET api/TrnSalesInvoice
        //[ApiAuthorize(AccessLevel = "salesorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnSalesInvoiceService.getSalesInvoices(pageSize, page, search);
            return Ok(result);
        }

        // GET api/TrnSalesInvoice/1
        //[ApiAuthorize(AccessLevel = "salesorder")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _trnSalesInvoiceService.getSalesInvoiceById(id);
            return Ok(result);
        }
    }
}
