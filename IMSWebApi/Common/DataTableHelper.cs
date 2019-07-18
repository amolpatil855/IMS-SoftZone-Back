using Bytescout.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace IMSWebApi.Common
{
    public class DataTableHelper
    {
        /// <summary>
        /// preparing data table from excel file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public DataTable PrepareDataTable(HttpPostedFileBase file)
        {
            var fileName = string.Concat(
            Path.GetFileNameWithoutExtension(file.FileName),
            DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(file.FileName)
            );

            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), fileName);

            DataTable dataTable = new DataTable();

            Spreadsheet document = new Spreadsheet();

            //string saveFolder = @"D:\uploads"; //Pick a folder on your machine to store the uploaded files

            //string filePath1 = Path.Combine(filePath, fileName);

            file.SaveAs(filePath);

            var temp = filePath;

            //var tempDirectory = filePath;
            var fileExtension = Path.GetExtension(filePath);
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                document.LoadFromFile(filePath);
                dataTable = document.ExportToDataTable(0, true);
            }
            return dataTable;
        }

        /// <summary>
        /// Converting invalid data to excel sheet
        /// </summary>
        /// <param name="invalidData"></param>
        /// <returns></returns>
        public int ConvertToExcel(DataTable dataTable, bool isInvalid)
        {
            //Spreadsheet scc = new Spreadsheet();

            //scc.Workbook.Worksheets.Add("Logs");
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"));
            //string saveLocation = @"D:\uploads\";
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
                File.WriteAllLines(filePath + "/Invalidexcel.xls", lines);
            }
            else
            {
                File.WriteAllLines(filePath + "/validexcel.xls", lines);
            }

            return 0;
        }
    }
}