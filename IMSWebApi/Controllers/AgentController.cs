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
    public class AgentController : ApiController
    {
        private AgentService _agentService = null;

        public AgentController()
        {
            _agentService = new AgentService();
        }

        // GET api/Agent
        [ApiAuthorize(AccessLevel = "agent")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _agentService.getAgent(pageSize, page, search);
            return Ok(result);
        }

        // GET api/Agent/1
        [ApiAuthorize(AccessLevel = "agent")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _agentService.getAgentById(id);
            return Ok(result);
        }

        // POST api/Agent
        [ApiAuthorize(AccessLevel = "agent")]
        [HttpPost]
        public IHttpActionResult PostAgent(VMAgent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _agentService.postAgent(agent);
            return Ok(result);
        }

        // PUT api/Agent
        [ApiAuthorize(AccessLevel = "agent")]
        [HttpPut]
        public IHttpActionResult PutAgent(VMAgent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _agentService.putAgent(agent);
            return Ok(result);
        }

        // DELETE api/Agent/1
        [ApiAuthorize(AccessLevel = "agent")]
        [HttpDelete]
        public IHttpActionResult DeleteAgent(long id)
        {
            var result = _agentService.deleteAgent(id);
            return Ok(result);
        }
    }
}
