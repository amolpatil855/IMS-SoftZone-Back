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
    public class PatternService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public PatternService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMPattern> getPatterns(int pageSize, int page, string search)
        {
            List<VMPattern> patternListView;
            var result = repo.MstPatterns.Where(p => !string.IsNullOrEmpty(search)
                    ? p.name.StartsWith(search)
                    || p.fabricHeight.ToString().StartsWith(search)
                    || p.liningHeight.ToString().StartsWith(search)
                    || p.meterPerInch.ToString().StartsWith(search)
                    || p.widthPerInch.ToString().StartsWith(search)
                    || p.setRateForCustomer.ToString().StartsWith(search) : true)
                    .OrderByDescending(o => o.id)
                    .Skip(page * pageSize).Take(pageSize).ToList();

            patternListView = Mapper.Map<List<MstPattern>, List<VMPattern>>(result);

            return new ListResult<VMPattern>
            {
                Data = patternListView,
                TotalCount = repo.MstPatterns.Where(p => !string.IsNullOrEmpty(search)
                    ? p.name.StartsWith(search)
                    || p.fabricHeight.ToString().StartsWith(search)
                    || p.liningHeight.ToString().StartsWith(search)
                    || p.meterPerInch.ToString().StartsWith(search)
                    || p.widthPerInch.ToString().StartsWith(search)
                    || p.setRateForCustomer.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public List<VMLookUpItem> getPatternLookup()
        {
            return repo.MstPatterns.OrderBy(p => p.name)
                .Select(s => new VMLookUpItem
                {
                    value = s.id,
                    label = s.name
                }).ToList();
        }

        public List<VMPattern> getAllPatterns()
        {
            var result = repo.MstPatterns.ToList();
            return Mapper.Map<List<MstPattern>, List<VMPattern>>(result);
        }

        public List<VMPatternDetailsForTailor> getPatternDetailsForTailor()
        {
            return repo.MstPatterns.Select(p => new VMPatternDetailsForTailor
                {
                    patternId = p.id,
                    name = p.name,
                    charge = 0
                }).ToList();
        }

        public VMPattern getPatternById(Int64 id)
        {
            var result = repo.MstPatterns.Where(p => p.id == id).FirstOrDefault();

            VMPattern patternView = Mapper.Map<MstPattern, VMPattern>(result);
            return patternView;
        }

        public ResponseMessage postPattern(VMPattern pattern)
        {
            MstPattern patternToPost = Mapper.Map<VMPattern, MstPattern>(pattern);
            patternToPost.createdOn = DateTime.Now;
            patternToPost.createdBy = _LoggedInuserId;

            repo.MstPatterns.Add(patternToPost);
            repo.SaveChanges();
            return new ResponseMessage(patternToPost.id, resourceManager.GetString("PatternAdded"), ResponseType.Success);
        }

        public ResponseMessage putPattern(VMPattern pattern)
        {
            var patternToPut = repo.MstPatterns.Where(p => p.id == pattern.id).FirstOrDefault();

            patternToPut.name = pattern.name;
            patternToPut.fabricHeight = pattern.fabricHeight;
            patternToPut.liningHeight = pattern.liningHeight;
            patternToPut.woFabricHeight = pattern.woFabricHeight;
            patternToPut.woLiningHeight = pattern.woLiningHeight;
            patternToPut.meterPerInch = pattern.meterPerInch;
            patternToPut.widthPerInch = pattern.widthPerInch;
            patternToPut.setRateForCustomer = pattern.setRateForCustomer;
            patternToPut.horizontalPatch = pattern.horizontalPatch;
            patternToPut.verticalPatch = pattern.verticalPatch;

            patternToPut.updatedBy = _LoggedInuserId;
            patternToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(pattern.id, resourceManager.GetString("PatternUpdated"), ResponseType.Success);
        }

        public ResponseMessage deletePattern(Int64 id)
        {
            repo.MstPatterns.Remove(repo.MstPatterns.Where(p => p.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("PatternDeleted"), ResponseType.Success);
        }

        public int UploadPatterns(HttpPostedFileBase file)
        {
            DataTable patternDataTable = new DataTable();
            patternDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateDataTable(patternDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Name*"].SetOrdinal(0);
            validatedDataTable.Columns["Fabric Height*"].SetOrdinal(1);
            validatedDataTable.Columns["Lining Height*"].SetOrdinal(2);
            validatedDataTable.Columns["Work Order Fabric Height*"].SetOrdinal(3);
            validatedDataTable.Columns["Work Order Lining Height"].SetOrdinal(4);
            validatedDataTable.Columns["Meter Per Inch*"].SetOrdinal(5);
            validatedDataTable.Columns["Width Per Inch"].SetOrdinal(6);
            validatedDataTable.Columns["Set Rate for Customers"].SetOrdinal(7);
            validatedDataTable.Columns["Vertical Patch"].SetOrdinal(8);
            validatedDataTable.Columns["Horizontal Patch"].SetOrdinal(9);

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
                    da.SelectCommand = new SqlCommand("UploadPattern", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@PatternType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Name*") == row["name"].ToString()).FirstOrDefault());
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
            var model = new VMPattern();

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
                model.name = row["Name*"].ToString();
                model.fabricHeight = !string.IsNullOrWhiteSpace(row["Fabric Height*"].ToString()) ? Convert.ToDecimal(row["Fabric Height*"]) : 0;
                model.liningHeight = !string.IsNullOrWhiteSpace(row["Lining Height*"].ToString()) ? Convert.ToDecimal(row["Lining Height*"]) : 0;
                model.woFabricHeight = !string.IsNullOrWhiteSpace(row["Work Order Fabric Height*"].ToString()) ? Convert.ToDecimal(row["Work Order Fabric Height*"]) : 0;
                model.woLiningHeight = !string.IsNullOrWhiteSpace(row["Work Order FabLiningric Height*"].ToString()) ? Convert.ToDecimal(row["Work Order Lining Height*"]) : 0;
                model.meterPerInch = !string.IsNullOrWhiteSpace(row["Meter Per Inch*"].ToString()) ? Convert.ToInt32(row["Meter Per Inch*"]) : 0;
                model.widthPerInch = !string.IsNullOrWhiteSpace(row["Width Per Inch*"].ToString()) ? Convert.ToInt32(row["Width Per Inch*"]) : 0;
                model.setRateForCustomer = !string.IsNullOrWhiteSpace(row["Set Rate for Customers*"].ToString()) ? Convert.ToDecimal(row["Set Rate for Customers*"]) : 0;
                model.verticalPatch = !string.IsNullOrWhiteSpace(row["Vertical Patch"].ToString()) ? Convert.ToInt32(row["Vertical Patch*"]) : 0;
                model.horizontalPatch = !string.IsNullOrWhiteSpace(row["Horizontal Patch*"].ToString()) ? Convert.ToInt32(row["Horizontal Patch*"]) : 0;
                

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

            return rawTable;
        }
    }
}