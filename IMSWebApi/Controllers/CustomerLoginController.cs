using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
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
    [ApiAuthorize(AccessLevel = "customerLogin")]
    public class CustomerLoginController : ApiController
    {
        private TrnSaleOrderService _trnSaleOrderService = null;
        private TrnSalesInvoiceService _trnSalesInvoiceService = null;
        private CustomerLoginService _customerLoginService = null;

        public CustomerLoginController()
        {
            _trnSaleOrderService = new TrnSaleOrderService();
            _trnSalesInvoiceService = new TrnSalesInvoiceService();
            _customerLoginService = new CustomerLoginService();
        }

        // GET api/CustomerLogin
        [HttpGet]
        [Route("api/CustomerLogin/GetSalesOrdersForLoggedInUser")]
        public IHttpActionResult GetSalesOrdersForLoggedInUser(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnSaleOrderService.getSalesOrdersForLoggedInUser(pageSize, page, search);
            return Ok(result);
        }

        // GET api/CustomerLogin/1
        [HttpGet]
        [Route("api/CustomerLogin/GetSalesOrderByIdForCustomerUser/{id}")]
        public IHttpActionResult GetSalesOrderByIdForCustomerUser(long id)
        {
            var result = _trnSaleOrderService.getSaleOrderById(id);
            return Ok(result);
        }

        // PUT api/CustomerLogin/1
        [HttpPut]
        [Route("api/CustomerLogin/CancelSOForCustomerUser/{id}")]
        public IHttpActionResult CancelSOForCustomerUser(long id)
        {
            var result = _trnSaleOrderService.cancelSO(id);
            return Ok(result);
        }

        // POST api/CustomerLogin
        [HttpPost]
        [Route("api/CustomerLogin/PostTrnSaleOrderForCustomerUser")]
        public IHttpActionResult PostTrnSaleOrderForCustomerUser(VMTrnSaleOrder saleorder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnSaleOrderService.postSaleOrder(saleorder);
            return Ok(result);
        }

        // PUT api/CustomerLogin
        [HttpPut]
        [Route("api/CustomerLogin/PutTrnSaleOrderForCustomerUser")]
        public IHttpActionResult PutTrnSaleOrderForCustomerUser(VMTrnSaleOrder saleorder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _trnSaleOrderService.putSaleOrder(saleorder);
            return Ok(result);
        }

        // GET api/CustomerLogin
        [HttpGet]
        [Route("api/CustomerLogin/GetSalesInvoicesForLoggedInUser")]
        public IHttpActionResult GetSalesInvoicesForLoggedInUser(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _trnSalesInvoiceService.getSalesInvoicesForLoggedInUser(pageSize, page, search);
            return Ok(result);
        }

        // GET api/CustomerLogin/1
        [HttpGet]
        [Route("api/CustomerLogin/GetSalesInvoiceByIdForCustomerUser/{id}")]
        public IHttpActionResult GetSalesInvoiceByIdForCustomerUser(long id)
        {
            var result = _trnSalesInvoiceService.getSalesInvoiceById(id);
            return Ok(result);
        }

        // GET api/CustomerLogin   
        [HttpGet]
        [Route("api/CustomerLogin/GetFabricProducts")]
        public IHttpActionResult GetFabricProducts(int pageSize = 0, int page = 0)
        {
            var result = _customerLoginService.getFabricProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/CustomerLogin   
        [HttpGet]
        [Route("api/CustomerLogin/GetFoamProducts")]
        public IHttpActionResult GetFoamProducts(int pageSize = 0, int page = 0)
        {
            var result = _customerLoginService.getFoamProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/CustomerLogin   
        [HttpGet]
        [Route("api/CustomerLogin/GetAccessoryProducts")]
        public IHttpActionResult GetAccessoryProducts(int pageSize = 0, int page = 0)
        {
            var result = _customerLoginService.getAccessoryProducts(pageSize, page);
            return Ok(result);
        }

        // GET api/CustomerLogin   
        [HttpGet]
        [Route("api/CustomerLogin/GetFabricProductsForExport")]
        public IHttpActionResult GetFabricProductsForExport()
        {
            var result = _customerLoginService.getFabricProductsForExport();
            return Ok(result);
        }

        // GET api/CustomerLogin   
        [HttpGet]
        [Route("api/CustomerLogin/GetFoamProductsForExport")]
        public IHttpActionResult GetFoamProductsForExport()
        {
            var result = _customerLoginService.getFoamProductsForExport();
            return Ok(result);
        }

        // GET api/CustomerLogin   
        [HttpGet]
        [Route("api/CustomerLogin/GetAccessoryProductsForExport")]
        public IHttpActionResult GetAccessoryProductsForExport()
        {
            var result = _customerLoginService.getAccessoryProductsForExport();
            return Ok(result);
        }

        // GET api/CustomerLogin  
        [HttpGet]
        [Route("api/CustomerLogin/GetDashboardData")]
        public IHttpActionResult GetDashboardData()
        {
            var result = _customerLoginService.getDashboardData();
            return Ok(result);
        }

        // GET api/CustomerLogin
        [HttpGet]
        [Route("api/CustomerLogin/GetRecordsForTotalOutstandingAmt")]
        public IHttpActionResult GetRecordsForTotalOutstandingAmt(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _customerLoginService.getRecordsForTotalOutstandingAmt(pageSize, page, search);
            return Ok(result);
        }

        // GET api/CustomerLogin
        [HttpGet]
        [Route("api/CustomerLogin/GetRecordsForSOCount")]
        public IHttpActionResult GetRecordsForSOCount(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _customerLoginService.getRecordsForSOCount(pageSize, page, search);
            return Ok(result);
        }
       
    }
}
