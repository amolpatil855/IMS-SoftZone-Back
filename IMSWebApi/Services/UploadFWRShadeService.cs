using Bytescout.Spreadsheet;
using IMSWebApi.Common;
using IMSWebApi.Models;
using IMSWebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace IMSWebApi.Services
{
    public class UploadFWRShadeService
    {
        WebAPIdbEntities repo = new WebAPIdbEntities();
        DataTableHelper datatable_helper = new DataTableHelper();

        public int uploadCategory(HttpPostedFileBase file)
        {
            DataTable categoryDataTable = new DataTable();
            categoryDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            foreach (DataColumn col in categoryDataTable.Columns)
            {
                string colname = categoryDataTable.Columns[col.ColumnName].Caption;
                col.ColumnName = colname;
            }
            categoryDataTable.AcceptChanges();

            SqlParameter param = new SqlParameter("@CategoryType", SqlDbType.Structured);
            param.TypeName = "dbo.CategoryType";
            param.Value = categoryDataTable;

            using (WebAPIdbEntities db = new WebAPIdbEntities())
            {
                var i = db.Database.ExecuteSqlCommand("exec dbo.UploadCategory @CategoryType", param);
            }

            //var res = repo.UploadCategory(categoryDataTable);

            //string cs = ConfigurationManager.ConnectionStrings["testconnection"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(cs))
            //{
            //    SqlCommand cmd = new SqlCommand("UploadCategory", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    SqlParameter param = new SqlParameter();
            //    param.ParameterName = "@CategoryType";
            //    param.Value = categoryDataTable;
            //    cmd.Parameters.Add(param);
            //    con.Open();
            //    var res = cmd.ExecuteReader();
            //    con.Close();
            //}

            return 0;
        }




        /// <summary>
        /// Separation of valid and invalid rows from uploaded file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int UploadShade(HttpPostedFileBase file)
        {
            DataTable shadeDataTable = new DataTable();
            shadeDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            validatedDataTable = ValidateDataTable(shadeDataTable); //contains validated data

            //reordering columns
            validatedDataTable.Columns["Category*"].SetOrdinal(0);
            validatedDataTable.Columns["Collection*"].SetOrdinal(1);
            validatedDataTable.Columns["Quality*"].SetOrdinal(2);
            validatedDataTable.Columns["Design*"].SetOrdinal(3);
            validatedDataTable.Columns["Shade Code*"].SetOrdinal(4);
            validatedDataTable.Columns["Color*"].SetOrdinal(5);
            validatedDataTable.Columns["Serial Number*"].SetOrdinal(6);
            validatedDataTable.Columns["Description "].SetOrdinal(7);
            validatedDataTable.Columns["Stock Reorder Level *"].SetOrdinal(8);

            validatedDataTable.AcceptChanges();

            //var shade = repo.UploadFWRShade(validatedDataTable).ToList();

            SqlParameter param = new SqlParameter("@FWRShadeType", SqlDbType.Structured);
            param.TypeName = "dbo.FWRShadeType";
            param.Value = validatedDataTable;

            using (WebAPIdbEntities db = new WebAPIdbEntities())
            {
                var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRShade @FWRShadeType", param);
            }

            //string cs = ConfigurationManager.ConnectionStrings["testconnection"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(cs))
            //{
            //    SqlCommand cmd = new SqlCommand("UploadFWRShade", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    SqlParameter param = new SqlParameter();
            //    param.ParameterName = "@ShadeType";
            //    param.Value = validatedDataTable;
            //    cmd.Parameters.Add(param);
            //    con.Open();
            //    var res = cmd.ExecuteScalar();
            //    con.Close();
            //}


            return 0;
        }

        /// <summary>
        /// validate data table content
        /// </summary>
        /// <param name="rawTable"></param>
        /// <returns></returns>
        private DataTable ValidateDataTable(DataTable rawTable)
        {
            var model = new VMFWRShade();

            //datatable for invalid rows
            DataTable InvalidData = new DataTable();
            InvalidData.Columns.Add("Category");
            InvalidData.Columns.Add("Collection");
            InvalidData.Columns.Add("Quality");
            InvalidData.Columns.Add("Design");
            InvalidData.Columns.Add("Shade Code");
            InvalidData.Columns.Add("Shade Name");
            InvalidData.Columns.Add("Serial Number");
            InvalidData.Columns.Add("Description");
            InvalidData.Columns.Add("StockReorderLevel");
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
                model.categoryId = model.collectionId = model.qualityId = model.designId = 1;
                model.shadeCode = row["Shade Code*"].ToString();
                model.shadeName = row["Color*"].ToString();
                model.serialNumber = !string.IsNullOrWhiteSpace(row["Serial Number*"].ToString()) ? Convert.ToInt32(row["Serial Number*"]) : 0;
                model.description = row["Description "].ToString();
                model.stockReorderLevel = !string.IsNullOrWhiteSpace(row["Stock Reorder Level *"].ToString()) ? Convert.ToInt32(row["Stock Reorder Level *"]) : 0;

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
            List<string> Design = new List<string>();
            Design = rawTable.AsEnumerable().Select(c => c.Field<string>("Design*")).Distinct().ToList();

            //formatting as comma separated string
            string designString = (string.Join(",", Design.ToArray()));

            //fetching data for design            
            var designKey = repo.GET_DESIGN_ID(designString).ToList();

            //replacing values of category, collection, quality and design by their ids
            //for (int i = 0; i < designKey.Count; i++)
            //{
            //for(int j = i; j < rawTable.Rows.Count && j == i; j++)
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = designKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category*"].ToString().Trim().ToLower())
                                    && d.MstCollection.collectionName.ToLower().Equals(rawTable.Rows[j]["Collection*"].ToString().Trim().ToLower())
                                    && d.MstQuality.qualityCode.ToLower().Equals(rawTable.Rows[j]["Quality*"].ToString().Trim().ToLower())
                                    && d.designCode.ToLower().Equals(rawTable.Rows[j]["Design*"].ToString().Trim().ToLower())).FirstOrDefault();
                if (row != null)
                {
                    rawTable.Rows[j]["Category*"] = row.categoryId;
                    rawTable.Rows[j]["Design*"] = row.id;
                    rawTable.Rows[j]["Collection*"] = row.collectionId;
                    rawTable.Rows[j]["Quality*"] = row.qualityId;
                }
                else
                {
                    InvalidData.Rows.Add(rawTable.Rows[j].ItemArray);
                    InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection, design, quality";
                    rawTable.Rows[j].Delete();
                }
            }
            //}

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