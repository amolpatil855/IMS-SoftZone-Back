using Microsoft.AspNet.Identity;
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
    public class RoleController : ApiController
    {
        private RoleService _roleService = null;

        public RoleController()
        {
            _roleService = new RoleService();
        }

        // POST api/Role/getRole
        [Authorize]
        [ApiAuthorize(AccessLevel = "role")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize=0, int page=0,string search=null)
        {

            var temp = User.Identity.GetUserId();
            var result = _roleService.getRole(pageSize,page,search);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [ApiAuthorize(AccessLevel = "role")]
        public IHttpActionResult Get(Int64 id)
        {
            var result = _roleService.getRoleById(id);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [ApiAuthorize(AccessLevel = "role")]
        [Route("api/Role/getRoleMenu/{id}")]
        //  [ActionName("getRoleMenu")]  
        public IHttpActionResult getRoleMenu(Int64 id)
        {
            return Ok(_roleService.getRoleMenu(id));
        }

        [HttpPost]
        [Authorize]
        [ApiAuthorize(AccessLevel = "role")]
        public IHttpActionResult insertRole(VMRole role)
        {
            return Ok(_roleService.insertRole(role));
        }

        [HttpPut]
        [Authorize]
        [ApiAuthorize(AccessLevel = "role")]
        public IHttpActionResult updateRole(VMRole role)
        {
            return Ok(_roleService.updateRole(role));
        }

        [HttpDelete]
        [Authorize]
        [ApiAuthorize(AccessLevel = "role")]
        public IHttpActionResult deleRole(Int64 Id)
        {
            return Ok(_roleService.deleteRole(Id));
        }
    }
}
