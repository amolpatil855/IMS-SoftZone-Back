using AutoMapper;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class MenuService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        public List<VMMenu> getMenu()
        {
            var result = repo.MstMenus.ToList();
            List<VMMenu> menuViews = Mapper.Map<List<MstMenu>, List<VMMenu>>(result);
            return menuViews;
        }
    }
}