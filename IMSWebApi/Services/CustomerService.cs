﻿using AutoMapper;
using IMSWebApi.Enums;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using System.Resources;
using System.Reflection;
using System.Transactions;

namespace IMSWebApi.Services
{
    public class CustomerService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        RoleService _roleService = new RoleService();
        UserService _userService = new UserService();
         Int64 _LoggedInuserId;
         ResourceManager resourceManager = null;
         public CustomerService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _roleService = new RoleService();
            _userService = new UserService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMCustomer> getCustomer(int pageSize, int page, string search)
        {
            List<VMCustomer> customerViews;
            if (pageSize > 0)
            {
                var result = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search) 
                    ? c.name.StartsWith(search) 
                    || c.email.StartsWith(search)
                    || c.phone.StartsWith(search)
                    || c.nickName.StartsWith(search)
                    || c.code.StartsWith(search) : true)
                    .OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            }
            else
            {
                var result = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search)
                    ? c.name.StartsWith(search)
                    || c.email.StartsWith(search)
                    || c.phone.StartsWith(search)
                    || c.nickName.StartsWith(search)
                    || c.code.StartsWith(search) : true).ToList();
                customerViews = Mapper.Map<List<MstCustomer>, List<VMCustomer>>(result);
            }
            customerViews.ForEach(s => s.MstCustomerAddresses.RemoveAll(a => a.isPrimary == false));
            return new ListResult<VMCustomer>
                {
                    Data = customerViews,
                    TotalCount = repo.MstCustomers.Where(c => !string.IsNullOrEmpty(search)
                        ? c.name.StartsWith(search)
                        || c.email.StartsWith(search)
                        || c.phone.StartsWith(search)
                        || c.nickName.StartsWith(search)
                        || c.code.StartsWith(search) : true).Count(),
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
            using (var transaction = new TransactionScope())
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
                transaction.Complete();
                return new ResponseMessage(customerToPost.id, resourceManager.GetString("CustomerAdded"),
                    ResponseType.Success);
            }
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
            var originalPassword = _userService.createRandomPassword(8);
            userToPost.password = UserService.encryption(originalPassword);
            userToPost.createdOn = DateTime.Now;
            userToPost.createdBy = _LoggedInuserId;
            repo.MstUsers.Add(userToPost);
            repo.SaveChanges();
            _userService.sendEmail(userToPost.id, originalPassword,"RegisterUser");
            userId = userToPost.id;
            return userId;
        }

        public ResponseMessage putCustomer(VMCustomer customer)
        {
            using (var transaction = new TransactionScope())
            {
                repo.MstCustomerAddresses.RemoveRange(repo.MstCustomerAddresses
                    .Where(s => s.customerId == customer.id));
                repo.SaveChanges();

                List<MstCustomerAddress> customerAddressDetails = Mapper.Map<List<VMCustomerAddress>,
                    List<MstCustomerAddress>>(customer.MstCustomerAddresses);
                foreach (var caddress in customerAddressDetails)
                {
                    caddress.isPrimary = caddress.isPrimary == null ? false : caddress.isPrimary;
                    caddress.customerId = customer.id;
                    caddress.createdOn = DateTime.Now;
                    caddress.createdBy = _LoggedInuserId;
                }

                MstCustomer customerToPut = repo.MstCustomers.Where(c => c.id == customer.id).FirstOrDefault();
                customerToPut.name = customer.name;
                customerToPut.nickName = customer.nickName;
                customerToPut.code = customer.code;
                customerToPut.email = customer.email;
                customerToPut.alternateEmail1 = customer.alternateEmail1;
                customerToPut.alternateEmail2 = customer.alternateEmail2;
                customerToPut.phone = customer.phone;
                customerToPut.alternatePhone1 = customer.alternatePhone1;
                customerToPut.alternatePhone2 = customer.alternatePhone2;
                customerToPut.isWholesaleCustomer = customer.isWholesaleCustomer;
                customerToPut.pan = customer.pan;
                customerToPut.accountPersonName = customer.accountPersonName;
                customerToPut.accountPersonEmail = customer.accountPersonEmail;
                customerToPut.accountPersonPhone = customer.accountPersonPhone;
                customerToPut.MstCustomerAddresses = customerAddressDetails;
                customerToPut.updatedOn = DateTime.Now;
                customerToPut.updatedBy = _LoggedInuserId;
                repo.SaveChanges();

                if (customer.userId!=null)
                {
                    MstUser user = repo.MstUsers.Where(u => u.id == customer.userId).FirstOrDefault();
                    user.email = customer.email;
                    repo.SaveChanges();
                }

                transaction.Complete();
                return new ResponseMessage(customer.id, resourceManager.GetString("CustomerUpdated"), ResponseType.Success);
            }
          
        }

        public ResponseMessage deleteCustomer(Int64 id)
        {
            using (var transaction = new TransactionScope())
            {
                MstCustomer customerToDelete = repo.MstCustomers.Where(c => c.id == id).FirstOrDefault();
                repo.MstUsers.Remove(repo.MstUsers.Where(u => u.id == customerToDelete.userId).FirstOrDefault());
                repo.MstCustomers.Remove(repo.MstCustomers.Where(s => s.id == id).FirstOrDefault());
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(id, resourceManager.GetString("CustomerDeleted"), ResponseType.Success);
            }
        }

    }
}