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
    public class CustomerController : ApiController
    {
        private CustomerService _customerService = null;

        public CustomerController()
        {
            _customerService = new CustomerService();
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _customerService.getCustomer();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Get(Int64 id)
        {
            var result = _customerService.getCustomerById(id);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult postCustomer(VMCustomer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_customerService.postCustomer(customer));
        }
    }
}
