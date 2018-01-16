using IMSWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IMSWebApi.Controllers
{
    public class MenuController : ApiController
    {
        private MenuService _menuService = null;
        public MenuController()
        {
            _menuService = new MenuService();
        }
        // GET: api/Menu
        public IHttpActionResult Get()
        {
            var result = _menuService.getMenu();
            return Ok(result);
        }
    }
}
