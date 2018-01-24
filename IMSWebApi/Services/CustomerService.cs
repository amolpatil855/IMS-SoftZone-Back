using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;

namespace IMSWebApi.Services
{
    public class CustomerService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        RoleService _roleService = new RoleService();
        UserService _userService = new UserService();
         Int64 _LoggedInuserId;
         public CustomerService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _roleService = new RoleService();
            _userService = new UserService();
        }

        public ListResult<VMCustomer> getCustomer(int pageSize, int page, string search)
        {
            List<VMCustomer> customerViews;
            if (pageSize > 0)
            {
                var result = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) 
                    ? c.code.StartsWith(search) 
                    || c.phone.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            }
            else
            {
                var result = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) 
                    ? c.code.StartsWith(search) 
                    || c.phone.StartsWith(search) : true).ToList();
                customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            }

            return new ListResult<VMCustomer>
                {
                    Data = customerViews,
                    TotalCount = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) 
                        ? c.code.StartsWith(search) || c.phone.StartsWith(search) : true).Count(),
                    Page = page
                };
        }

        public VMCustomer getCustomerById(Int64 id)
        {
            var result = repo.MstCustomers.Where(c=>c.id == id).FirstOrDefault();
            VMCustomer customerViews = Mapper.Map<MstCustomer,VMCustomer>(result);
            return customerViews;
        }

        public List<VMLookUpItem> getCustomerLookUp()
        {
            return repo.MstCustomers
                .OrderBy(s=>s.code)
                .Select(s => new VMLookUpItem { value = s.id, label = s.name }).ToList();
        }

        public ResponseMessage postCustomer(VMCustomer customer)
        {
            Nullable<long> userId = null;
            if (customer.isWholesaleCustomer == true)
            {
                userId = CreateUser(customer, userId);
            }
            MstCustomer customerToPost = Mapper.Map<VMCustomer, MstCustomer>(customer);
            customerToPost.userId = userId;
            List<MstCustomerAddress> customerAddress = customerToPost.MstCustomerAddresses.ToList();
            foreach (var caddress in customerAddress)
            {
                caddress.createdOn = DateTime.Now;
                caddress.createdBy = _LoggedInuserId;
            }
            customerToPost.createdOn = DateTime.Now;
            customerToPost.createdBy = _LoggedInuserId;
            repo.MstCustomers.Add(customerToPost);
            repo.SaveChanges();
            return new ResponseMessage(customerToPost.id, "Customer Added Successfully", 
                ResponseType.Success);
        }

        private long? CreateUser(VMCustomer customer, Nullable<long> userId)
        {
            VMUser user = new VMUser();
            user.userName = customer.userName;
            user.email = customer.email;
            user.phone = customer.phone;
            user.roleId = _roleService.getCustomerRole().id;
            user.userTypeId = _userService.getCustomerUserType().id;
            MstUser userToPost = Mapper.Map<VMUser, MstUser>(user);
            userToPost.password = _userService.createRandomPassword(8);
            userToPost.createdOn = DateTime.Now;
            userToPost.createdBy = _LoggedInuserId;
            repo.MstUsers.Add(userToPost);
            repo.SaveChanges();
            _userService.sendEmail(userToPost.id, "RegisterUser");
            userId = userToPost.id;
            return userId;
        }

        public ResponseMessage putCustomer(VMCustomer customer)
        {
            Nullable<long> userId = null;
            var customerAddressDetails = Mapper.Map<List<VMCustomerAddress>, 
                List<MstCustomerAddress>>(customer.MstCustomerAddresses);
            repo.MstCustomerAddresses.RemoveRange(repo.MstCustomerAddresses
                .Where(s => s.customerId== customer.id));
            repo.SaveChanges();

            foreach (var caddress in customerAddressDetails)
            {
                caddress.createdOn = DateTime.Now;
                caddress.createdBy = _LoggedInuserId;
            }
            var customerToPut = repo.MstCustomers.Where(s => s.id == customer.id).FirstOrDefault();
            if(customerToPut.userId ==null && customer.isWholesaleCustomer == true)
            {
                userId = CreateUser(customer, userId);
            }
            else if (customer.isWholesaleCustomer == false && customer.userId!=null)
            {
                repo.MstUsers.Remove(repo.MstUsers.Where(u => u.id == userId).FirstOrDefault());
                repo.SaveChanges();
                customer.userId = null;
            }
            customerToPut = Mapper.Map<VMCustomer, MstCustomer>(customer, customerToPut);
            customerToPut.updatedOn = DateTime.Now;
            customerToPut.updatedBy = _LoggedInuserId;
            customerToPut.userId = userId!=null ? userId : customer.userId;

            customerToPut.MstCustomerAddresses = customerAddressDetails;
            repo.SaveChanges();

            return new ResponseMessage(customer.id, "Customer Updated Successfully", ResponseType.Success);
        }

        public ResponseMessage deleteCustomer(Int64 id)
        {
            repo.MstCustomerAddresses.RemoveRange(repo.MstCustomerAddresses.Where(s => s.customerId== id));
            repo.SaveChanges();
            MstCustomer customerToDelete = repo.MstCustomers.Where(c => c.id == id).FirstOrDefault();
            repo.MstUsers.Remove(repo.MstUsers.Where(u => u.id == customerToDelete.userId).FirstOrDefault());
            repo.MstCustomers.Remove(repo.MstCustomers.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, "Customer Deleted Successfully", ResponseType.Success);
        }

    }
}