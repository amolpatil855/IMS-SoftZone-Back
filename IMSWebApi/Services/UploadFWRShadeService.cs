using Bytescout.Spreadsheet;
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



        public int uploadCategory(HttpPostedFileBase file)
        {
            DataTable categoryDataTable = new DataTable();
            categoryDataTable = PrepareDataTable(file); //contains raw data table

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
            shadeDataTable = PrepareDataTable(file); //contains raw data table

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

            SqlParameter param = new SqlParameter("@ShadeType", SqlDbType.Structured);
            param.TypeName = "dbo.ShadeType";
            param.Value = validatedDataTable;

            using (WebAPIdbEntities db = new WebAPIdbEntities())
            {
                var i = db.Database.ExecuteSqlCommand("exec dbo.UploadFWRShade @ShadeType", param);
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
        /// preparing data table from excel file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private DataTable PrepareDataTable(HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);

            //var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);

            DataTable dataTable = new DataTable();

            Spreadsheet document = new Spreadsheet();

            string saveFolder = @"D:\uploads"; //Pick a folder on your machine to store the uploaded files

            string filePath1 = Path.Combine(saveFolder, fileName);

            file.SaveAs(filePath1);

            var temp = filePath1;

            //var tempDirectory = filePath;
            var fileExtension = Path.GetExtension(filePath1);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                document.LoadFromFile(filePath1);
                dataTable = document.ExportToDataTable(0, true);
            }
            return dataTable;
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
                model.categoryId = 1;
                model.collectionId = 1;
                model.qualityId = 1;
                model.designId = 1;
                model.shadeCode = row["Shade Code*"].ToString();
                model.shadeName = row["Color*"].ToString();
                model.serialNumber = Convert.ToInt32(row["Serial Number*"]);
                model.description = row["Description "].ToString();
                model.stockReorderLevel = Convert.ToInt32(row["Stock Reorder Level *"]);

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
            for (int i = 0; i < designKey.Count; i++)
            {
                for(int j = i; j < rawTable.Rows.Count && j == i; j++)
                {
                    var row = designKey.Where(d => d.MstCollection.collectionName.Equals(rawTable.Rows[j]["Collection*"].ToString().Trim()) && d.MstQuality.qualityCode.Equals(rawTable.Rows[j]["Quality*"].ToString().Trim()) && d.designCode.Equals(rawTable.Rows[j]["Design*"].ToString().Trim())).FirstOrDefault();
                    if (row != null)
                    {                        
                        rawTable.Rows[i]["Category*"] = row.categoryId;                        
                        rawTable.Rows[i]["Design*"] = row.id;
                        rawTable.Rows[i]["Collection*"] = row.collectionId;
                        rawTable.Rows[i]["Quality*"] = row.qualityId;
                    }
                    else
                    {
                        InvalidData.Rows.Add(rawTable.Rows[i].ItemArray);
                        InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = "Please verify category, collection, design, quality";
                        rawTable.Rows[i].Delete(); 
                    }
                }
            }

            rawTable.AcceptChanges();

            //iterating over data table for validating rows
            //long z = 0;
            //foreach (DataRow row in rawTable.Rows)
            //{
            //    model.categoryId = long.TryParse(row["Category*"].ToString(), out z) ? z : 0;
            //    model.collectionId = long.TryParse(row["Collection*"].ToString(), out z) ? z : 0;
            //    model.qualityId = long.TryParse(row["Quality*"].ToString(), out z) ? z : 0;
            //    model.designId = long.TryParse(row["Design*"].ToString(), out z) ? z : 0;
            //    //model.shadeCode = row["Shade Code*"].ToString();
            //    //model.shadeName = row["Color*"].ToString();
            //    //model.serialNumber = Convert.ToInt32(row["Serial Number*"]);
            //    //model.description = row["Description "].ToString();
            //    //model.stockReorderLevel = Convert.ToInt32(row["Stock Reorder Level *"]);

            //    var context = new ValidationContext(model, null, null);
            //    var result = new List<ValidationResult>();
            //    var isValid = Validator.TryValidateObject(model, context, result, true);

            //    string errormessage = "";
            //    if (!isValid)
            //    {
            //        InvalidData.Rows.Add(row.ItemArray);
                    //if (model.categoryId != 0)
                    //{
                    //    string x = InvalidData.Rows[InvalidData.Rows.Count - 1]["Category"].ToString();
                    //    var category = designKey.Where(a => a.MstCategory.id == Convert.ToInt32(x)).FirstOrDefault();
                    //    InvalidData.Rows[InvalidData.Rows.Count - 1]["Category"] = category.MstCategory.code;
                    //}
                    //if (model.collectionId != 0)
                    //{
                    //    string x = InvalidData.Rows[InvalidData.Rows.Count - 1]["Collection"].ToString();
                    //    var collection = designKey.Where(a => a.MstCollection.id == Convert.ToInt32(x)).FirstOrDefault();
                    //    InvalidData.Rows[InvalidData.Rows.Count - 1]["Collection"] = collection.MstCollection.collectionCode;
                    //}
                    //if (model.qualityId != 0)
                    //{
                    //    string x = InvalidData.Rows[InvalidData.Rows.Count - 1]["Quality"].ToString();
                    //    var quality = designKey.Where(a => a.MstQuality.id == Convert.ToInt32(x)).FirstOrDefault();
                    //    InvalidData.Rows[InvalidData.Rows.Count - 1]["Quality"] = quality.MstQuality.qualityCode;
                    //}
                    //if (model.collectionId != 0)
                    //{
                    //    string x = InvalidData.Rows[InvalidData.Rows.Count - 1]["Design"].ToString();
                    //    var cc = designKey.Where(a => a.id == Convert.ToInt32(x)).FirstOrDefault();
                    //    InvalidData.Rows[InvalidData.Rows.Count - 1]["Design"] = cc.designCode;
                    //}


                    //adding reason of invalid row
            //        for (int i = 0; i < result.Count; i++)
            //        {
            //            errormessage = string.Join(".", string.Join(",", result.ElementAt(i)), errormessage);
            //        }
            //        InvalidData.Rows[InvalidData.Rows.Count - 1]["Reason"] = errormessage;
            //        row.Delete();
            //    }
            //}

            //rawTable.AcceptChanges();
            //InvalidData.AcceptChanges();

            //if contains invalid data then convert to Excel 
            if(InvalidData != null)
            {
                ConvertToExcel(InvalidData, true);
            }

            //valid data convert to excel
            ConvertToExcel(rawTable, false);

            return rawTable;
        }

        /// <summary>
        /// Converting invalid data to excel sheet
        /// </summary>
        /// <param name="invalidData"></param>
        /// <returns></returns>
        private int ConvertToExcel(DataTable dataTable, bool isInvalid)
        {
            //Spreadsheet scc = new Spreadsheet();

            //scc.Workbook.Worksheets.Add("Logs");
            string saveLocation = @"D:\uploads\";
            //string fileName = "excel.xls";
            

            //scc.ImportFromDataTable(dataTable, scc.Workbook.Worksheets.IndexOf("Logs"));
            //scc.SaveAs(saveLocation+ "excel.xls");

            var lines = new List<string>();
            string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                  Select(column => column.ColumnName).
                                  ToArray();

            var header = string.Join(",", columnNames);
            lines.Add(header);

            var valueLines = dataTable.AsEnumerable()
                               .Select(row => string.Join(",", row.ItemArray));
            lines.AddRange(valueLines);

            if (isInvalid)
            {
                File.WriteAllLines(saveLocation + "Invalidexcel.xls", lines);
            }            
            else
            {
                File.WriteAllLines(saveLocation + "validexcel.xls", lines);
            }

            return 0;
        }
    }
}