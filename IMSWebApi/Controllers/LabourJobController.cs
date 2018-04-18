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
    public class LabourJobController : ApiController
    {
        private LabourJobService _labourJobService = null;

        public LabourJobController()
        {
            _labourJobService = new LabourJobService();
        }

        // GET api/LabourJob   
        [ApiAuthorize(AccessLevel = "labourjob")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string isLabourChargePaid = null, long? tailorId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var result = _labourJobService.getLabourJobs(pageSize, page, isLabourChargePaid, tailorId, startDate, endDate);
            return Ok(result);
        }

        // PUT api/LabourJob/5
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/LabourJob/PaidLabourCharge/{id}")]
        public IHttpActionResult PaidLabourCharge(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _labourJobService.paidLabourCharge(id);
            return Ok(result);
        }
    }
}
