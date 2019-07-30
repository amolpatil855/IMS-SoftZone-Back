﻿using IMSWebApi.Models;
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
using System.Configuration;

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

        public string UploadFWRQualityFlatRate(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable fwrQualityFlatRateDatatable = new DataTable();
            fwrQualityFlatRateDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateFWRQualityFlatDT(fwrQualityFlatRateDatatable, ref InvalidData); //contains validated data

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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFWRQualityFlatRate", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FWRQualityFlatRateType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Code* ") == row["qualityCode"].ToString()).FirstOrDefault());
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
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return Invalidfilename;
        }

        public string UploadFWRQualityCutRoleRate(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty; 

            DataTable fwrQualityCutRoleRateDatatable = new DataTable();
            fwrQualityCutRoleRateDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateFWRQualityCutRoleDT(fwrQualityCutRoleRateDatatable, ref InvalidData); //contains validated data

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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFWRQualityCutRoleRate", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FWRQualityCutRoleRateType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Code* ") == row["qualityCode"].ToString()).FirstOrDefault());
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
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return Invalidfilename;
        }

        public string UploadMattressQuality(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable mattressQualityDatatable = new DataTable();
            mattressQualityDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateMattressQualityDT(mattressQualityDatatable, ref InvalidData); //contains validated data

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

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadMattressQuality", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@MattressQualityType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Quality Code *") == row["qualityCode"].ToString()).FirstOrDefault());
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
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return Invalidfilename;
        }

        public string UploadFoamQuality(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable foamQualityDatatable = new DataTable();
            foamQualityDatatable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable InvalidData = new DataTable();

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateFoamQualityDT(foamQualityDatatable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection *"].SetOrdinal(1);
            validatedDataTable.Columns["Quality Code *"].SetOrdinal(2);
            validatedDataTable.Columns["Quality Name *"].SetOrdinal(3);
            validatedDataTable.Columns["HSN Code *"].SetOrdinal(4);
            validatedDataTable.Columns["Description "].SetOrdinal(5);
            validatedDataTable.Columns["Max. Discount% *"].SetOrdinal(6);

            validatedDataTable.AcceptChanges();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFoamQuality", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FoamQualityType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Quality Code *") == row["qualityCode"].ToString()).FirstOrDefault());
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
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return Invalidfilename;
        }

        /// <summary>
        /// validate FWR Quality Wirh Flat Rate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateFWRQualityFlatDT(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMFWRQualityFlatUpload();

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

            return rawTable;
        }

        /// <summary>
        /// validate FWR Quality with Cut Role Rate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateFWRQualityCutRoleDT(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMFWRQualityCutRoleUpload();

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

            return rawTable;
        }

        /// <summary>
        /// validate Mattress Quality data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateMattressQualityDT(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMMattressQualityUpload();

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

            return rawTable;
        }

        /// <summary>
        /// validate Mattress Quality data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateFoamQualityDT(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMFoamQualityUpload();

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

            return rawTable;
        }
    }
}