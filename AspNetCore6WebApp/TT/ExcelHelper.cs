using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.OleDb;

namespace TT
{
    public class ExcelHelper
    {
        public class OleDb
        {
            /// https://gist.github.com/asurovov/1c13f6bddabaceab423c037494542e26
            /// https://zxxcc0001.pixnet.net/blog/post/14758489-%5Bc%23-.net%5D--%E5%A6%82%E4%BD%95-%E5%9C%A8c%23-%E4%BD%BF%E7%94%A8-oledb-%E8%AE%80%E5%8F%96-excel-%28%E4%B8%80%29
            /// https://www.codingame.com/playgrounds/9014/read-write-excel-file-with-oledb-in-c-without-interop
            /// https://stackoverflow.com/questions/43728201/how-to-read-xlsx-and-xls-files-using-c-sharp-and-oledbconnection
            
            //private static string GetConnectionString(string path)
            //{
            //    /// Set HRD=NO;IMEX=1 to avoid crash and missing data
            //    /// https://stackoverflow.com/questions/10102149/what-is-imex-within-oledb-connection-strings
            //    string strFileType = System.IO.Path.GetExtension(path).Trim().ToLower();
            //    if (".xls".Equals(strFileType, StringComparison.OrdinalIgnoreCase))
            //        //return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
            //        return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\"";
            //    else if (".xlsx".Equals(strFileType, StringComparison.OrdinalIgnoreCase))
            //        return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\"";
            //    else
            //        return string.Empty;
            //}

            ///// Get all sheetnames in the Excel file
            ///// Return Value = array of sheet names
            //public static string[]? GetExcelSheetNames(string path)
            //{
            //    if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) == false) return null;
            //    using OleDbConnection cn = new(GetConnectionString(path));
            //    cn.Open();
            //    if (cn.State == ConnectionState.Closed || cn.State == ConnectionState.Broken)
            //    {
            //        return null;
            //    }
            //    DataTable? dt = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //    if (dt == null) return null;
            //    string[] excelSheets = new string[dt.Rows.Count];
            //    int i = 0;
            //    // Add sheet names to the string array
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        if (row != null)
            //        {
            //            string s = row["TABLE_NAME"]?.ToString()?.Trim('\'').Trim('$') ?? string.Empty;
            //            if (!string.IsNullOrWhiteSpace(s))
            //            {
            //                excelSheets[i] = s;
            //            }
            //        }
            //        i++;
            //    }
            //    dt.Dispose();
            //    cn.Close();
            //    cn.Dispose();
            //    return excelSheets;
            //}

            ///// Get the content in the specific sheet of the Excel file
            ///// Returns a specific worksheet as a linq-queryable enumeration
            //public static EnumerableRowCollection<DataRow>? GetWorkSheet(string path, string sheetName)
            //{
            //    if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) == false) return null;
            //    using OleDbDataAdapter da = new(string.Format("SELECT * FROM [{0}$]", sheetName), GetConnectionString(path));
            //    DataTable dt = new();
            //    da.Fill(dt);
            //    return dt.AsEnumerable();
            //}

            ///// Get the content in the first sheet of the Excel file
            ///// Returns a first worksheet as a linq-queryable enumeration
            //public static EnumerableRowCollection<DataRow>? GetWorkSheet(string path)
            //{
            //    if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) == false) return null;
            //    using OleDbConnection cn = new(GetConnectionString(path));
            //    cn.Open();
            //    if (cn.State == ConnectionState.Closed || cn.State == ConnectionState.Broken)
            //    {
            //        return null;
            //    }
            //    DataTable dtSchema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //    if (dtSchema == null || dtSchema.Rows.Count < 1) return null;
            //    string? sheetName = dtSchema.Rows[0]["TABLE_NAME"].ToString()?.Trim('\'').Trim('$');
            //    if (string.IsNullOrWhiteSpace(sheetName)) return null;
            //    return GetWorkSheet(path, sheetName);
            //}
        }
    }
}
