using IMSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Reflection;
using IMSWebApi.Common;
using IMSWebApi.ViewModel;
using AutoMapper;
using IMSWebApi.Enums;
using System.Transactions;
using System.Data;
using System.ComponentModel.DataAnnotations;
using IMSWebApi.ViewModel.UploadVM;
using System.Data.SqlClient;
using System.Configuration;

namespace IMSWebApi.Services
{
    public class TrnProductStockDetailService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        TrnProductStockService _trnProductStockService = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public TrnProductStockDetailService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            _trnProductStockService = new TrnProductStockService();
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMTrnProductStockDetailList> getTrnProductStockDetail(int pageSize, int page, string search)
        {
            List<VMTrnProductStockDetailList> trnProductStockDetailView;
            trnProductStockDetailView = repo.TrnProductStockDetails.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.itemCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.itemCode.StartsWith(search)
                    || q.matSizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search): true)
                    .Select(p => new VMTrnProductStockDetailList
                    {
                        id = p.id,
                        categoryName = p.categoryId != null ? p.MstCategory.code : string.Empty,
                        collectionName = p.collectionId != null ? p.MstCollection.collectionCode : string.Empty,
                        serialno = p.fwrShadeId != null ? (p.MstFWRShade.serialNumber + " (" + p.MstFWRShade.shadeCode + "-" +
                                           p.MstFWRShade.MstFWRDesign.designCode + ")") : string.Empty,
                        matSize = p.matSizeId != null ? p.MstMatSize.sizeCode + " (" + p.MstMatSize.MstMatThickness.thicknessCode + "-" + p.MstMatSize.MstQuality.qualityCode + ")"
                                   : p.matSizeCode != null ? p.matSizeCode + " (" + p.MstMatThickness.thicknessCode + "-" + p.MstQuality.qualityCode + ")" : string.Empty,
                        fomItem = p.fomSizeId != null ? p.MstFomSize.itemCode : string.Empty,
                        accessoryCode = p.accessoryId != null ? p.MstAccessory.itemCode : string.Empty,
                        stock = p.stock
                    })
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();

            return new ListResult<VMTrnProductStockDetailList>
            {
                Data = trnProductStockDetailView,
                TotalCount = repo.TrnProductStockDetails.Where(q => !string.IsNullOrEmpty(search)
                    ? q.MstCategory.code.StartsWith(search)
                    || q.MstCollection.collectionCode.StartsWith(search)
                    || q.MstAccessory.itemCode.StartsWith(search)
                    || q.MstFWRShade.serialNumber.ToString().StartsWith(search)
                    || q.MstMatSize.sizeCode.StartsWith(search)
                    || q.MstFomSize.itemCode.StartsWith(search)
                    || q.matSizeCode.StartsWith(search)
                    || q.stock.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMTrnProductStockDetail getTrnProductStockDetailById(Int64 id)
        {
            var result = repo.TrnProductStockDetails.Where(q => q.id == id).FirstOrDefault();
            var trnProductStockDetailView = Mapper.Map<TrnProductStockDetail, VMTrnProductStockDetail>(result);
            return trnProductStockDetailView;
        }

        public ResponseMessage postTrnProductStockDetail(VMTrnProductStockDetail trnProductStockDetail)
        {  
            using (var transaction = new TransactionScope())
            {
                TrnProductStockDetail trnProductStockDetailToPost = Mapper.Map<VMTrnProductStockDetail, TrnProductStockDetail>(trnProductStockDetail);
                trnProductStockDetailToPost.kgPerUnit = trnProductStockDetail.stockInKg!=null ? trnProductStockDetailToPost.stockInKg / trnProductStockDetailToPost.stock : null;
                trnProductStockDetailToPost.createdOn = DateTime.Now;
                trnProductStockDetailToPost.createdBy = _LoggedInuserId;

                repo.TrnProductStockDetails.Add(trnProductStockDetailToPost);

                _trnProductStockService.AddProductInStock(trnProductStockDetail, false,0,null);

                repo.SaveChanges();
                transaction.Complete();
                return new ResponseMessage(trnProductStockDetailToPost.id, resourceManager.GetString("TrnProductStockAdded"), ResponseType.Success);
            }
        }

        public ResponseMessage putTrnProductStockDetail(VMTrnProductStockDetail trnProductStockDetail)
        {
            decimal stock = 0;
            decimal? stockInKg = null;
            var trnProductStockDetailToPut = repo.TrnProductStockDetails.Where(q => q.id == trnProductStockDetail.id).FirstOrDefault();

            trnProductStockDetailToPut.categoryId = trnProductStockDetail.categoryId;
            trnProductStockDetailToPut.collectionId = trnProductStockDetail.collectionId;
            trnProductStockDetailToPut.fomSizeId = trnProductStockDetail.fomSizeId;
            trnProductStockDetailToPut.fwrShadeId = trnProductStockDetail.fwrShadeId;
            trnProductStockDetailToPut.matSizeId = trnProductStockDetail.matSizeId;
            trnProductStockDetailToPut.accessoryId = trnProductStockDetail.accessoryId;

            stock = trnProductStockDetail.stock - trnProductStockDetailToPut.stock;
            stockInKg = trnProductStockDetail.stockInKg - trnProductStockDetailToPut.stockInKg;
            //trnProductStockToPut.locationId = trnProductStock.locationId;
            trnProductStockDetailToPut.stock = trnProductStockDetail.stock;
            trnProductStockDetailToPut.stockInKg = trnProductStockDetail.stockInKg;
            trnProductStockDetailToPut.kgPerUnit = trnProductStockDetailToPut.stock == 0 ? 0 : trnProductStockDetailToPut.stockInKg / trnProductStockDetailToPut.stock;

            trnProductStockDetailToPut.updatedBy = _LoggedInuserId;
            trnProductStockDetailToPut.updatedOn = DateTime.Now;

            _trnProductStockService.AddProductInStock(trnProductStockDetail, true, stock, stockInKg);


            repo.SaveChanges();
            return new ResponseMessage(trnProductStockDetail.id, resourceManager.GetString("TrnProductStockUpdated"), ResponseType.Success);
        }

        public Tuple<string, int> UploadFWRProductStock(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable fwrProductStockDataTable = new DataTable();
            fwrProductStockDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateFWRDataTable(fwrProductStockDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category *"].SetOrdinal(0);
            validatedDataTable.Columns["Location *"].SetOrdinal(1);
            validatedDataTable.Columns["Collection *"].SetOrdinal(2);
            validatedDataTable.Columns["Serial Number *"].SetOrdinal(3);
            validatedDataTable.Columns["Product Stock *"].SetOrdinal(4);

            validatedDataTable.AcceptChanges();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFWRStock", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FWRStockType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Serial Number *") == row["fwrShadeId"].ToString()
                                                                                                    && r.Field<string>("Collection *") == row["collectionId"].ToString()).FirstOrDefault());
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
            if (InvalidData.Rows.Count > 0)
            {
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return new Tuple<string, int>(Invalidfilename, validatedDataTable.Rows.Count);
        }

        public Tuple<string, int> UploadFoamProductStock(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable foamProductStockDataTable = new DataTable();
            foamProductStockDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateFoamDataTable(foamProductStockDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Location *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection *"].SetOrdinal(1);
            validatedDataTable.Columns["Foam Item *"].SetOrdinal(2);
            validatedDataTable.Columns["Product Stock *"].SetOrdinal(3);
            validatedDataTable.Columns["Stock (in KG) *"].SetOrdinal(4);

            validatedDataTable.AcceptChanges();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFoamStock", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@FoamStockType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Foam Item *") == row["fomSizeId"].ToString()
                                                                                                    && r.Field<string>("Collection *") == row["collectionId"].ToString()).FirstOrDefault());
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
            if (InvalidData.Rows.Count > 0)
            {
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return new Tuple<string, int>(Invalidfilename, validatedDataTable.Rows.Count);
        }

        public Tuple<string, int> UploadMattressProductStock(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable mattressProductStockDataTable = new DataTable();
            mattressProductStockDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateMattressDataTable(mattressProductStockDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Location *"].SetOrdinal(0);
            validatedDataTable.Columns["Collection *"].SetOrdinal(1);
            validatedDataTable.Columns["Mattress Size *"].SetOrdinal(2);
            validatedDataTable.Columns["Product Stock *"].SetOrdinal(3);

            validatedDataTable.AcceptChanges();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadMattressStock", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@MattressStockType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Mattress Size *") == row["matSizeId"].ToString()
                                                                                                    && r.Field<string>("Collection *") == row["collectionId"].ToString()).FirstOrDefault());
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
            if (InvalidData.Rows.Count > 0)
            {
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return new Tuple<string, int>(Invalidfilename, validatedDataTable.Rows.Count);
        }

        public Tuple<string, int> UploadAccessoryProductStock(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable accesoryProductStockDataTable = new DataTable();
            accesoryProductStockDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateAccessoryDataTable(accesoryProductStockDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Location *"].SetOrdinal(0);
            validatedDataTable.Columns["Accessory Code*"].SetOrdinal(1);
            validatedDataTable.Columns["Product Stock *"].SetOrdinal(2);

            validatedDataTable.AcceptChanges();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadAccessoryStock", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@AccessoryStockType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Accessory Code*") == row["accessoryId"].ToString()).FirstOrDefault());
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
            if (InvalidData.Rows.Count > 0)
            {
                Invalidfilename = datatable_helper.ConvertToExcel(InvalidData, true);
                Invalidfilename = string.Concat(path, "ExcelUpload\\", Invalidfilename);
            }

            //valid data convert to excel
            datatable_helper.ConvertToExcel(validatedDataTable, false);

            return new Tuple<string, int>(Invalidfilename, validatedDataTable.Rows.Count);
        }

        private DataTable ValidateFWRDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new FWRProductStock();
            decimal outputValue = 0.0M;

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
                model.categoryId = model.collectionId = model.locationId = model.fwrShadeId = 1;
                
                if (!string.IsNullOrWhiteSpace(row["Product Stock *"].ToString()) ? Decimal.TryParse(row["Product Stock *"].ToString(), out outputValue) : false)
                {
                    model.stock = outputValue;
                    outputValue = 0.0M;
                }
               
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

            //finding distinct values of Serial Number
            List<string> Shade = new List<string>();
            Shade = rawTable.AsEnumerable().Select(c => c.Field<string>("Serial Number *")).Distinct().ToList();

            //finding distinct values of location
            List<string> Location = new List<string>();
            Location = rawTable.AsEnumerable().Select(c => c.Field<string>("Location *")).Distinct().ToList();

            //formatting as comma separated string
            string shadeString = (string.Join(",", Shade.ToArray()));

            //formatting as comma separated string
            string locationString = (string.Join(",", Location.ToArray()));

            //fetching data for shade            
            var shadeKey = repo.GET_FWRSHADE_ID(shadeString).ToList();

            //fetching data for location            
            var locationKey = repo.GET_LOCATION_ID(locationString).ToList();

            //replacing values of category, collection, quality and design by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = shadeKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category *"].ToString().Trim().ToLower())
                                    && d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())).FirstOrDefault();

                var locationRow = locationKey.Where(l => l.locationCode.ToLower().Equals(rawTable.Rows[j]["Location *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null && locationRow != null)
                {
                    rawTable.Rows[j]["Category *"] = row.categoryId;
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Serial Number *"] = row.id;
                    rawTable.Rows[j]["Location *"] = locationRow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify Category, Collection, Location and Serial Number";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }

        private DataTable ValidateFoamDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new FoamProductStock();
            decimal outputValue = 0.0M;

            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                InvalidData.Columns.Add(colname);
                col.ColumnName = colname;
            }

            rawTable.Columns.Remove("Category *");

            InvalidData.Columns.Add("Reason");

            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.collectionId = model.locationId = model.fomSizeId = 1;

                if (!string.IsNullOrWhiteSpace(row["Product Stock *"].ToString()) ? Decimal.TryParse(row["Product Stock *"].ToString(), out outputValue) : false)
                {
                    model.stock = outputValue;
                    outputValue = 0.0M;
                }

                if (!string.IsNullOrWhiteSpace(row["Stock (in KG) *"].ToString()) ? Decimal.TryParse(row["Stock (in KG) *"].ToString(), out outputValue) : false)
                {
                    model.stockInKg = outputValue;
                    outputValue = 0.0M;
                }

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

            //finding distinct values of Foam Item
            List<string> FoamItem = new List<string>();
            FoamItem = rawTable.AsEnumerable().Select(c => c.Field<string>("Foam Item *")).Distinct().ToList();

            //finding distinct values of location
            List<string> Location = new List<string>();
            Location = rawTable.AsEnumerable().Select(c => c.Field<string>("Location *")).Distinct().ToList();

            //formatting as comma separated string
            string foamItemString = (string.Join(",", FoamItem.ToArray()));

            //formatting as comma separated string
            string locationString = (string.Join(",", Location.ToArray()));

            //fetching data for Foam Item            
            var foamItemKey = repo.GET_FOAMSIZE_ID(foamItemString).ToList();

            //fetching data for location            
            var locationKey = repo.GET_LOCATION_ID(locationString).ToList();

            //replacing values of category, collection, quality and design by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = foamItemKey.Where(d => d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())
                                            && d.itemCode.ToLower().Equals(rawTable.Rows[j]["Foam Item *"].ToString().Trim().ToLower())).FirstOrDefault();

                var locationRow = locationKey.Where(l => l.locationCode.ToLower().Equals(rawTable.Rows[j]["Location *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null && locationRow != null)
                {
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Foam Item *"] = row.id;
                    rawTable.Rows[j]["Location *"] = locationRow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify Category, Collection, Location and Foam Item";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }

        private DataTable ValidateMattressDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new MattressProductStock();
            decimal outputValue = 0.0M;

            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                InvalidData.Columns.Add(colname);
                col.ColumnName = colname;
            }

            rawTable.Columns.Remove("Category *");

            InvalidData.Columns.Add("Reason");

            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.collectionId = model.locationId = model.matSizeId = 1;

                if (!string.IsNullOrWhiteSpace(row["Product Stock *"].ToString()) ? Decimal.TryParse(row["Product Stock *"].ToString(), out outputValue) : false)
                {
                    model.stock = outputValue;
                    outputValue = 0.0M;
                }

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

            //finding distinct values of Mattress Size
            List<string> MattSizeItem = new List<string>();
            MattSizeItem = rawTable.AsEnumerable().Select(c => c.Field<string>("Mattress Size *")).Distinct().ToList();

            //finding distinct values of location
            List<string> Location = new List<string>();
            Location = rawTable.AsEnumerable().Select(c => c.Field<string>("Location *")).Distinct().ToList();

            //formatting as comma separated string
            string mattSizeString = (string.Join(",", MattSizeItem.ToArray()));

            //formatting as comma separated string
            string locationString = (string.Join(",", Location.ToArray()));

            //fetching data for Mattress Size            
            var mattSizeKey = repo.GET_MATTRESSSIZE_ID(mattSizeString).ToList();

            //fetching data for location            
            var locationKey = repo.GET_LOCATION_ID(locationString).ToList();

            //replacing values of category, collection, quality and design by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = mattSizeKey.Where(d => d.sizeCode.ToLower().Equals(rawTable.Rows[j]["Mattress Size *"].ToString().Trim().ToLower())
                                            && d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection *"].ToString().Trim().ToLower())).FirstOrDefault();

                var locationRow = locationKey.Where(l => l.locationCode.ToLower().Equals(rawTable.Rows[j]["Location *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null && locationRow != null)
                {
                    rawTable.Rows[j]["Collection *"] = row.collectionId;
                    rawTable.Rows[j]["Mattress Size *"] = row.id;
                    rawTable.Rows[j]["Location *"] = locationRow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify Category, Collection, Location and Mattress Size";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }

        private DataTable ValidateAccessoryDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new AccessoryProductStock();
            decimal outputValue = 0.0M;

            //setting column name as its caption name
            foreach (DataColumn col in rawTable.Columns)
            {
                string colname = rawTable.Columns[col.ColumnName].Caption;
                InvalidData.Columns.Add(colname);
                col.ColumnName = colname;
            }

            rawTable.Columns.Remove("Category *");

            InvalidData.Columns.Add("Reason");

            rawTable.AcceptChanges();

            //first validating without keys
            foreach (DataRow row in rawTable.Rows)
            {
                model.accessoryId = model.locationId = 1;

                if (!string.IsNullOrWhiteSpace(row["Product Stock *"].ToString()) ? Decimal.TryParse(row["Product Stock *"].ToString(), out outputValue) : false)
                {
                    model.stock = outputValue;
                    outputValue = 0.0M;
                }

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

            //finding distinct values of Accesories
            List<string> Accesories = new List<string>();
            Accesories = rawTable.AsEnumerable().Select(c => c.Field<string>("Accessory Code*")).Distinct().ToList();

            //finding distinct values of location
            List<string> Location = new List<string>();
            Location = rawTable.AsEnumerable().Select(c => c.Field<string>("Location *")).Distinct().ToList();

            //formatting as comma separated string
            string accessoriesString = (string.Join(",", Accesories.ToArray()));

            //formatting as comma separated string
            string locationString = (string.Join(",", Location.ToArray()));

            //fetching data for Accessories
            var accesoriesKey = repo.GET_ACCESSORY_ID(accessoriesString).ToList();

            //fetching data for location            
            var locationKey = repo.GET_LOCATION_ID(locationString).ToList();

            //replacing values of Accesory Item Code and Location by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = accesoriesKey.Where(d => d.itemCode.ToLower().Equals(rawTable.Rows[j]["Accessory Code*"].ToString().Trim().ToLower())).FirstOrDefault();

                var locationRow = locationKey.Where(l => l.locationCode.ToLower().Equals(rawTable.Rows[j]["Location *"].ToString().Trim().ToLower())).FirstOrDefault();

                if (row != null && locationRow != null)
                {
                    rawTable.Rows[j]["Accessory Code*"] = row.id;
                    rawTable.Rows[j]["Location *"] = locationRow.id;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify Accessory Item Code and Location";
                    rawTable.Rows[j].Delete();
                }
            }

            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}