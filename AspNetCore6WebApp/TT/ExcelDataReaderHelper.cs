using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using ExcelDataReader;

namespace TT
{
    public interface IExcelDataReaderHelper
    {
        DataSet? GetDataSet(string filepath);
        DataTable? GetDataTable(string filepath, string sheetName);
    }

    public class ExcelDataReaderHelper : IExcelDataReaderHelper
    {
        public DataSet? GetDataSet(string filepath)
        {
            /// https://github.com/ExcelDataReader/ExcelDataReader
            if (string.IsNullOrEmpty(filepath) || System.IO.File.Exists(filepath) == false) return null;
            using var stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            return reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
        }

        public DataTable? GetDataTable(string filepath, string sheetName)
        {
            var ds = GetDataSet(filepath);
            if (ds == null || ds.Tables == null || ds.Tables.Count < 1) return null;
            if (string.IsNullOrWhiteSpace(sheetName)) return ds.Tables[0];
            return ds.Tables.Contains(sheetName) ? ds.Tables[sheetName] : null;
        }
    }
}
