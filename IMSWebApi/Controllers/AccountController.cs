using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IMSWebApi.Models;

namespace IMSWebApi.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;

        public AccountController()
        {
            _repo = new AuthRepository();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public IHttpActionResult Register(User userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = _repo.RegisterUser(userModel);


            return Ok();
        }
    }
}
