﻿using System;
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

namespace IMSWebApi.Controllers
{
    public class UserController : ApiController
    {
        private UserService _userService = null;

        public UserController()
        {
            _userService = new UserService();
        }

        // GET api/User
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = _userService.getUser();
            return Ok(result);
        }

        // GET api/User/5
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            var result = _userService.getUserById(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/User/GetUserType")]
        public IHttpActionResult GetUserType()
        {
            var result = _userService.getUserType();
            return Ok(result);
        }

        // PUT api/User/5
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
        [HttpDelete]
        public IHttpActionResult DeleteMstUser(long id)
        {   
            var result = _userService.deleteUser(id);
            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut]
        [Route("api/User/ChangePassword")]
        public IHttpActionResult ChangePassword(VMUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _userService.changePassword(user);
            if (result!=0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}