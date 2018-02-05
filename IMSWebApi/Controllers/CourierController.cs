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
    public class CourierController : ApiController
    {
        private CourierService _courierService = null;

        public CourierController()
        {
            _courierService = new CourierService();
        }

        // GET api/Courier
        [ApiAuthorize(AccessLevel = "courier")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _courierService.getCourier(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Courier/1
        [ApiAuthorize(AccessLevel = "courier")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _courierService.getCourierById(id);
            return Ok(result);
        }

        // POST api/Courier
        [ApiAuthorize(AccessLevel = "courier")]
        [HttpPost]
        public IHttpActionResult PostCourier(VMCourier courier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _courierService.postCourier(courier);
            return Ok(result);
        }

        // PUT api/Courier
        [ApiAuthorize(AccessLevel = "courier")]
        [HttpPut]
        public IHttpActionResult PutCourier(VMCourier courier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _courierService.putCourier(courier);
            return Ok(result);
        }

        // DELETE api/Courier/1
        [ApiAuthorize(AccessLevel = "courier")]
        [HttpDelete]
        public IHttpActionResult DeleteCourier(long id)
        {
            var result = _courierService.deleteCourier(id);
            return Ok(result);
        }
    }
}
