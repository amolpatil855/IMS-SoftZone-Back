using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using IMSWebApi.Models;
using IMSWebApi.Services;
using IMSWebApi.ViewModel;
using System.Security.Claims;
using IMSWebApi.CustomAttributes;

namespace IMSWebApi.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private UserService _userService = null;

        public UserController()
        {
            _userService = new UserService();
        }

        // GET api/User
        [ApiAuthorize(AccessLevel = "user")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            var result = _userService.getUsers(pageSize, page, search);
            return Ok(result);
        }

        // GET api/User/5
        [ApiAuthorize(AccessLevel = "user")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _userService.getUserById(id);
            return Ok(result);
        }
        
        //Get Currently Logged In User Details
        [HttpGet]
        [Route("api/User/GetLoggedInUserDetail")]
        public IHttpActionResult GetLoggedInUserDetail()
        {
            var identity = User.Identity.Name;
            var result = _userService.getLoggedInUserDetails(User.Identity.Name);
            return Ok(result);
        }

        //Get Permissions for User
        [HttpGet]
        [Route("api/User/getPermissions")]
        public IHttpActionResult getPermissions()
        {
            var res = User.Identity.Name;
            var result = _userService.getUserPermission(User.Identity.Name);
            return Ok(result);
        }

        // PUT api/User/5
        [ApiAuthorize(AccessLevel = "user")]
        [HttpPut]
        public IHttpActionResult PutMstUser(VMUser mstuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _userService.putUser(mstuser);
            return Ok(result);
        }

         //POST api/User
        [ApiAuthorize(AccessLevel = "user")]
        [HttpPost]
        public IHttpActionResult PostMstUser(VMUser mstuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _userService.postUser(mstuser);
            return Ok(result);
        }

        // DELETE api/User/5
        [ApiAuthorize(AccessLevel = "user")]
        [HttpDelete]
        public IHttpActionResult DeleteMstUser(long id)
        {   
            var result = _userService.deleteUser(id);
            return Ok(result);
        }
        
        //Change Password for Current User
        [HttpPut]
        [Route("api/User/ChangePassword")]
        public IHttpActionResult ChangePassword(VMUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _userService.changePassword(user);
            if (result.type.Equals("Error"))
            {
                return BadRequest(result.message);
            }
            else
            {
                return Ok(result);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/User/ForgetPassword")]
        public IHttpActionResult ForgetPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _userService.forgetPassword(email);
            if (result.type.Equals("Error"))
            {
                return BadRequest(result.message);
            }
            else
            {
            return Ok(result);
            }
        }

        // PUT api/User/5
        [ApiAuthorize(Roles = "Administrator")]
        [HttpPut]
        [Route("api/User/ActivateDeActivateUser/{id}")]
        public IHttpActionResult ActivateDeActivateUser(long id,bool isActive)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _userService.activateDeActivateUser(id,isActive);
            return Ok(result);
        }

    }
}