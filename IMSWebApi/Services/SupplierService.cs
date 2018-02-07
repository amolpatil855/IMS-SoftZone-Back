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
using System.Resources;
using System.Reflection;
using System.Transactions;
namespace IMSWebApi.Services
{
    public class SupplierService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        public SupplierService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMSupplier> getSupplier(int pageSize, int page, string search)
        {
            List<VMSupplier> supplierView;
            if (pageSize > 0)
            {
                var result = repo.MstSuppliers.OrderBy(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                supplierView = Mapper.Map<List<MstSupplier>, List<VMSupplier>>(result);
            }
            else
            {
                var result = repo.MstSuppliers.ToList();
                supplierView = Mapper.Map<List<MstSupplier>, List<VMSupplier>>(result);
            }
            supplierView.ForEach(s => s.MstSupplierAddresses.RemoveAll(a => a.isPrimary == false));
            supplierView = supplierView.Where(s => !string.IsNullOrEmpty(search) ?
                    s.name.StartsWith(search)
                    || s.firmName.StartsWith(search)
                    || s.code.StartsWith(search)
                    || s.email.StartsWith(search)
                    || s.phone.StartsWith(search) 
                    || (s.MstSupplierAddresses.Count() > 0 ? s.MstSupplierAddresses[0].gstin.StartsWith(search) : false) : true).ToList();
            return new ListResult<VMSupplier>
            {
                Data = supplierView,
                TotalCount = supplierView.Count(),
                Page = page
            };
        }

        public VMSupplier getSupplierById(Int64 id)
        {
            var result = repo.MstSuppliers.Where(s => s.id == id).FirstOrDefault();
            var supplier = Mapper.Map<MstSupplier, VMSupplier>(result);
            return supplier;
        }

        public List<VMLookUpItem> getSupplierLookUp()
        {
            return repo.MstSuppliers
                .OrderBy(s=>s.code)
                .Select(s => new VMLookUpItem { value = s.id, label = s.code }).ToList();
        }

        public ResponseMessage postSupplier(VMSupplier supplier)
        {
            using (var transaction = new TransactionScope())
            {
                MstSupplier supplierToPost = Mapper.Map<VMSupplier, MstSupplier>(supplier);
                var supplierAddresses = supplierToPost.MstSupplierAddresses.ToList();
                foreach (var saddress in supplierAddresses)
                {
                    saddress.createdOn = DateTime.Now;
                    saddress.createdBy = _LoggedInuserId;
                }
                supplierToPost.createdOn = DateTime.Now;
                supplierToPost.createdBy = _LoggedInuserId;

                repo.MstSuppliers.Add(supplierToPost);
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(supplierToPost.id, resourceManager.GetString("SupplierAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putSupplier(VMSupplier supplier)
        {
            using (var transaction = new TransactionScope())
            {  
                repo.MstSupplierAddresses.RemoveRange(repo.MstSupplierAddresses
                    .Where(s => s.supplierId == supplier.id));
                repo.SaveChanges();
                List<MstSupplierAddress> supplierAddressDetails = Mapper.Map<List<VMSupplierAddress>,
                   List<MstSupplierAddress>>(supplier.MstSupplierAddresses);
                
                foreach (var saddress in supplierAddressDetails)
                {
                    saddress.isPrimary = saddress.isPrimary == null ? false : saddress.isPrimary;
                    saddress.supplierId = supplier.id;
                    saddress.createdOn = DateTime.Now;
                    saddress.createdBy = _LoggedInuserId;
                }

                MstSupplier supplierToPut = repo.MstSuppliers.Where(s => s.id == supplier.id).FirstOrDefault();
                supplierToPut.name = supplier.name;
                supplierToPut.code = supplier.code;
                supplierToPut.firmName = supplier.firmName;
                supplierToPut.description = supplier.description;
                supplierToPut.email = supplier.email;
                supplierToPut.phone = supplier.phone;
                supplierToPut.accountPersonName = supplier.accountPersonName;
                supplierToPut.accountPersonEmail = supplier.accountPersonEmail;
                supplierToPut.accountPersonPhone = supplier.accountPersonPhone;
                supplierToPut.warehousePersonName = supplier.warehousePersonName;
                supplierToPut.warehousePersonEmail = supplier.warehousePersonEmail;
                supplierToPut.warehousePersonPhone = supplier.warehousePersonPhone;
                supplierToPut.dispatchPersonName = supplier.dispatchPersonName;
                supplierToPut.dispatchPersonEmail = supplier.dispatchPersonEmail;
                supplierToPut.dispatchPersonPhone = supplier.dispatchPersonPhone;
                supplierToPut.updatedOn = DateTime.Now;
                supplierToPut.updatedBy = _LoggedInuserId;
                supplierToPut.MstSupplierAddresses = supplierAddressDetails;
                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(supplier.id, resourceManager.GetString("SupplierUpdated"), ResponseType.Success);
            }
        }

        public ResponseMessage deleteSupplier(Int64 id)
        {  
            repo.MstSuppliers.Remove(repo.MstSuppliers.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("SupplierDeleted"), ResponseType.Success);
        }

    }
}