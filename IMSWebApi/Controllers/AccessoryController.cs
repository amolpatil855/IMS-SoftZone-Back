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
    public class AccessoryController : ApiController
    {
        private AccessoryService _accessoryService = null;

        public AccessoryController()
        {
            _accessoryService = new AccessoryService();
        }

        // GET api/Accessory   
        [ApiAuthorize(AccessLevel = "accessory")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _accessoryService.getAccessory(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Accessory/1
        [ApiAuthorize(AccessLevel = "accessory")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _accessoryService.getAccessoryById(id);
            return Ok(result);
        }

        // POST api/Accessory
        [ApiAuthorize(AccessLevel = "accessory")]
        [HttpPost]
        public IHttpActionResult PostAccessory(VMAccessory accessory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _accessoryService.postAccessory(accessory);
            return Ok(result);
        }

        // PUT api/Accessory
        [ApiAuthorize(AccessLevel = "accessory")]
        [HttpPut]
        public IHttpActionResult PutAccessory(VMAccessory accessory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _accessoryService.putAccessory(accessory);
            return Ok(result);
        }

        // DELETE api/Accessory/1
        [ApiAuthorize(AccessLevel = "accessory")]
        [HttpDelete]
        public IHttpActionResult DeleteAccessory(long id)
        {
            var result = _accessoryService.deleteAccessory(id);
            return Ok(result);
        }

    }
}
