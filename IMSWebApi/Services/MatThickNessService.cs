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
    public class MatThicknessService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        Int64 _LoggedInuserId;
        ResourceManager resourceManager = null;
        DataTableHelper datatable_helper = new DataTableHelper();

        public MatThicknessService()
        {
            _LoggedInuserId = Convert.ToInt64(HttpContext.Current.User.Identity.GetUserId());
            resourceManager = new ResourceManager("IMSWebApi.App_Data.Resource", Assembly.GetExecutingAssembly());
        }

        public ListResult<VMMatThickness> getMatThickness(int pageSize, int page, string search)
        {
            List<VMMatThickness> matThicknessView;
            if (pageSize > 0)
            {
                var result = repo.MstMatThicknesses.Where(q => !string.IsNullOrEmpty(search) ?
                    q.thicknessCode.StartsWith(search) 
                    || q.size.ToString().StartsWith(search): true)
                    .OrderBy(q => q.id).Skip(page * pageSize).Take(pageSize).ToList();
                matThicknessView = Mapper.Map<List<MstMatThickness>, List<VMMatThickness>>(result);
            }
            else
            {
                var result = repo.MstMatThicknesses.Where(q => !string.IsNullOrEmpty(search) ?
                    q.thicknessCode.StartsWith(search)
                    || q.size.ToString().StartsWith(search) : true).ToList();
                matThicknessView = Mapper.Map<List<MstMatThickness>, List<VMMatThickness>>(result);
            }

            return new ListResult<VMMatThickness>
            {
                Data = matThicknessView,
                TotalCount = repo.MstMatThicknesses.Where(q => !string.IsNullOrEmpty(search) ?
                    q.thicknessCode.StartsWith(search)
                    || q.size.ToString().StartsWith(search) : true).Count(),
                Page = page
            };
        }

        public VMMatThickness getMatThicknessById(Int64 id)
        {
            var result = repo.MstMatThicknesses.Where(q => q.id == id).FirstOrDefault();
            var matThicknessView = Mapper.Map<MstMatThickness, VMMatThickness>(result);
            return matThicknessView;
        }

        public List<VMLookUpItem> getMatThicknessLookUp()
        {
            return repo.MstMatThicknesses
                .OrderBy(s=>s.thicknessCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.thicknessCode }).ToList();
        }

        public List<VMLookUpItem> getMatThicknessLookUpForCustomMat()
        {
            return repo.MstMatThicknesses
                .OrderBy(s => s.thicknessCode)
                .Select(q => new VMLookUpItem { value = q.id, label = q.thicknessCode + " (" +q.size+")"}).ToList();
        }

        public ResponseMessage postMatThickness(VMMatThickness matThickness)
        {
            MstMatThickness matThicknessToPost = Mapper.Map<VMMatThickness, MstMatThickness>(matThickness);
            matThicknessToPost.createdOn = DateTime.Now;
            matThicknessToPost.createdBy = _LoggedInuserId;

            repo.MstMatThicknesses.Add(matThicknessToPost);
            repo.SaveChanges();
            return new ResponseMessage(matThicknessToPost.id, resourceManager.GetString("MatThicknessAdded"), ResponseType.Success);
        }

        public ResponseMessage putMatThickness(VMMatThickness matThickness)
        {
            var matThicknessToPut = repo.MstMatThicknesses.Where(q => q.id == matThickness.id).FirstOrDefault();

            matThicknessToPut = Mapper.Map<VMMatThickness, MstMatThickness>(matThickness, matThicknessToPut);
            matThicknessToPut.updatedBy = _LoggedInuserId;
            matThicknessToPut.updatedOn = DateTime.Now;

            repo.SaveChanges();
            return new ResponseMessage(matThicknessToPut.id, resourceManager.GetString("MatThicknessUpdated"), ResponseType.Success);
        }

        public ResponseMessage deleteMatThickness(Int64 id)
        {
            repo.MstMatThicknesses.Remove(repo.MstMatThicknesses.Where(q => q.id == id).FirstOrDefault());
            repo.SaveChanges();
            return new ResponseMessage(id, resourceManager.GetString("MatThicknessDeleted"), ResponseType.Success);
        }

        public Tuple<string,int> UploadMatThickness(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable matThicknessDataTable = new DataTable();
            matThicknessDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateDataTable(matThicknessDataTable, ref InvalidData); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Thickness Code*"].SetOrdinal(0);
            validatedDataTable.Columns["Size*"].SetOrdinal(1);
            
            validatedDataTable.AcceptChanges();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadMattressThickness", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@MattressThicknessType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Thickness Code*") == row["thicknessCode"].ToString()).FirstOrDefault());
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

        /// <summary>
        /// validate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateDataTable(DataTable rawTable, ref DataTable InvalidData)
        {
            var model = new VMMatThickness();
            
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
                model.thicknessCode = row["Thickness Code*"].ToString();
                model.size = !string.IsNullOrWhiteSpace(row["Size*"].ToString()) ? Convert.ToDecimal(row["Size*"]) : 0;
               
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