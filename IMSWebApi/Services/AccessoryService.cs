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
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;

namespace IMSWebApi.Services
{
    public class AccessoryService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        CategoryService _categoryService = null;
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public AccessoryService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMAccessoryList> getAccessories(int pageSize, int page, string search)
        {
            List<VMAccessoryList> accessoryListView;
            accessoryListView = repo.MstAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search)
                    || a.itemCode.StartsWith(search)
                    || a.MstSupplier.code.StartsWith(search)
                    || a.purchaseRate.ToString().StartsWith(search)
                    || a.sellingRate.ToString().StartsWith(search)
                    || a.size.StartsWith(search) : true)
                    .Select(a => new VMAccessoryList
                    {
                        id = a.id,
                        name = a.name,
                        itemCode = a.itemCode,
                        supplierCode = a.MstSupplier != null ? a.MstSupplier.code : string.Empty,
                        sellingRate = a.sellingRate,
                        purchaseRate = a.purchaseRate,
                        size = a.size
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
               
            return new ListResult<VMAccessoryList>
            {
                Data = accessoryListView,
                TotalCount = repo.MstAccessories.Where(a => !string.IsNullOrEmpty(search)
                    ? a.name.StartsWith(search)
                    || a.itemCode.StartsWith(search)
                    || a.MstSupplier.code.StartsWith(search)
                    || a.purchaseRate.ToString().StartsWith(search)
                    || a.sellingRate.ToString().StartsWith(search)
                    || a.size.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMAccessory getAccessoryById(Int64 id)
        {
            var result = repo.MstAccessories.Where(s => s.id == id).FirstOrDefault();
            VMAccessory accessoryView = Mapper.Map<MstAccessory, VMAccessory>(result);
            return accessoryView;
        }

        public List<VMLookUpItem> getAccessoryLookUp()
        {
            return repo.MstAccessories
                .OrderBy(m => m.itemCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.itemCode}).ToList();
        }

        public List<VMLookUpItem> getAccessoryLookUpBySupplierId(Int64 supplierId)
        {
            return repo.MstAccessories.Where(a=>a.supplierId == supplierId)
                .OrderBy(m => m.itemCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.itemCode }).ToList();
        }

        public List<VMLookUpItem> getAccessoryLookUpForGRN()
        {
            return repo.TrnPurchaseOrderItems.Where(p => p.categoryId == 7 && (p.status.Equals("Approved") || p.status.Equals("PartialCompleted")))
                .OrderBy(m => m.MstAccessory.itemCode)
                .Select(q => new VMLookUpItem { value = q.MstAccessory.id, label = q.MstAccessory.itemCode }).Distinct().ToList();
        }

        public ResponseMessage postAccessory(VMAccessory accessory)
        {
            MstAccessory accessoryToPost = Mapper.Map<VMAccessory, MstAccessory>(accessory);
            accessoryToPost.categoryId = _categoryService.getAccessoryCategory().id;
            accessoryToPost.createdOn = DateTime.Now;
            accessoryToPost.createdBy = _LoggedInuserId;

            repo.MstAccessories.Add(accessoryToPost);
            repo.SaveChanges();
            return new ResponseMessage(accessoryToPost.id,
                resourceManager.GetString("AccessoryAdded"), ResponseType.Success);
        }

        public ResponseMessage putAccessory(VMAccessory accessory)
        {
            var accessoryToPut = repo.MstAccessories.Where(s => s.id == accessory.id).FirstOrDefault();
            accessoryToPut.name = accessory.name;
            accessoryToPut.itemCode = accessory.itemCode;
            accessoryToPut.supplierId = accessory.supplierId;
            accessoryToPut.hsnId = accessory.hsnId;
            accessoryToPut.uomId = accessory.uomId;
            accessoryToPut.sellingRate = accessory.sellingRate;
            accessoryToPut.purchaseRate = accessory.purchaseRate;
            accessoryToPut.size = accessory.size;
            accessoryToPut.description = accessory.description;
            accessoryToPut.updatedBy = _LoggedInuserId;
            accessoryToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(accessory.id, resourceManager.GetString("AccessoryUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteAccessory(Int64 id)
        {
            repo.MstAccessories.Remove(repo.MstAccessories.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("AccessoryDeleted"), ResponseType.Success);
        }

        public int UploadAccessories(HttpPostedFileBase file)
        {
            DataTable accessoryDataTable = new DataTable();
            accessoryDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateDataTable(accessoryDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Item Code*"].SetOrdinal(0);
            validatedDataTable.Columns["Name*"].SetOrdinal(1);
            validatedDataTable.Columns["Supplier Name*"].SetOrdinal(2);
            validatedDataTable.Columns["Hsn Code*"].SetOrdinal(3);
            validatedDataTable.Columns["Unit of Measurement*"].SetOrdinal(4);
            validatedDataTable.Columns["Selling Rate*"].SetOrdinal(5);
            validatedDataTable.Columns["Purchase Rate*"].SetOrdinal(6);
            validatedDataTable.Columns["Size*"].SetOrdinal(7);
            validatedDataTable.Columns["Description"].SetOrdinal(8);

            validatedDataTable.AcceptChanges();

            //var shade = repo.UploadFWRShade(validatedDataTable).ToList();

            //SqlParameter param = new SqlParameter("@FWRShadeType", SqlDbType.Structured);
            //param.TypeName = "dbo.FWRShadeType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRShade @FWRShadeType", param);
            //}

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadAccessories", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@AccessoriesType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Item Code*") == row["itemCode"].ToString()).FirstOrDefault());
                            InvalidData.Rows.Add(row.ItemArray);
                            row.Delete();
                        }
                        dt.AcceptChanges();
                    }
                }
            }

            validatedDataTable.AcceptChanges();
            InvalidData.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return 0;
        }

        /// <summary>
        /// validate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMAccessory();

            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                InvalidData.Columns.Add(colname);
                col.ColumnName = colname;
            }
            InvalidData.Columns.Add("Reason");
            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.hsnId = model.uomId  = model.supplierId = 1;
                model.itemCode = row["Item Code*"].ToString();
                model.name = row["Name*"].ToString();
                model.sellingRate = !string.IsNullOrWhiteSpace(row["Selling Rate*"].ToString()) ? Convert.ToDecimal(row["Selling Rate*"]) : 0;
                model.purchaseRate = !string.IsNullOrWhiteSpace(row["Purchase Rate*"].ToString()) ? Convert.ToDecimal(row["Purchase Rate*"]) : 0;
                model.size = row["Size*"].ToString();
                model.description = row["Description"].ToString();

                var context = new ValidationContext(model, null, null);
                var result = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(model, context, result, true);

                string errormessage = "";
                if (!isValid)
                {
                    InvalidData.Rows.Add(row.ItemArray);

                    //adding reason of invalid row
                    for (int i = 0; i < result.Count; i++)
                    {
                        errormessage = string.Join(".", string.Join(",", result.ElementAt(i)), errormessage);
                    }
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = errormessage;
                    row.Delete();
                }
            }

            rawTable.AcceptChanges();
            InvalidData.AcceptChanges();

            //finding distinct values of Supplier, HSN, UOM
            List<string> Supplier = new List<string>();
            List<string> Hsn = new List<string>();
            List<string> Uom = new List<string>();

            Supplier = rawTable.AsEnumerable().Select(c => c.Field<string>("Supplier Name*")).Distinct().ToList();
            Hsn = rawTable.AsEnumerable().Select(c => c.Field<string>("Hsn Code*")).Distinct().ToList();
            Uom = rawTable.AsEnumerable().Select(c => c.Field<string>("Unit of Measurement*")).Distinct().ToList();

            //formatting as comma separated string
            string supplierString = (string.Join(",", Supplier.ToArray()));
            string hsnString = (string.Join(",", Hsn.ToArray()));
            string uomString = (string.Join(",", Uom.ToArray()));

            //fetching data for Supplier, HSN, UOM
            var supplierKey = repo.GET_SUPPLIER_ID(supplierString).ToList();
            var hsnKey = repo.GET_HSN_ID(hsnString).ToList();
            var uomKey = repo.GET_UOM_ID(uomString).ToList();

            //replacing values of Supplier, HSN, UOM by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var supplierRow = supplierKey.Where(d => d.code.ToLower().Equals(rawTable.Rows[j]["Supplier Name*"].ToString().Trim().ToLower())).FirstOrDefault();
                var hsnRow = hsnKey.Where(d => d.hsnCode.ToLower().Equals(rawTable.Rows[j]["Hsn Code*"].ToString().Trim().ToLower())).FirstOrDefault();
                var uomRow = uomKey.Where(d => d.uomName.ToLower().Equals(rawTable.Rows[j]["Unit of Measurement*"].ToString().Trim().ToLower())).FirstOrDefault();
                if (supplierRow != null && hsnRow != null && uomRow != null)
                {
                    rawTable.Rows[j]["Supplier Name*"] = supplierRow.id;
                    rawTable.Rows[j]["Hsn Code*"] = hsnRow.id;
                    rawTable.Rows[j]["Unit of Measurement*"] = uomRow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify Supplier, HSN Code, UOM";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}