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
using IMSWebApi.ViewModel.UploadVM;

namespace IMSWebApi.Services
{
    public class QualityService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public QualityService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMQualityList> getQualities(int pageSize, int page, string search)
        {
            List<VMQualityList> qualityListView;
            qualityListView = repo.MstQualities.Where(q => !string.IsNullOrEmpty(search)
                   ? q.MstCategory.code.StartsWith(search)
                   || q.MstCollection.collectionCode.StartsWith(search)
                   || q.qualityCode.StartsWith(search)
                   || q.qualityName.StartsWith(search)
                   || q.MstHsn.hsnCode.StartsWith(search) : true)
                   .Select(q => new VMQualityList
                   {
                       id = q.id,
                       categoryId = q.categoryId,
                       categoryCode = q.MstCategory != null ? q.MstCategory.code : string.Empty,
                       collectionCode = q.MstCollection != null ? q.MstCollection.collectionCode : string.Empty,
                       qualityCode = q.qualityCode,
                       qualityName = q.qualityName,
                       hsnCode = q.MstHsn != null ? q.MstHsn.hsnCode : string.Empty
                   })
                   .OrderBy(q => q.categoryId).ThenBy(q => q.collectionCode)
                   .Skip(page * pageSize).Take(pageSize).ToList();

             return new ListResult<VMQualityList>
            {
                Data = qualityListView,
                TotalCount = repo.MstQualities.Where(q => !string.IsNullOrEmpty(search)
                     ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.qualityCode.StartsWith(search)
                    || q.qualityName.StartsWith(search)
                    || q.MstHsn.hsnCode.StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMQuality getQualityById(Int64 id)
        {
            var result = repo.MstQualities.Where(q => q.id == id).FirstOrDefault();
            var qualityView = Mapper.Map<MstQuality, VMQuality>(result);
            return qualityView;
        }

        public List<VMLookUpItem> getQualityLookUpByCollection(Int64 collectionId)
        {
            return repo.MstQualities.Where(q => q.collectionId == collectionId)
                .OrderBy(q=>q.qualityCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.qualityCode }).ToList();
        }

        public List<VMLookUpItem> getQualityLookUpForSO(Int64 collectionId)
        {
            return repo.MstQualities.Where(q => q.collectionId == collectionId && q.flatRate != null)
                .OrderBy(q => q.qualityCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.qualityCode }).ToList();
        }

        public ResponseMessage postQuality(VMQuality Quality)
        {
            MstQuality QualityToPost = Mapper.Map<VMQuality, MstQuality>(Quality);
            QualityToPost.createdOn = DateTime.Now;
            QualityToPost.createdBy = _LoggedInuserId;

            repo.MstQualities.Add(QualityToPost);
            repo.SaveChanges();
            return new ResponseMessage(QualityToPost.id, resourceManager.GetString("QualityAdded"), ResponseType.Success);
        }

        public ResponseMessage putQuality(VMQuality quality)
        {
            var qualityToPut = repo.MstQualities.Where(q => q.id == quality.id).FirstOrDefault();
            //MstCategory qualityCategory = qualityToPut.MstCategory;
            //MstCollection qualityCollection = qualityToPut.MstCollection;

            //qualityToPut = Mapper.Map<VMQuality, MstQuality>(quality, qualityToPut);
            //qualityToPut.MstCategory = qualityCategory;
            //qualityToPut.MstCollection = qualityCollection;
            qualityToPut.categoryId = quality.categoryId;
            qualityToPut.collectionId = quality.collectionId;
            qualityToPut.qualityCode = quality.qualityCode;
            qualityToPut.qualityName = quality.qualityName;
            qualityToPut.description = quality.description;
            qualityToPut.width = quality.width;
            qualityToPut.size = quality.size;
            qualityToPut.hsnId = quality.hsnId;
            qualityToPut.cutRate = quality.cutRate;
            qualityToPut.roleRate = quality.roleRate;
            qualityToPut.rrp = quality.rrp;
            qualityToPut.maxCutRateDisc = quality.maxCutRateDisc;
            qualityToPut.maxRoleRateDisc = quality.maxRoleRateDisc;
            qualityToPut.flatRate = quality.flatRate;
            qualityToPut.purchaseFlatRate = quality.purchaseFlatRate;
            qualityToPut.maxFlatRateDisc = quality.maxFlatRateDisc;
            qualityToPut.custRatePerSqFeet = quality.custRatePerSqFeet;
            qualityToPut.maxDiscount = quality.maxDiscount;
            qualityToPut.updatedBy = _LoggedInuserId;
            qualityToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(quality.id, resourceManager.GetString("QualityUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteQuality(Int64 id)
        {
            repo.MstQualities.Remove(repo.MstQualities.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("QualityDeleted"), ResponseType.Success);
        }

        public int UploadFWRQualityFlatRate(HttpPostedFileBase file)
        {
            DataTable fwrQualityFlatRateDatatable = new DataTable();
            fwrQualityFlatRateDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateFWRQualityFlatDT(fwrQualityFlatRateDatatable); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category*"].SetOrdinal(0);
            validatedDataTable.Columns["Collection * "].SetOrdinal(1);
            validatedDataTable.Columns["Code* "].SetOrdinal(2);
            validatedDataTable.Columns["Name* "].SetOrdinal(3);
            validatedDataTable.Columns["HSN Code*"].SetOrdinal(4);
            validatedDataTable.Columns["Description "].SetOrdinal(5);
            validatedDataTable.Columns["Width* "].SetOrdinal(6);
            validatedDataTable.Columns["Flat Rate * "].SetOrdinal(7);
            validatedDataTable.Columns["Purchase Flat Rate * "].SetOrdinal(8);
            validatedDataTable.Columns["Flat Rate Max Dis.%* "].SetOrdinal(8);

            validatedDataTable.AcceptChanges();

            //Execute Stored Procedure Here
            //SqlParameter param = new SqlParameter("@FWRQualityFlatRateType", SqlDbType.Structured);
            //param.TypeName = "dbo.FWRQualityFlatRateType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRQualityFlatRate @FWRQualityFlatRateType", param);
            //}

            return 0;
        }

        public int UploadFWRQualityCutRoleRate(HttpPostedFileBase file)
        {
            DataTable fwrQualityCutRoleRateDatatable = new DataTable();
            fwrQualityCutRoleRateDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateFWRQualityCutRoleDT(fwrQualityCutRoleRateDatatable); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category * "].SetOrdinal(0);
            validatedDataTable.Columns["Collection * "].SetOrdinal(1);
            validatedDataTable.Columns["Code* "].SetOrdinal(2);
            validatedDataTable.Columns["Name* "].SetOrdinal(3);
            validatedDataTable.Columns["HSN Code*"].SetOrdinal(4);
            validatedDataTable.Columns["Description "].SetOrdinal(5);
            validatedDataTable.Columns["Width* "].SetOrdinal(6);
            validatedDataTable.Columns["Cut Rate* "].SetOrdinal(7);
            validatedDataTable.Columns["Role Rate* "].SetOrdinal(8);
            validatedDataTable.Columns["Cut Rate Max Dis.%*"].SetOrdinal(9);
            validatedDataTable.Columns["Role Rate Max Dis.%* "].SetOrdinal(10);
            validatedDataTable.Columns["RRP* "].SetOrdinal(11);

            validatedDataTable.AcceptChanges();

            //Execute Stored Procedure Here
            //SqlParameter param = new SqlParameter("@FWRQualityFlatRateType", SqlDbType.Structured);
            //param.TypeName = "dbo.FWRQualityFlatRateType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRQualityFlatRate @FWRQualityFlatRateType", param);
            //}

            return 0;
        }

        public int UploadMattressQuality(HttpPostedFileBase file)
        {
            DataTable mattressQualityDatatable = new DataTable();
            mattressQualityDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateMattressQualityDT(mattressQualityDatatable); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection *"].SetOrdinal(1);
            validatedDataTable.Columns["Quality Code *"].SetOrdinal(2);
            validatedDataTable.Columns["Quality Name *"].SetOrdinal(3);
            validatedDataTable.Columns["HSN Code *"].SetOrdinal(4);
            validatedDataTable.Columns["Description "].SetOrdinal(5);
            validatedDataTable.Columns["Max. Discount% *"].SetOrdinal(6);
            validatedDataTable.Columns["Cust Rate/Sq.Mtr * "].SetOrdinal(7);
            
            validatedDataTable.AcceptChanges();

            //Execute Stored Procedure Here
            //SqlParameter param = new SqlParameter("@FWRQualityFlatRateType", SqlDbType.Structured);
            //param.TypeName = "dbo.FWRQualityFlatRateType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRQualityFlatRate @FWRQualityFlatRateType", param);
            //}

            return 0;
        }

        public int UploadFoamQuality(HttpPostedFileBase file)
        {
            DataTable foamQualityDatatable = new DataTable();
            foamQualityDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateFoamQualityDT(foamQualityDatatable); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection *"].SetOrdinal(1);
            validatedDataTable.Columns["Quality Code *"].SetOrdinal(2);
            validatedDataTable.Columns["Quality Name *"].SetOrdinal(3);
            validatedDataTable.Columns["HSN Code *"].SetOrdinal(4);
            validatedDataTable.Columns["Description "].SetOrdinal(5);
            validatedDataTable.Columns["Max. Discount% *"].SetOrdinal(6);

            validatedDataTable.AcceptChanges();

            //Execute Stored Procedure Here
            //SqlParameter param = new SqlParameter("@FWRQualityFlatRateType", SqlDbType.Structured);
            //param.TypeName = "dbo.FWRQualityFlatRateType";
            //param.Value = validatedDataTable;

            //using (WebAPIdbEntities db = new WebAPIdbEntities())
            //{
            //    var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRQualityFlatRate @FWRQualityFlatRateType", param);
            //}

            return 0;
        }

        /// <summary>
        /// validate FWR Quality Wirh Flat Rate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateFWRQualityFlatDT(DataTable rawTable)
        {
            var model = new VMFWRQualityFlatUpload();

            //datatable for invalid rows
            DataTable InvalidData = new DataTable();
            InvalidData.Columns.Add("Category *");
            InvalidData.Columns.Add("Collection *");
            InvalidData.Columns.Add("Code* ");
            InvalidData.Columns.Add("Name* ");
            InvalidData.Columns.Add("HSN Code*");
            InvalidData.Columns.Add("Description ");
            InvalidData.Columns.Add("Width* ");
            InvalidData.Columns.Add("Flat Rate * ");
            InvalidData.Columns.Add("Purchase Flat Rate * ");
            InvalidData.Columns.Add("Flat Rate Max Dis.%* ");
            InvalidData.Columns.Add("Reason");


            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                col.ColumnName = colname;
            }

            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.categoryId = model.collectionId = model.hsnId = 1;
                model.qualityCode = row["Code* "].ToString();
                model.qualityName = row["Name* "].ToString();
                model.description = row["Description "].ToString();
                model.width = Convert.ToDecimal(row["Width* "].ToString());
                model.flatRate = Convert.ToDecimal(row["Flat Rate * "].ToString());
                model.purchaseFlatRate = Convert.ToDecimal(row["Purchase Flat Rate * "].ToString());
                model.maxFlatRateDisc = Convert.ToDecimal(row["Flat Rate Max Dis.%* "].ToString());

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

            //finding distinct values of Design codes
            List<string> Collection = new List<string>();
            Collection = rawTable.AsEnumerable().Select(c => c.Field<string>("Collection * ")).Distinct().ToList();

            //formatting as comma separated string
            string collectionString = (string.Join(",", Collection.ToArray()));

            //fetching data for design            
            var collectionKey = repo.GET_COLLECTION_ID(collectionString).ToList();

            //replacing values of category, collection, quality by their ids         
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = collectionKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Category *"] = row.categoryId;
                    rawTable.Rows[j]["Collection *"] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection";
                    rawTable.Rows[j].Delete();
                }
            }


            rawTable.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(rawTable, false);

            return rawTable;
        }

        /// <summary>
        /// validate FWR Quality with Cut Role Rate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateFWRQualityCutRoleDT(DataTable rawTable)
        {
            var model = new VMFWRQualityCutRoleUpload();

            //datatable for invalid rows
            DataTable InvalidData = new DataTable();

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
                model.categoryId = model.collectionId = model.hsnId = 1;
                model.qualityCode = row["Code* "].ToString();
                model.qualityName = row["Name* "].ToString();
                model.description = row["Description "].ToString();
                model.width = Convert.ToDecimal(row["Width* "].ToString());
                model.cutRate = Convert.ToDecimal(row["Cut Rate* "].ToString());
                model.roleRate = Convert.ToDecimal(row["Role Rate* "].ToString());
                model.rrp = Convert.ToDecimal(row["RRP* "].ToString());
                model.maxCutRateDisc = Convert.ToDecimal(row["Cut Rate Max Dis.%*"].ToString());
                model.maxRoleRateDisc = Convert.ToDecimal(row["Role Rate Max Dis.%* "].ToString());

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

            //finding distinct values of Design codes
            List<string> Collection = new List<string>();
            Collection = rawTable.AsEnumerable().Select(c => c.Field<string>("Collection * ")).Distinct().ToList();

            //formatting as comma separated string
            string collectionString = (string.Join(",", Collection.ToArray()));

            //fetching data for design            
            var collectionKey = repo.GET_COLLECTION_ID(collectionString).ToList();

            //replacing values of category, collection, quality by their ids         
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = collectionKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category * "].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Category * "] = row.categoryId;
                    rawTable.Rows[j]["Collection * "] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection";
                    rawTable.Rows[j].Delete();
                }
            }


            rawTable.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(rawTable, false);

            return rawTable;
        }

        /// <summary>
        /// validate Mattress Quality data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateMattressQualityDT(DataTable rawTable)
        {
            var model = new VMMattressQualityUpload();

            //datatable for invalid rows
            DataTable InvalidData = new DataTable();

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
                model.categoryId = model.collectionId = model.hsnId = 1;
                model.qualityCode = row["Quality Code *"].ToString();
                model.qualityName = row["Quality Name *"].ToString();
                model.description = row["Description "].ToString();
                model.maxDiscount = Convert.ToDecimal(row["Max. Discount% *"].ToString());
                model.custRatePerSqFeet = Convert.ToDecimal(row["Cust Rate/Sq.Mtr * "].ToString());
                
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

            //finding distinct values of Design codes
            List<string> Collection = new List<string>();
            Collection = rawTable.AsEnumerable().Select(c => c.Field<string>("Collection *")).Distinct().ToList();

            //formatting as comma separated string
            string collectionString = (string.Join(",", Collection.ToArray()));

            //fetching data for design            
            var collectionKey = repo.GET_COLLECTION_ID(collectionString).ToList();

            //replacing values of category, collection, quality by their ids         
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = collectionKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Category *"] = row.categoryId;
                    rawTable.Rows[j]["Collection *"] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection";
                    rawTable.Rows[j].Delete();
                }
            }


            rawTable.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(rawTable, false);

            return rawTable;
        }

        /// <summary>
        /// validate Mattress Quality data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateFoamQualityDT(DataTable rawTable)
        {
            var model = new VMFoamQualityUpload();

            //datatable for invalid rows
            DataTable InvalidData = new DataTable();

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
                model.categoryId = model.collectionId = model.hsnId = 1;
                model.qualityCode = row["Quality Code *"].ToString();
                model.qualityName = row["Quality Name *"].ToString();
                model.description = row["Description "].ToString();
                model.maxDiscount = Convert.ToDecimal(row["Max. Discount% *"].ToString());

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

            //finding distinct values of Design codes
            List<string> Collection = new List<string>();
            Collection = rawTable.AsEnumerable().Select(c => c.Field<string>("Collection *")).Distinct().ToList();

            //formatting as comma separated string
            string collectionString = (string.Join(",", Collection.ToArray()));

            //fetching data for design            
            var collectionKey = repo.GET_COLLECTION_ID(collectionString).ToList();

            //replacing values of category, collection, quality by their ids         
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = collectionKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null)
                {
                    rawTable.Rows[j]["Category *"] = row.categoryId;
                    rawTable.Rows[j]["Collection *"] = row.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection";
                    rawTable.Rows[j].Delete();
                }
            }


            rawTable.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if (InvalidData != null)
            {
                datatable_helper.ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(rawTable, false);

            return rawTable;
        }
    }
}