using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using MvcApp1.Models;

namespace MvcApp1.DAO
{
    public class StudentDao
    {
        private static Student Mapping(DataRow dr)
        {
            object o = DbHandler.GetObjectFromDb(dr, "StudentId");
            int? i = DbHandler.IsMssqlConnected ? (int?)o : (int?)(long?)o;
            if (i == null) return null;
            return new Student()
            {
                StudentId = i.GetValueOrDefault(),
                StudentName = (string)DbHandler.GetObjectFromDb(dr, "StudentName"),
                Age = (int?)DbHandler.GetObjectFromDb(dr, "Age") ?? -1,
                Email = (string)DbHandler.GetObjectFromDb(dr, "Email")
            };
        }

        public static List<Student> GetFullList()
        {
            List<Student> rList = new List<Student>();
            DataTable dt = null;
            string sql = "SELECT StudentId,StudentName,Age,Email FROM Students ORDER BY StudentId";
            try
            {
                if (DbHandler.IsMssqlConnected)
                {
                    dt = DbHandler.MSSQL.SelectDataTable(sql);
                }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql);
                }
                if (dt == null) { return null; }
                foreach (DataRow dr in dt.Rows)
                {
                    Student o = Mapping(dr);
                    if (o != null) rList.Add(o);
                }
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
            return rList;
        }

        /// Get single record by ID.
        public static Student GetRecordById(int id)
        {
            DataTable dt = null;
            string sql = "SELECT StudentId,StudentName,Age,Email FROM Students WHERE StudentId=@StudentId";
            try
            {
                if (DbHandler.IsMssqlConnected)
                {
                    dt = DbHandler.MSSQL.SelectDataTable(sql, new SqlParameter("@StudentId", SqlDbType.Int) { Value = id });
                }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql, new System.Data.SQLite.SQLiteParameter("@StudentId", DbType.Int32) { Value = id });
                }
                if (dt.Rows.Count < 1) return null;
                return Mapping(dt.Rows[0]);
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        //// Get the number of records in database.
        //public static int CountAll()
        //{
        //    string sSql = "SELECT COUNT(*) FROM Students";
        //    return DbHandler.MssqlSelectScalar(CommonDao.ConnectionString, sSql);
        //}

        /// Check if the student name already exists in database.
        /// Return value: True if exists. Otherwise, false.
        public static bool StudentNameExists(string studentName, int studentId)
        {
            string sql = "SELECT COUNT(*) FROM Students WHERE StudentId!=@StudentId AND StudentName=@StudentName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql,
                new SqlParameter("@StudentName", SqlDbType.VarChar) { Value = studentName },
                new SqlParameter("@StudentId", SqlDbType.Int) { Value = studentId })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql,
                new System.Data.SQLite.SQLiteParameter("@StudentName", DbType.AnsiString) { Value = studentName },
                new System.Data.SQLite.SQLiteParameter("@StudentId", DbType.Int32) { Value = studentId }))
                ) > 0;
        }

        public static bool StudentNameExists(string studentName)
        {
            string sql = "SELECT COUNT(*) FROM Students WHERE StudentName=@StudentName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@StudentName", SqlDbType.VarChar) { Value = studentName })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@StudentName", DbType.AnsiString) { Value = studentName }))
                ) > 0;
        }

        /// Return value = number of records affected.
        public static int UpdateRecord(Student std)
        {
            if (std == null) { return -1; }
            string sql = "UPDATE Students SET StudentName=@StudentName,Age=@Age,Email=@Email WHERE StudentId=@StudentId";
            SqlParameter[] array1 = new SqlParameter[]
            {
                new SqlParameter("@StudentId", SqlDbType.Int) { Value = std.StudentId },
                new SqlParameter("@StudentName", SqlDbType.VarChar) { Value = std.StudentName },
                new SqlParameter("@Age", SqlDbType.Int) { Value = std.Age },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(std.Email) }
            };
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@StudentId", SqlDbType.Int) { Value = std.StudentId },
                new SqlParameter("@StudentName", SqlDbType.VarChar) { Value = std.StudentName },
                new SqlParameter("@Age", SqlDbType.Int) { Value = std.Age },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(std.Email) }):
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@StudentId", DbType.Int32) { Value = std.StudentId },
                new System.Data.SQLite.SQLiteParameter("@StudentName", DbType.AnsiString) { Value = std.StudentName },
                new System.Data.SQLite.SQLiteParameter("@Age", DbType.Int32) { Value = std.Age },
                new System.Data.SQLite.SQLiteParameter("@Email", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(std.Email) });
        }

        /// Return Value = number of records affected.
        public static int DeleteRecord(int StudentId)
        {
            string sql = "DELETE FROM Students WHERE StudentId=@StudentId";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@StudentId", SqlDbType.Int) { Value = StudentId }) :
                DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@StudentId", DbType.Int32) { Value = StudentId });
        }

        public static int InsertRecord(Student std)
        {
            if (std == null) { return -1; }
            string sql = "INSERT INTO Students (StudentName,Age,Email) VALUES (@StudentName,@Age,@Email)";
            SqlParameter[] array1 = new SqlParameter[]
            {
                //new SqlParameter("@StudentId", SqlDbType.Int) { Value = std.StudentId },
                new SqlParameter("@StudentName", SqlDbType.VarChar) { Value = std.StudentName },
                new SqlParameter("@Age", SqlDbType.Int) { Value = std.Age },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(std.Email) }
            };
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@StudentName", SqlDbType.VarChar) { Value = std.StudentName },
                new SqlParameter("@Age", SqlDbType.Int) { Value = std.Age },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(std.Email) }):
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@StudentName", DbType.AnsiString) { Value = std.StudentName },
                new System.Data.SQLite.SQLiteParameter("@Age", DbType.AnsiString) { Value = std.Age },
                new System.Data.SQLite.SQLiteParameter("@Email", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(std.Email) });
        }
    }
}