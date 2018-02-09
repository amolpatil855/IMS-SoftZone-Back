﻿using IMSWebApi.CustomAttributes;
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
    public class TrnSaleOrderController : ApiController
    {


        public TrnSaleOrderController()
        {

        }

        // GET api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpGet]
        public IHttpActionResult Get(int pageSize = 0, int page = 0, string search = null)
        {
            return Ok();
        }

        // GET api/TrnSaleOrder/1
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpGet]
        public IHttpActionResult Get(long id)
        {
            return Ok();
        }

        // POST api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpPost]
        public IHttpActionResult PostTrnSaleOrder(VMTrnSaleOrder salesorder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // PUT api/TrnSaleOrder
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpPut]
        public IHttpActionResult PutTrnSaleOrder(VMTrnSaleOrder salesorder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // DELETE api/TrnSaleOrder/1
        [ApiAuthorize(AccessLevel = "salesorder")]
        [HttpDelete]
        public IHttpActionResult DeleteTrnSaleOrder(long id)
        {
            return Ok();
        }
    }
}
