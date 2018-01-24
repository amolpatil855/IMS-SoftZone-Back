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
                .OrderBy(s=>s.code)
                .Select(s => new VMLookUpItem { value = s.id, label = s.code }).ToList();
        }

        public List<VMLookUpItem> getFWRCategoryLookUp()
        {
            return repo.MstCategories
                .Where(s=>s.code!="Mattress" && s.code!="Foam" && s.code!="Accessories" )
                .OrderBy(s => s.code)
                .Select(s => new VMLookUpItem { value = s.id, label = s.code }).ToList();
        }

        public MstCategory getMatressCategory()
        {
            return repo.MstCategories.Where(c => c.code == "Mattress").FirstOrDefault();
        }

        public MstCategory getFoamCategory()
        {
            return repo.MstCategories.Where(c => c.code == "Foam").FirstOrDefault();
        }
    }
}