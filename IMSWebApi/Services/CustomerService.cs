using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWebApi.Services
{
    public class CustomerService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();

        public List<VMCustomer> getCustomer()
        {
            var result = repo.MstCustomers.ToList();
            List<VMCustomer> customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            return customerViews;
        }

        public VMCustomer getCustomerById(Int64 id)
        {
            var result = repo.MstCustomers.Where(c=>c.id == id).FirstOrDefault();
            VMCustomer customerViews = Mapper.Map<MstCustomer,VMCustomer>(result);
            return customerViews;
        }

        public ResponseMessage postCustomer(VMCustomer customer)
        {
            MstCustomer customerToPost = Mapper.Map<VMCustomer, MstCustomer>(customer);
            List<MstCustomerAddressDetail> customerAddress = customerToPost.MstCustomerAddressDetails.ToList();
            customerAddress.ForEach(d => d.createdOn = DateTime.Now);
            customerToPost.createdOn = DateTime.Now;
            customerToPost.createdBy = 1;
            repo.MstCustomers.Add(customerToPost);
            repo.SaveChanges();
            return new ResponseMessage(customerToPost.id, "Customer Added Successfully", ResponseType.Success);
        }

    }
}