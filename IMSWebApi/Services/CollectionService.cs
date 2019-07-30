using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;
using System.Resources;
using System.Reflection;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;

namespace IMSWebApi.Services
{
    public class CollectionService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        CategoryService _categoryService;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();
        public CollectionService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _categoryService = new CategoryService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMCollectionList> getCollections(int pageSize, int page, string search)
        {
            List<VMCollectionList> collectionListView;
            collectionListView = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search)
                    ? c.MstCategory.code.StartsWith(search)
                    || c.collectionCode.StartsWith(search)
                    || c.manufacturerName.StartsWith(search)
                    || c.MstSupplier.code.StartsWith(search)
                    || c.collectionName.StartsWith(search) : true)
                    .Select(c => new VMCollectionList
                    {
                        id = c.id,
                        categoryCode = c.MstCategory != null ? c.MstCategory.code : string.Empty,
                        collectionCode = c.collectionCode,
                        collectionName = c.collectionName,
                        supplierCode = c.MstSupplier != null ? c.MstSupplier.code : string.Empty,
                        manufacturerName = c.manufacturerName
                    })
                    .OrderByDescending(p => p.id).Skip(page * pageSize).Take(pageSize).ToList();
                           
            return new ListResult<VMCollectionList>
            {
                Data = collectionListView,
                TotalCount = repo.MstCollections.Where(c => !string.IsNullOrEmpty(search)
                    ? c.MstCategory.code.StartsWith(search)
                    || c.collectionCode.StartsWith(search)
                    || c.manufacturerName.StartsWith(search)
                    || c.MstSupplier.code.StartsWith(search)
                    || c.collectionName.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMCollection getCollectionById(Int64 id)
        {
            var result = repo.MstCollections.Where(s => s.id == id).FirstOrDefault();
            VMCollection collectionView = Mapper.Map<MstCollection, VMCollection>(result);
            return collectionView;
        }

        public List<VMLookUpItem> getCollectionLookUpByCategoryId(Int64 categoryId)
        {
            return repo.MstCollections.Where(c => c.categoryId == categoryId)
                .OrderBy(s=>s.collectionCode)
                .Select(s => new VMLookUpItem{ value = s.id, label = s.collectionCode +" (" + 
                    s.MstSupplier.code +")"
                }).ToList();
        }

        public List<VMLookUpItem> getCollectionLookUpForSO(Int64 categoryId)
        {
            if (categoryId == 1)
            {
                List<Int64> collectionIds = repo.MstQualities.Where(q => q.flatRate != null).Select(c => c.collectionId).Distinct().ToList();
                return repo.MstCollections.Where(c => c.categoryId == categoryId && collectionIds.Contains(c.id))
                    .OrderBy(s => s.collectionCode)
                    .Select(s => new VMLookUpItem
                    {
                        value = s.id,
                        label = s.collectionCode + " (" +
                            s.MstSupplier.code + ")"
                    }).ToList();
            }
            else
            {
                return repo.MstCollections.Where(c => c.categoryId == categoryId)
                    .OrderBy(s => s.collectionCode)
                    .Select(s => new VMLookUpItem
                    {
                        value = s.id,
                        label = s.collectionCode + " (" +
                            s.MstSupplier.code + ")"
                    }).ToList();
            }
            
        }

        public List<VMLookUpItem> getMatCollectionLookUp()
        {
            var matCategoryId = _categoryService.getMatressCategory().id;
            return repo.MstCollections.Where(c => c.categoryId == matCategoryId)
                .OrderBy(s => s.collectionCode)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.collectionCode + " (" +
                        s.MstSupplier.code + ")"
                }).ToList();
        }

        public List<VMLookUpItem> getFomCollectionLookUp()
        {
            var fomCategoryId = _categoryService.getFoamCategory().id;
            return repo.MstCollections.Where(c => c.categoryId == fomCategoryId)
                .OrderBy(s => s.collectionCode)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.collectionCode + " (" +
                        s.MstSupplier.code + ")"
                }).ToList();
        }

        public List<VMLookUpItem> getCollectionForGRNByCategorynSupplierId(Int64 categoryId,Int64? supplierId)
        {
            return repo.TrnPurchaseOrderItems.Where(p => p.categoryId == categoryId 
                                                    && p.MstCollection.supplierId == supplierId 
                                                    && (p.status.Equals("Approved") || p.status.Equals("PartialCompleted")))
                .Select(s => new VMLookUpItem
                {
                    value = s.MstCollection.id,
                    label = s.MstCollection.collectionCode + " (" +
                        s.MstCollection.MstSupplier.code + ")"
                }).Distinct().ToList();
        }

        public ResponseMessage postCollection(VMCollection collection)
        {
            MstCollection collectionToPost = Mapper.Map<VMCollection, MstCollection>(collection);
            collectionToPost.createdOn = DateTime.Now;
            collectionToPost.createdBy = _LoggedInuserId;

            repo.MstCollections.Add(collectionToPost);
            repo.SaveChanges();
            return new ResponseMessage(collectionToPost.id,
                resourceManager.GetString("CollectionAdded"), ResponseType.Success);
        }

        public ResponseMessage putCollection(VMCollection collection)
        {
            var collectionToPut = repo.MstCollections.Where(s => s.id == collection.id).FirstOrDefault();
            collectionToPut.categoryId = collection.categoryId;
            collectionToPut.supplierId = collection.supplierId;
            collectionToPut.collectionCode = collection.collectionCode;
            collectionToPut.collectionName = collection.collectionName;
            collectionToPut.purchaseDiscount = collection.purchaseDiscount;
            collectionToPut.description = collection.description;
            collectionToPut.manufacturerName = collection.manufacturerName;
            collectionToPut.updatedBy = _LoggedInuserId;
            collectionToPut.updatedOn = DateTime.Now;
            
            repo.SaveChanges();
            return new ResponseMessage(collection.id, resourceManager.GetString("CollectionUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteCollection(Int64 id)
        {  
            repo.MstCollections.Remove(repo.MstCollections.Where(s => s.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("CollectionDeleted"), ResponseType.Success);
        }

        public int UploadCollections(HttpPostedFileBase file)
        {
            DataTable collectionDataTable = new DataTable();
            collectionDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateDataTable(collectionDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection Code *"].SetOrdinal(1);
            validatedDataTable.Columns["Collection Name *"].SetOrdinal(2);
            validatedDataTable.Columns["Supplier *"].SetOrdinal(3);
            validatedDataTable.Columns["Manufacturer Name"].SetOrdinal(4);
            validatedDataTable.Columns["Purchase Discount (%) *"].SetOrdinal(5);
            validatedDataTable.Columns["Description"].SetOrdinal(6);

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
                    da.SelectCommand = new SqlCommand("UploadCollection", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@CollectionType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Collection Code *") == row["collectionCode"].ToString()).FirstOrDefault());
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
            var model = new VMCollection();

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
                model.categoryId = model.supplierId = 1;
                model.collectionCode = row["Collection Code *"].ToString();
                model.collectionName = row["Collection Name *"].ToString();
                model.manufacturerName = row["Manufacturer Name"].ToString();
                model.purchaseDiscount = !string.IsNullOrWhiteSpace(row["Purchase Discount (%) *"].ToString()) ? Convert.ToDecimal(row["Purchase Discount (%) *"]) : 0;
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

            //finding distinct values of Category, Supplier
            List<string> Category = new List<string>();
            List<string> Supplier = new List<string>();

            Category = rawTable.AsEnumerable().Select(c => c.Field<string>("Category *")).Distinct().ToList();
            Supplier = rawTable.AsEnumerable().Select(c => c.Field<string>("Supplier *")).Distinct().ToList();

            //formatting as comma separated string
            string categoryString = (string.Join(",", Category.ToArray()));
            string supplierString = (string.Join(",", Supplier.ToArray()));

            //fetching data for Category,Supplier
            var categoryKey = repo.GET_CATEGORY_ID(categoryString).ToList();
            var supplierKey = repo.GET_SUPPLIER_ID(supplierString).ToList();

            //replacing values of Supplier, HSN, UOM by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var categoryRow = categoryKey.Where(d => d.code.ToLower().Equals(rawTable.Rows[j]["Category *"].ToString().Trim().ToLower())).FirstOrDefault();
                var supplierRow = supplierKey.Where(d => d.code.ToLower().Equals(rawTable.Rows[j]["Supplier *"].ToString().Trim().ToLower())).FirstOrDefault();
                if (categoryRow != null && supplierRow != null)
                {
                    rawTable.Rows[j]["Category *"] = categoryRow.id;
                    rawTable.Rows[j]["Supplier *"] = supplierRow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify Category, Supplier";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}