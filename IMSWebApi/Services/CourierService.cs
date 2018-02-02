using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;

namespace IMSWebApi.Services
{
    public class CourierService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public CourierService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMCourier> getCourier(int pageSize, int page, string search)
        {
            List<VMCourier> courierView;
            if (pageSize > 0)
            {
                var result = repo.MstCouriers.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search): true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                courierView = Mapper.Map<List<MstCourier>, List<VMCourier>>(result);
            }
            else
            {
                var result = repo.MstCouriers.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search) : true).ToList();
                courierView = Mapper.Map<List<MstCourier>, List<VMCourier>>(result);
            }

            return new ListResult<VMCourier>
            {
                Data = courierView,
                TotalCount = repo.MstCouriers.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMCourier getCourierById(Int64 id)
        {
            var result = repo.MstCouriers.Where(s => s.id == id).FirstOrDefault();
            VMCourier courierView = Mapper.Map<MstCourier, VMCourier>(result);
            return courierView;
        }

        public List<VMLookUpItem> getCourierLookup()
        {
            return repo.MstCouriers.OrderBy(s => s.name)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.name
                }).ToList();
        }

        public ResponseMessage postCourier(VMCourier courier)
        {
            MstCourier courierToPost = Mapper.Map<VMCourier, MstCourier>(courier);
            courierToPost.createdOn = DateTime.Now;
            courierToPost.createdBy = _LoggedInuserId;

            repo.MstCouriers.Add(courierToPost);
            repo.SaveChanges();
            return new ResponseMessage(courierToPost.id,
                resourceManager.GetString("CourierAdded"), ResponseType.Success);
        }

        public ResponseMessage putCourier(VMCourier courier)
        {
            var courierToPut = repo.MstCouriers.Where(s => s.id == courier.id).FirstOrDefault();
            courierToPut.name = courier.name;
            courierToPut.docketNumber = courier.docketNumber;
            courierToPut.updatedBy = _LoggedInuserId;
            courierToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(courier.id, resourceManager.GetString("CourierUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteCourier(Int64 id)
        {
            repo.MstCouriers.Remove(repo.MstCouriers.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("CourierDeleted"), ResponseType.Success);
        }
    }
}