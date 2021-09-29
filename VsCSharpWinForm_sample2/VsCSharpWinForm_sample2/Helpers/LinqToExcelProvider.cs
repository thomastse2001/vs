using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace VsCSharpWinForm_sample2.Helpers
{
    /// Provides linq querying functionality towards Excel (xls) files
    public class LinqToExcelProvider
    {
        /// Gets or sets the Excel filename
        private string FileName { get; set; }
        private FileType ImportFiletype { get; set; }

        public enum FileType
        {
            Xls,
            Xlsx
        }

        /// Template connectionstring for Excel connections
        //private const string ConnectionStringTemplate = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;IMEX=1;"";";
        private const string ConnectionStringTemplate = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;IMEX=1;"";";
        private const string ConnectionStringTemplateXlsx = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1;"";";

        /// Default constructor
        public LinqToExcelProvider(string fileName)
        {
            FileName = fileName;
            string ext = fileName.Substring(fileName.LastIndexOf('.') + 1, fileName.Length - (fileName.LastIndexOf('.') + 1));
            if (ext.ToLower() == "xls") ImportFiletype = FileType.Xls;
            else if (ext.ToLower() == "xlsx") ImportFiletype = FileType.Xlsx;
        }

        public LinqToExcelProvider(string fileName, FileType importFileType)
        {
            FileName = fileName;
            ImportFiletype = importFileType;
        }

        private string GetConnectionString()
        {
            if (ImportFiletype == FileType.Xls)
                return string.Format(ConnectionStringTemplate, FileName);
            else if (ImportFiletype == FileType.Xlsx)
                return string.Format(ConnectionStringTemplateXlsx, FileName);
            else
                return null;
        }

        /// Returns a worksheet as a linq-queryable enumeration
        public EnumerableRowCollection<DataRow> GetWorkSheet(string sheetName)
        {
            using (OleDbDataAdapter da = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", sheetName), GetConnectionString()))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.AsEnumerable();
            }
        }

        public string[] GetExcelSheetNames()
        {
            using (OleDbConnection cn = new OleDbConnection(GetConnectionString()))
            {
                cn.Open();
                DataTable dt = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null) return null;
                string[] excelSheets = new string[dt.Rows.Count];
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString().Trim('\'').Trim('$');
                    i++;
                }
                dt.Dispose();
                cn.Close();
                cn.Dispose();
                return excelSheets;
            }
        }
    }
}
