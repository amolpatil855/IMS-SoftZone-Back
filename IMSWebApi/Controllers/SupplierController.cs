using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IMSWebApi.Models;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
using IMSWebApi.Enums;
using IMSWebApi.CustomAttributes;

namespace IMSWebApi.Controllers
{
    public class SupplierController : ApiController
    {
        private SupplierService _supplierService = null;
        public SupplierController()
        {
            _supplierService = new SupplierService();
        }
        // GET api/Supplier
        [Authorize]
        [ApiAuthorize(AccessLevel = "supplier")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _supplierService.getSupplier(pageSize, page, search);
           return Ok(result);
        }

        // GET api/Supplier/1
        [Authorize]
        [ApiAuthorize(AccessLevel = "supplier")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _supplierService.getSupplierById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Supplier/GetSupplierLookUp")]
        public IHttpActionResult GetSupplierLookUp()
        {
            var result = _supplierService.getSupplierLookUp();
            return Ok(result);
        }

        // PUT api/Supplier/1
        [Authorize]
        [ApiAuthorize(AccessLevel = "supplier")]
        [HttpPut]
        public IHttpActionResult PutSupplier(VMSupplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _supplierService.putSupplier(supplier);
            return Ok(result);
        }

        // POST api/Supplier/1
        [Authorize]
        [ApiAuthorize(AccessLevel = "supplier")]
        [HttpPost]
        public IHttpActionResult PostSupplier(VMSupplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _supplierService.postSupplier(supplier);
            return Ok(result);
        }

        // DELETE api/Supplier/1
        [Authorize]
        [ApiAuthorize(AccessLevel = "supplier")]
        [HttpDelete]
        public IHttpActionResult DeleteSupplier(long id)
        {
            var result = _supplierService.deleteSupplier(id);
            return Ok(result);
        }
    }
}
