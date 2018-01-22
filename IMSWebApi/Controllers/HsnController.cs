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
    public class HsnController : ApiController
    {
        private HsnService _hsnService = null;

        public HsnController()
        {
            _hsnService = new HsnService();
        }

        // GET api/Hsn   
        [ApiAuthorize(AccessLevel = "hsn")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _hsnService.getHsn(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Hsn/1 
        [ApiAuthorize(AccessLevel = "hsn")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _hsnService.getHsnById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Hsn/GetHsnLookUp")]
        public IHttpActionResult GetHsnLookUp()
        {
            var result = _hsnService.getHsnLookUp();
            return Ok(result);
        }

        // POST api/Hsn
        [ApiAuthorize(AccessLevel = "hsn")]
        [HttpPost]
        public IHttpActionResult PostHsn(VMHsn hsn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _hsnService.postHsn(hsn);
            return Ok(result);
        }

        // PUT api/Hsn
        [ApiAuthorize(AccessLevel = "hsn")]
        [HttpPut]
        public IHttpActionResult PutHsn(VMHsn hsn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _hsnService.putHsn(hsn);
            return Ok(result);
        }

        // DELETE api/Hsn/1
        [ApiAuthorize(AccessLevel = "hsn")]
        [HttpDelete]
        public IHttpActionResult DeleteQuality(long id)
        {
            var result = _hsnService.deleteHsn(id);
            return Ok(result);
        }
    }
}
