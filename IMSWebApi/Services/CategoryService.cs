using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class CategoryService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public List<VMLookUpItem> getCategoryLookUp()
        {
            return repo.MstCategories
                .Select(s => new VMLookUpItem { value = s.id, label = s.name }).ToList();
        }
    }
}