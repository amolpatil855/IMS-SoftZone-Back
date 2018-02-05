using IMSWebApi.CustomAttributes;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class CustomerController : ApiController
    {
        private CustomerService _customerService = null;

        public CustomerController()
        {
            _customerService = new CustomerService();
        }

        // GET api/Customer
        [ApiAuthorize(AccessLevel = "customer")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _customerService.getCustomer(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Customer/1
        [ApiAuthorize(AccessLevel = "customer")]
        [HttpGet]
        public IHttpActionResult Get(Int64 id)
        {
            var result = _customerService.getCustomerById(id);
            return Ok(result);
        }

        // POST api/Customer
        [ApiAuthorize(AccessLevel = "customer")]
        [HttpPost]
        public IHttpActionResult postCustomer(VMCustomer customer)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_customerService.postCustomer(customer));
        }

        // PUT api/Customer/
        [ApiAuthorize(AccessLevel = "customer")]
        [HttpPut]
        public IHttpActionResult PutCustomer(VMCustomer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _customerService.putCustomer(customer);
            return Ok(result);
        }

        // DELETE api/Customer/1
        [ApiAuthorize(AccessLevel = "customer")]
        [HttpDelete]
        public IHttpActionResult DeleteCustomer(long id)
        {
            var result = _customerService.deleteCustomer(id);
            return Ok(result);
        }


    }
}
