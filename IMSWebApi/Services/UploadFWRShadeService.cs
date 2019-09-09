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


            return 0;
        }

        /// <summary>
        /// Separation of valid and invalid rows from uploaded file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Tuple<string, int> UploadShade(HttpPostedFileBase file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string Invalidfilename = string.Empty;

            DataTable shadeDataTable = new DataTable();
            shadeDataTable = datatable_helper.PrepareDataTable(file); //contains raw data table

            DataTable validatedDataTable = new DataTable();
            DataTable InvalidData = new DataTable();
            validatedDataTable = ValidateDataTable(shadeDataTable, ref InvalidData); //contains validated data

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

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["IMS"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand("UploadFWRShade", conn);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@ShadeType";
                    param.Value = validatedDataTable;
                    da.SelectCommand.Parameters.Add(param);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "result_name");

                    DataTable dt = ds.Tables["result_name"];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            validatedDataTable.Rows.Remove(validatedDataTable.AsEnumerable().Where(r => r.Field<string>("Serial Number*") == row["serialNumber"].ToString()).FirstOrDefault());
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
            var model = new VMFWRShade();
            int outputValue = 0;

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
                model.categoryId = model.collectionId = model.qualityId = model.designId = 1;
                model.shadeCode = row["Shade Code*"].ToString();
                model.shadeName = row["Color*"].ToString();
                if (!string.IsNullOrWhiteSpace(row["Serial Number*"].ToString()) ? Int32.TryParse(row["Serial Number*"].ToString(), out outputValue) : false)
                {
                    model.serialNumber = outputValue;
                    outputValue = 0;
                }
                model.description = row["Description "].ToString();
                if (!string.IsNullOrWhiteSpace(row["Stock Reorder Level *"].ToString()) ? Int32.TryParse(row["Stock Reorder Level *"].ToString(), out outputValue) : false)
                {
                    model.stockReorderLevel = outputValue;
                    outputValue = 0;
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

            //finding distinct values of Design codes
            List<string> Design = new List<string>();
            Design = rawTable.AsEnumerable().Select(c => c.Field<string>("Design*")).Distinct().ToList();

            //formatting as comma separated string
            string designString = (string.Join(",", Design.ToArray()));

            //fetching data for design            
            var designKey = repo.GET_DESIGN_ID(designString).ToList();

            //replacing values of category, collection, quality and design by their ids
            for (int j = 0; j < rawTable.Rows.Count; j++)
            {
                var row = designKey.Where(d => d.MstCategory.code.ToLower().Equals(rawTable.Rows[j]["Category*"].ToString().Trim().ToLower())
                                    && d.MstCollection.collectionCode.ToLower().Equals(rawTable.Rows[j]["Collection*"].ToString().Trim().ToLower())
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

            rawTable.AcceptChanges();

            return rawTable;
        }
    }
}