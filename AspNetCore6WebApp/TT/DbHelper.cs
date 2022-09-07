using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace TT
{
    public interface IDbHelper
    {
        object? GetObjectFromDb(DataRow dr, int columnIndex);
        object? GetObjectFromDb(DataRow dr, string fieldName);
        bool? GetBoolFromDb(DataRow dr, int columnIndex);
        bool? GetBoolFromDb(DataRow dr, string fieldName);
        int? GetIntFromDb(DataRow dr, int columnIndex);
        int? GetIntFromDb(DataRow dr, string fieldName);
        DateTime? GetDateTimeFromDb(DataRow dr, int columnIndex);
        DateTime? GetDateTimeFromDb(DataRow dr, string fieldName);
        decimal? GetDecimalFromDb(DataRow dr, int columnIndex);
        decimal? GetDecimalFromDb(DataRow dr, string fieldName);
        string? GetStringFromDb(DataRow dr, int columnIndex);
        string? GetStringFromDb(DataRow dr, string fieldName);

        int? ParseToIntFromDb(DataRow dr, int columnIndex);
        int? ParseToIntFromDb(DataRow dr, string fieldName);
        DateTime? ParseToDateTimeFromDb(DataRow dr, int dateColumnIndex, int timeColumnIndex);
        DateTime? ParseToDateTimeFromDb(DataRow dr, string dateFieldName, string timeFieldName);

    }

    public class DbHelper : IDbHelper
    {
        public DbHelper() { }

        private static bool ShouldReturnNull(DataRow dr, int columnIndex)
        {
            /// Check if the column index is out of range.
            /// Check if it is equal to DBNull.
            return dr == null || columnIndex < 0 || dr.Table.Columns.Count <= columnIndex || DBNull.Value.Equals(dr[columnIndex]);
        }

        private static bool ShouldReturnNull(DataRow dr, string fieldName)
        {
            /// Check if the field name exists.
            /// Check if it is equal to DBNull.
            return string.IsNullOrWhiteSpace(fieldName) || (dr?.Table.Columns.Contains(fieldName) ?? false) == false || DBNull.Value.Equals(dr[fieldName]);
        }

        public object? GetObjectFromDb(DataRow dr, int columnIndex)
        {
            ///// Check if the column index is out of range.
            ///// Check if it is equal to DBNull.
            //if (dr == null || dr.Table.Columns.Count <= columnIndex || DBNull.Value.Equals(dr[columnIndex]))
            //    return null;
            //return dr[columnIndex];
            return ShouldReturnNull(dr, columnIndex) ? null : dr[columnIndex];
        }

        public object? GetObjectFromDb(DataRow dr, string fieldName)
        {
            ///// Check if the field name exists.
            ///// Check if it is equal to DBNull.
            //if (string.IsNullOrWhiteSpace(fieldName) || (dr?.Table.Columns.Contains(fieldName) ?? false) == false || DBNull.Value.Equals(dr[fieldName]))
            //    return null;
            //return dr[fieldName];
            return ShouldReturnNull(dr, fieldName) ? null : dr[fieldName];
        }

        //private static T ConvertToSpecificType<T>(object o)
        //{
        //    Type t = typeof(T);
        //    if (t == typeof(int)) return (T)Convert.ChangeType(o, typeof(int));
        //    if (t == typeof(string)) return (T)Convert.ChangeType(o, typeof(string));
        //    if (t == typeof(DateTime)) return (T)Convert.ChangeType(o, typeof(DateTime));
        //    if (t == typeof(bool)) return (T)Convert.ChangeType(o, typeof(bool));
        //    if (t == typeof(char)) return (T)Convert.ChangeType(o, typeof(char));
        //    if (t == typeof(decimal)) return (T)Convert.ChangeType(o, typeof(decimal));
        //    if (t == typeof(byte)) return (T)Convert.ChangeType(o, typeof(byte));
        //    if (t == typeof(sbyte)) return (T)Convert.ChangeType(o, typeof(sbyte));
        //    if (t == typeof(short)) return (T)Convert.ChangeType(o, typeof(short));
        //    if (t == typeof(ushort)) return (T)Convert.ChangeType(o, typeof(ushort));
        //    if (t == typeof(uint)) return (T)Convert.ChangeType(o, typeof(uint));
        //    if (t == typeof(long)) return (T)Convert.ChangeType(o, typeof(long));
        //    if (t == typeof(ulong)) return (T)Convert.ChangeType(o, typeof(ulong));
        //    if (t == typeof(float)) return (T)Convert.ChangeType(o, typeof(float));
        //    if (t == typeof(double)) return (T)Convert.ChangeType(o, typeof(double));
        //    return (T)o;
        //}

        //private static T? GetFromDb<T>(DataRow dr, int columnIndex) where T : struct
        //{
        //    return ShouldReturnNull(dr, columnIndex) ? null : ConvertToSpecificType<T>(dr[columnIndex]);
        //}

        //private static T? GetFromDb<T>(DataRow dr, string fieldName) where T : struct
        //{
        //    return ShouldReturnNull(dr, fieldName) ? null : ConvertToSpecificType<T>(dr[fieldName]);
        //}

        public bool? GetBoolFromDb(DataRow dr, int columnIndex)
        {
            //var o = GetObjectFromDb(dr, columnIndex);
            //return o == null ? null : Convert.ToBoolean(o);
            return ShouldReturnNull(dr, columnIndex) ? null : Convert.ToBoolean(dr[columnIndex]);
        }

        public bool? GetBoolFromDb(DataRow dr, string fieldName)
        {
            //var o = GetObjectFromDb(dr, fieldName);
            //return o == null ? null : Convert.ToBoolean(o);
            return ShouldReturnNull(dr, fieldName) ? null : Convert.ToBoolean(dr[fieldName]);
        }

        public int? GetIntFromDb(DataRow dr, int columnIndex)
        {
            return ShouldReturnNull(dr, columnIndex) ? null : Convert.ToInt32(dr[columnIndex]);
        }

        public int? GetIntFromDb(DataRow dr, string fieldName)
        {
            return ShouldReturnNull(dr, fieldName) ? null : Convert.ToInt32(dr[fieldName]);
        }

        public DateTime? GetDateTimeFromDb(DataRow dr, int columnIndex)
        {
            return ShouldReturnNull(dr, columnIndex) ? null : Convert.ToDateTime(dr[columnIndex]);
        }

        public DateTime? GetDateTimeFromDb(DataRow dr, string fieldName)
        {
            return ShouldReturnNull(dr, fieldName) ? null : Convert.ToDateTime(dr[fieldName]);
        }

        public decimal? GetDecimalFromDb(DataRow dr, int columnIndex)
        {
            return ShouldReturnNull(dr, columnIndex) ? null : Convert.ToDecimal(dr[columnIndex]);
        }

        public decimal? GetDecimalFromDb(DataRow dr, string fieldName)
        {
            return ShouldReturnNull(dr, fieldName) ? null : Convert.ToDecimal(dr[fieldName]);
        }

        public string? GetStringFromDb(DataRow dr, int columnIndex)
        {
            return ShouldReturnNull(dr, columnIndex) ? null : Convert.ToString(dr[columnIndex]);
        }

        public string? GetStringFromDb(DataRow dr, string fieldName)
        {
            return ShouldReturnNull(dr, fieldName) ? null : Convert.ToString(dr[fieldName]);
        }

        private static int? ParseToIntFromDbCore(object o)
        {
            //string? s = Convert.ToString(o);
            return int.TryParse(Convert.ToString(o), out int i) ? i : null;
        }

        public int? ParseToIntFromDb(DataRow dr, int columnIndex)
        {
            return ShouldReturnNull(dr, columnIndex) ? null : ParseToIntFromDbCore(dr[columnIndex]);
        }

        public int? ParseToIntFromDb(DataRow dr, string fieldName)
        {
            return ShouldReturnNull(dr, fieldName) ? null : ParseToIntFromDbCore(dr[fieldName]);
        }

        public DateTime? ParseToDateTimeFromDb(DataRow dr, int dateColumnIndex, int timeColumnIndex)
        {
            if (ShouldReturnNull(dr, dateColumnIndex)) return null;
            DateTime d = Convert.ToDateTime(dr[dateColumnIndex]);
            if (ShouldReturnNull(dr, timeColumnIndex)) return d;
            DateTime t = Convert.ToDateTime(dr[timeColumnIndex]);
            return new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
        }

        public DateTime? ParseToDateTimeFromDb(DataRow dr, string dateFieldName, string timeFieldName)
        {
            if (ShouldReturnNull(dr, dateFieldName)) return null;
            DateTime d = Convert.ToDateTime(dr[dateFieldName]);
            if (ShouldReturnNull(dr, timeFieldName)) return d;
            DateTime t = Convert.ToDateTime(dr[timeFieldName]);
            return new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
        }
    }
}
