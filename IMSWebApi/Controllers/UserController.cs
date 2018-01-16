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

        //// PUT api/User/5
        //public IHttpActionResult PutMstUser(long id, MstUser mstuser)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != mstuser.id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(mstuser).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MstUserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

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

        //// DELETE api/User/5
        //[ResponseType(typeof(MstUser))]
        //public IHttpActionResult DeleteMstUser(long id)
        //{
        //    MstUser mstuser = db.MstUsers.Find(id);
        //    if (mstuser == null)
        //    {
        //        return NotFound();
        //    }

        //    db.MstUsers.Remove(mstuser);
        //    db.SaveChanges();

        //    return Ok(mstuser);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool MstUserExists(long id)
        //{
        //    return db.MstUsers.Count(e => e.id == id) > 0;
        //}
    }
}