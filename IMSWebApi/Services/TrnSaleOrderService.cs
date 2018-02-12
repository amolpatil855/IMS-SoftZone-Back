using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using AutoMapper;
using IMSWebApi.ViewModel;

namespace IMSWebApi.Services
{
    public class TrnSaleOrderService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public TrnSaleOrderService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnSaleOrder> getSaleOrder(int pageSize, int page, string search)
        {
            List<VMTrnSaleOrder> saleOrderView;
            if (pageSize > 0)
            {
                var result = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.orderDate.ToString().StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                saleOrderView = Mapper.Map<List<TrnSaleOrder>, List<VMTrnSaleOrder>>(result);
            }
            else
            {
                var result = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.orderDate.ToString().StartsWith(search) : true).ToList();
                saleOrderView = Mapper.Map<List<TrnSaleOrder>, List<VMTrnSaleOrder>>(result);
            }

            return new ListResult<VMTrnSaleOrder>
            {
                Data = saleOrderView,
                TotalCount = repo.TrnSaleOrders.Where(so => !string.IsNullOrEmpty(search)
                    ? so.orderNumber.StartsWith(search)
                    || so.orderDate.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnSaleOrder getSaleOrderById(Int64 id)
        {
            var result = repo.TrnSaleOrders.Where(so => so.id == id).FirstOrDefault();
            VMTrnSaleOrder saleOrderView = Mapper.Map<TrnSaleOrder, VMTrnSaleOrder>(result);
            return saleOrderView;
        }
    }
}