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
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _roleService.getRole();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Get(Int64 id)
        {
            var result = _roleService.getRoleById(id);
            return Ok(result);
        }
       
        [HttpGet]
        [Route("api/{Role}/{getRoleMenu}/{id}")] 
      //  [ActionName("getRoleMenu")]  
        public IHttpActionResult getRoleMenu(Int64 id)
        {
            return Ok(_roleService.getRoleMenu(id));
        }

        [HttpPost]
        public IHttpActionResult insertRole(VMRole role)
        {
            return Ok(_roleService.insertRole(role));
        }


    }
}
