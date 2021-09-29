using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
using System.Data.SqlClient;/// For MSSQL.
//using System.Data.SQLite;/// Need to install System.Data.SQLite in NuGet Package Manager.

namespace VsCSharpWinForm_sample2.Helpers
{
    public class DbHelper
    {
        #region "Core"
        /// Database helper on Microsoft SQL Server, SQLite database.
        /// Updated date: 2020-11-04
        /// This region contains the core methods to access database.
        public static object GetObjectToDb<T>(T input)
        {
            if (input == null) return DBNull.Value;
            return input;
        }

        private static object GetObjectFromDb(System.Data.DataRow dr, string fieldName)
        {
            /// Check if the field name exists.
            /// Check if it is equal to DBNull.
            if (string.IsNullOrEmpty(fieldName) || (dr?.Table.Columns.Contains(fieldName) ?? false) == false || DBNull.Value.Equals(dr[fieldName]))
                return null;
            return dr[fieldName];
        }

        public static char? GetCharFromDb(System.Data.DataRow dr, string fieldName)
        {
            //string s = GetString(dr, fieldName);
            string s = (string)GetObjectFromDb(dr, fieldName);
            if (string.IsNullOrEmpty(s)) return null;
            if (s.Length > 1) s = s.Substring(0, 1);
            if (char.TryParse(s, out char c)) return c;
            return null;
        }

        /// Generic method to return nullable type values.
        /// https://stackoverflow.com/questions/17363937/generic-method-to-return-nullable-type-values
        //public static T? GetValue<T>(DataRow dr, string fieldName) where T : struct
        //{
        //    //if (CheckDbNull(dr, fieldName)) return null;
        //    //return (T?)dr[fieldName];
        //    return (T?)GetObject(dr, fieldName);
        //}

        //public static int? GetInt(DataRow dr, string fieldName)
        //{
        //    //if (int.TryParse(GetString(dr, fieldName), out int i)) return i;
        //    //return vDefault;
        //    if (CheckDbNull(dr, fieldName)) return null;
        //    return (int)dr[fieldName];
        //}

        public static void DisposeDataTable(ref System.Data.DataTable dt)
        {
            if (dt != null) { dt.Dispose(); dt = null; }
        }

        public class MSSQL
        {
            public static string ConnectionString { get; set; } = "";/// @"Server=.\SQLExpress;Database=Db1;User Id=User1;Password=pass1";

            /// https://www.codeproject.com/Tips/423233/How-to-Connect-to-MySQL-Using-Csharp
            public static int ExecuteNonQuery(string sql, params SqlParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) return 0;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand com = new SqlCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
                        return com.ExecuteNonQuery();
                    }
                }
            }

            public static int ExecuteNonQueryWithTransaction(string sql, params SqlParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) return 0;
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    SqlTransaction trans = cn.BeginTransaction();
                    int iReturn = 0;
                    try
                    {
                        using (SqlCommand com = new SqlCommand(sql, cn, trans))
                        {
                            if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
                            iReturn = com.ExecuteNonQuery();
                            trans.Commit();
                        }
                        return iReturn;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            /// Return value = Number of rows affected.
            /// arrayOfSqlItems = Array of SQL items.
            /// KeyValuePair<string, SqlParameter[]> o1 = new KeyValuePair<string, SqlParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SqlParameter[]
            ///        {
            ///           new SqlParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1001 },
            ///           new SqlParameter("@Description", SqlDbType.VarChar) { Value = "test1" }
            ///        }
            ///    );
            /// KeyValuePair<string, SqlParameter[]> o2 = new KeyValuePair<string, SqlParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SqlParameter[]
            ///        {
            ///           new SqlParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1002 },
            ///           new SqlParameter("@Description", SqlDbType.VarChar) { Value = "test2" }
            ///        }
            ///    );
            /// KeyValuePair<string, SqlParameter[]> o3 = new KeyValuePair<string, SqlParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SqlParameter[]
            ///        {
            ///           new SqlParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1003 },
            ///           new SqlParameter("@Description", SqlDbType.VarChar) { Value = "test3" }
            ///        }
            ///    );
            /// Example 1:
            /// int i = Execute(o1, o2);
            /// Example 2:
            /// KeyValuePair<string, SqlParameter[]>[] arrayOfSqlItems = new KeyValuePair<string, SqlParameter[]>[] { o1, o2, o3 };
            /// int i = Execute(arrayOfSqlItems);
            public static int ExecuteNonQuery(params KeyValuePair<string, SqlParameter[]>[] arrayOfSqlItems)
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    if (arrayOfSqlItems == null) return 0;
                    cn.Open();
                    int iReturn = 0;
                    SqlTransaction trans = cn.BeginTransaction();
                    try
                    {
                        foreach (KeyValuePair<string, SqlParameter[]> o in arrayOfSqlItems)
                        {
                            if (string.IsNullOrEmpty(o.Key) == false)/// o.Key is SQL.
                            {
                                using (SqlCommand com = new SqlCommand(o.Key, cn, trans))
                                {
                                    /// o.Value is an array of parameters.
                                    if ((o.Value?.Length ?? 0) > 0) com.Parameters.AddRange(o.Value);
                                    iReturn += com.ExecuteNonQuery();
                                }
                            }
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        iReturn = -2;
                        throw;
                    }
                    return iReturn;
                }
            }

            /// Get a single value from database.
            /// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/obtaining-a-single-value-from-a-database
            /// http://csharp.net-informations.com/data-providers/csharp-sqlcommand-executescalar.htm
            //public static T? ExecuteScalar<T>(string sql) where T : struct { return ExecuteScalar<T>(sql, null); }
            //public static T? ExecuteScalar<T>(string sql, params SqlParameter[] arrayOfParameters) where T : struct
            public static object ExecuteScalar(string sql, params SqlParameter[] arrayOfParameters)
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand com = new SqlCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
                        com.Connection.Open();
                        return com.ExecuteScalar();
                    }
                }
            }

            /// Get a data table from database.
            public static System.Data.DataTable SelectDataTable(string sql, params SqlParameter[] arrayOfParameters)
            {
                System.Data.DataTable dtReturn = new System.Data.DataTable();
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    //using (SqlDataAdapter da = new SqlDataAdapter(sql, cn))
                    //{
                    //    //if (arrayOfParameters != null && arrayOfParameters.Length > 0)
                    //    if ((arrayOfParameters?.Length ?? 0) > 0) da.SelectCommand.Parameters.AddRange(arrayOfParameters);
                    //    cn.Open();
                    //    da.Fill(dtReturn);
                    //}
                    using (SqlCommand com = new SqlCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
                        com.Connection.Open();
                        using (SqlDataReader rdr = com.ExecuteReader())
                        {
                            dtReturn.Load(rdr);
                        }
                    }
                    return dtReturn;
                }
            }

            public static bool CheckConnected()
            {
                try
                {
                    /// https://stackoverflow.com/questions/16171144/how-to-check-for-database-availability
                    using (SqlConnection cn = new SqlConnection(ConnectionString))
                    {
                        cn.Open();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            //public static SqlParameter GetParameter<T>(string parameterName, System.Data.SqlDbType dbType, T value)
            //{
            //    return new SqlParameter(parameterName, dbType) { Value = GetObjectToDb(value) };
            //}
        }

        /// Need to install System.Data.SQLite in NuGet Package Manager.
        /// The NuGet Package Manager will install the below packages automatically.
        /// EntityFramework
        /// System.Data.SQLite.Core
        /// System.Data.SQLite.EF6
        /// System.Data.SQLite.Linq
        /// System.Data.SQLite
        /// 
        /// Create SQLite Database and table
        /// https://stackoverflow.com/questions/15292880/create-sqlite-database-and-table
        /// How to Use and Connect To Sqlite in a Windows Application
        /// https://www.c-sharpcorner.com/UploadFile/5d065a/how-to-use-and-connect-sqlite-in-a-window-application/
        /// Using SQLite in your C# Application
        /// https://www.codeproject.com/Articles/22165/Using-SQLite-in-your-C-Application
        public class SQLite
        {
            private static string _DatabaseFilePath = System.IO.Path.GetFullPath("db.sqlite");
            public static string DatabaseFilePath { get { return _DatabaseFilePath; } set { _DatabaseFilePath = System.IO.Path.GetFullPath(value); } }
            public static string ConnectionStringTemplate = "Version=3;Data Source={0}";

            public static string GetConnectionString()
            {
                //return string.Format(ConnectionStringTemplate, System.IO.Path.GetFileName(DatabaseFilePath));
                return string.Format(ConnectionStringTemplate, DatabaseFilePath);
            }

            ///// Create database file if it does not exist.
            //public static void CreateDatabaseFileIfNotExist()
            //{
            //    if (!System.IO.File.Exists(DatabaseFilePath))
            //    {
            //        string folder = System.IO.Path.GetDirectoryName(DatabaseFilePath);
            //        if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
            //        System.Data.SQLite.SQLiteConnection.CreateFile(DatabaseFilePath);
            //    }
            //}

            //public static int ExecuteNonQuery(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            //{
            //    if (string.IsNullOrEmpty(sql)) return 0;
            //    CreateDatabaseFileIfNotExist();
            //    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
            //    {
            //        cn.Open();
            //        using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
            //        {
            //            if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
            //            return com.ExecuteNonQuery();
            //        }
            //    }
            //}

            //public static int ExecuteNonQueryWithTransaction(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            //{
            //    if (string.IsNullOrEmpty(sql)) return 0;
            //    CreateDatabaseFileIfNotExist();
            //    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
            //    {
            //        cn.Open();
            //        int iReturn = -2;
            //        System.Data.SQLite.SQLiteTransaction trans = cn.BeginTransaction();
            //        try
            //        {
            //            using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn, trans))
            //            {
            //                if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
            //                iReturn = com.ExecuteNonQuery();
            //            }
            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            iReturn = -2;
            //            throw;
            //        }
            //        return iReturn;
            //    }
            //}

            ///// Return value = Number of rows affected.
            ///// sConnectionString = Connection string.
            ///// arrayOfSqlItems = Array of SQL items.
            ///// KeyValuePair<string, SQLiteParameter[]> o1 = new KeyValuePair<string, SQLiteParameter[]>
            /////    (
            /////        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            /////        new SQLiteParameter[]
            /////        {
            /////           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1001 },
            /////           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test1" }
            /////        }
            /////    );
            ///// KeyValuePair<string, SQLiteParameter[]> o2 = new KeyValuePair<string, SQLiteParameter[]>
            /////    (
            /////        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            /////        new SQLiteParameter[]
            /////        {
            /////           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1002 },
            /////           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test2" }
            /////        }
            /////    );
            ///// KeyValuePair<string, SQLiteParameter[]> o3 = new KeyValuePair<string, SQLiteParameter[]>
            /////    (
            /////        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            /////        new SQLiteParameter[]
            /////        {
            /////           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1003 },
            /////           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test3" }
            /////        }
            /////    );
            ///// Example 1:
            ///// int i = ExecuteNonQuery(ConnectionString, o1, o2);
            ///// Example 2:
            ///// KeyValuePair<string, SQLiteParameter[]>[] arrayOfSqlItems = new KeyValuePair<string, SQLiteParameter[]>[] { o1, o2, o3 };
            ///// int i = ExecuteNonQuery(ConnectionString, arrayOfSqlItems);
            //public static int ExecuteNonQuery(params KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>[] arrayOfSqlItems)
            //{
            //    if (arrayOfSqlItems == null) return 0;
            //    CreateDatabaseFileIfNotExist();
            //    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
            //    {
            //        cn.Open();
            //        int iReturn = 0;
            //        System.Data.SQLite.SQLiteTransaction trans = cn.BeginTransaction();
            //        try
            //        {
            //            foreach (KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]> o in arrayOfSqlItems)
            //            {
            //                if (string.IsNullOrEmpty(o.Key) == false)/// o.Key is SQL.
            //                {
            //                    using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(o.Key, cn, trans))
            //                    {
            //                        /// o.Value is array of parameters.
            //                        if ((o.Value?.Length ?? 0) > 0) com.Parameters.AddRange(o.Value);
            //                        iReturn += com.ExecuteNonQuery();
            //                    }
            //                }
            //            }
            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            iReturn = -2;
            //            throw;
            //        }
            //        return iReturn;
            //    }
            //}

            ///// Get a single value from database.
            //public static object ExecuteScalar(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            //{
            //    CreateDatabaseFileIfNotExist();
            //    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
            //    {
            //        using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
            //        {
            //            if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
            //            com.Connection.Open();
            //            return com.ExecuteScalar();
            //        }
            //    }
            //}

            ///// Get a table from database.
            //public static System.Data.DataTable SelectDataTable(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            //{
            //    if (!System.IO.File.Exists(DatabaseFilePath)) return null;
            //    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
            //    {
            //        System.Data.DataTable dtReturn = new System.Data.DataTable();
            //        using (System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(sql, cn))
            //        {
            //            if ((arrayOfParameters?.Length ?? 0) > 0) da.SelectCommand.Parameters.AddRange(arrayOfParameters);
            //            cn.Open();
            //            da.Fill(dtReturn);
            //        }
            //        //using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
            //        //{
            //        //    if ((arrayOfParameters?.Length ?? 0) > 0) com.Parameters.AddRange(arrayOfParameters);
            //        //    com.Connection.Open();
            //        //    using (System.Data.SQLite.SQLiteDataReader rdr = com.ExecuteReader())
            //        //    {
            //        //        dtReturn.Load(rdr);
            //        //    }
            //        //}
            //        return dtReturn;
            //    }
            //}

            ///// Check if table exists.
            ///// Return value = true if exists. Otherwise, false.
            //public static bool TableExists(string tableName)
            //{
            //    return Convert.ToInt32(ExecuteScalar(
            //        "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name",
            //        new System.Data.SQLite.SQLiteParameter("@name", System.Data.DbType.String) { Value = tableName })) > 0;
            //}

            //public static bool CheckConnected()
            //{
            //    try
            //    {
            //        CreateDatabaseFileIfNotExist();
            //        using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
            //        {
            //            cn.Open();
            //            return true;
            //        }
            //    }
            //    catch
            //    {
            //        return false;
            //    }
            //}
        }
        #endregion

        /// Is database connected.
        public static bool IsMssqlConnected()
        {
            try
            {
                int? i = (int?)MSSQL.ExecuteScalar("SELECT 1 FROM SomeTable WHERE SomeKey='a'");
                return i.HasValue;
            }
            catch
            {
                return false;
            }
        }

        public static Models.Student GetStudentFromDataRow(System.Data.DataRow dr)
        {
            return dr == null ? null : new Models.Student()
            {
                StudentId = (long)GetObjectFromDb(dr, "StudentId"),
                UniqueName = (string)GetObjectFromDb(dr, "UniqueName"),
                DisplayName = (string)GetObjectFromDb(dr, "DisplayName"),
                Phone = (string)GetObjectFromDb(dr, "Phone"),
                Email = (string)GetObjectFromDb(dr, "Email"),
                GenderString = (string)GetObjectFromDb(dr, "GenderString"),
                EnrollmentFee = (int?)GetObjectFromDb(dr, "EnrollmentFee"),
                IsNewlyEnrolled = (bool?)GetObjectFromDb(dr, "IsNewlyEnrolled"),
                Birthday = (DateTime?)GetObjectFromDb(dr, "Birthday"),
                CreatedDate = (DateTime?)GetObjectFromDb(dr, "CreatedDate"),
                UpdatedDate = (DateTime?)GetObjectFromDb(dr, "UpdatedDate")
            };
        }

        #region SQLiteExampleRegion
        //private static void CreateStudentTableIfNotExist()
        //{
        //    if (SQLite.TableExists("Student")) return;
        //    int i = SQLite.ExecuteNonQuery(
        //        "CREATE TABLE Student ("
        //        + "StudentId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,"
        //        + "UniqueName varchar(255) UNIQUE NOT NULL,"
        //        + "DisplayName varchar(255),"
        //        + "Phone varchar(255),"
        //        + "Email varchar(255),"
        //        + "GenderString char(1),"
        //        + "EnrollmentFee int DEFAULT 0,"
        //        + "IsNewlyEnrolled bit DEFAULT true,"
        //        + "Birthday datetime,"
        //        + "CreatedDate datetime,"
        //        + "UpdatedDate datetime"
        //        + ")");
        //}

        //public static int InsertStudent(Models.Student o)
        //{
        //    if (o == null) return -1;
        //    List<System.Data.SQLite.SQLiteParameter> list = null;
        //    try
        //    {
        //        CreateStudentTableIfNotExist();
        //        string sql = "INSERT INTO Student (UniqueName,DisplayName,Phone,Email,GenderString,CreatedDate,UpdatedDate"
        //            + (o.EnrollmentFee == null ? "" : ",EnrollmentFee")
        //            + (o.IsNewlyEnrolled == null ? "" : ",IsNewlyEnrolled")
        //            + ") VALUES (@UniqueName,@DisplayName,@Phone,@Email,@GenderString,@CreatedDate,@UpdatedDate"
        //            + (o.EnrollmentFee == null ? "" : ",@EnrollmentFee")
        //            + (o.IsNewlyEnrolled == null ? "" : ",@IsNewlyEnrolled")
        //            + ")";
        //        list = new List<System.Data.SQLite.SQLiteParameter>()
        //        {
        //            new System.Data.SQLite.SQLiteParameter("@UniqueName", System.Data.DbType.String) { Value = GetObjectToDb(o.UniqueName) },
        //            new System.Data.SQLite.SQLiteParameter("@DisplayName", System.Data.DbType.String) { Value = GetObjectToDb(o.DisplayName) },
        //            new System.Data.SQLite.SQLiteParameter("@Phone", System.Data.DbType.String) { Value = GetObjectToDb(o.Phone) },
        //            new System.Data.SQLite.SQLiteParameter("@Email", System.Data.DbType.String) { Value = GetObjectToDb(o.Email) },
        //            new System.Data.SQLite.SQLiteParameter("@GenderString", System.Data.DbType.String) { Value = GetObjectToDb(o.GenderString) },
        //            new System.Data.SQLite.SQLiteParameter("@CreatedDate", System.Data.DbType.DateTime) { Value = GetObjectToDb(o.CreatedDate) },
        //            new System.Data.SQLite.SQLiteParameter("@UpdatedDate", System.Data.DbType.DateTime) { Value = GetObjectToDb(o.UpdatedDate) }
        //        };
        //        if (o.EnrollmentFee.HasValue) list.Add(new System.Data.SQLite.SQLiteParameter("@EnrollmentFee", System.Data.DbType.Int32) { Value = GetObjectToDb(o.EnrollmentFee) });
        //        if (o.IsNewlyEnrolled.HasValue) list.Add(new System.Data.SQLite.SQLiteParameter("@IsNewlyEnrolled", System.Data.DbType.Boolean) { Value = GetObjectToDb(o.IsNewlyEnrolled) });
        //        return SQLite.ExecuteNonQuery(sql, list.ToArray());
        //    }
        //    finally
        //    {
        //        if (list != null)
        //        {
        //            list.Clear();
        //            list = null;
        //        }
        //    }
        //}

        //public static void InsertStudent2(Models.Student o)
        //{
        //    /// https://www.learnentityframeworkcore.com/dbcontext/adding-data
        //    if (o == null) return;
        //    using (Models.MyDbContext db = new Models.MyDbContext())
        //    {
        //        db.Students.Add(o);
        //        db.SaveChanges();
        //    }
        //}

        //public static List<Models.Student> GetStudentList()
        //{
        //    System.Data.DataTable dt = SQLite.SelectDataTable("SELECT * FROM Student ORDER BY StudentId");
        //    if ((dt?.Rows.Count ?? 0) < 1) return null;
        //    else
        //    {
        //        List<Models.Student> list = new List<Models.Student>();
        //        foreach (System.Data.DataRow dr in dt.Rows)
        //        {
        //            Models.Student o = GetStudentFromDataRow(dr);
        //            if (o != null) list.Add(o);
        //        }
        //        return list;
        //    }
        //}

        //public static IQueryable<Models.Student> GetStudentList2()
        //{
        //    Models.MyDbContext db = new Models.MyDbContext();
        //    return from q in db.Students
        //           orderby q.StudentId
        //           select q;
        //}

        //private static void InitializeStudentTableIfNotExists()
        //{
        //    if (DbHelper.SQLite.TableExists("Student")) return;
        //    DateTime tRef = DateTime.Now;
        //    List<Models.Student> studentList = new List<Models.Student>
        //    {
        //        new Models.Student()
        //        {
        //            UniqueName = "apple.chan",
        //            DisplayName = "Apple Chan",
        //            Phone = "11111111",
        //            Email = "apple.chan@abc.com",
        //            Gender = 'F',
        //            CreatedDate = tRef,
        //            UpdatedDate = tRef
        //        },
        //        new Models.Student()
        //        {
        //            UniqueName = "orange.lee",
        //            DisplayName = "Orange Lee",
        //            Phone = "22222222",
        //            Email = "orange.lee@abc.com",
        //            Gender = 'M',
        //            EnrollmentFee = 100,
        //            IsNewlyEnrolled = false,
        //            CreatedDate = tRef,
        //            UpdatedDate = tRef
        //        }
        //    };
        //    studentList.ForEach(x => InsertStudent(x));
        //}

        //public static void InitializeSqliteDb()
        //{
        //    InitializeStudentTableIfNotExists();
        //}
        #endregion
    }
}
