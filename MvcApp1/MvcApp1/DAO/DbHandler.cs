using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class DbHandler
    {
        public static bool IsMssqlConnected { get; private set; }

        public static object GetObjectToDb<T>(T input)
        {
            if (input == null) { return DBNull.Value; }
            return input;
        }

        public static object GetObjectFromDb(DataRow dr, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || (dr?.Table.Columns.Contains(fieldName) ?? false) == false || DBNull.Value.Equals(dr[fieldName]))
            { return null; }
            return dr[fieldName];
        }

        //// Return value = True if it is null. Otherwise, false.
        //private static bool CheckDbNull(DataRow dr, string fieldName)
        //{
        //    return dr == null || string.IsNullOrWhiteSpace(fieldName) || dr.Table.Columns.Contains(fieldName) == false || DBNull.Value.Equals(dr[fieldName]);
        //}

        //// Return default value if the input is DBNull.
        //public static string GetString(object vObject) { return GetString(vObject, null); }
        //public static string GetString(object vObject, string vDefault)
        //{
        //    if (DBNull.Value.Equals(vObject)) { return vDefault; }
        //    return vObject.ToString();
        //}
        //public static string GetString(DataRow dr, string fieldName) { return GetString(dr, fieldName, null); }
        //public static string GetString(DataRow dr, string fieldName, string vDefault)
        //{
        //    if (CheckDbNull(dr, fieldName)) { return vDefault; }
        //    return dr[fieldName].ToString();
        //}

        //public static int? GetInt(object vObject) { return GetInt(vObject, null); }
        //public static int? GetInt(object vObject, int? vDefault)
        //{
        //    if (int.TryParse(GetString(vObject), out int i)) { return i; }
        //    return vDefault;
        //}
        //public static int? GetInt(DataRow dr, string fieldName) { return GetInt(dr, fieldName, null); }
        //public static int? GetInt(DataRow dr, string fieldName, int? vDefault)
        //{
        //    //if (int.TryParse(GetString(dr, fieldName), out int i)) { return i; }
        //    //return vDefault;
        //    if (CheckDbNull(dr, fieldName)) { return vDefault; }
        //    return (int)dr[fieldName];
        //}

        //public static bool? GetBool(object vObject) { return GetBool(vObject, null); }
        //public static bool? GetBool(object vObject, bool? vDefault)
        //{
        //    if (bool.TryParse(GetString(vObject), out bool b)) { return b; }
        //    return vDefault;
        //}
        //public static bool? GetBool(DataRow dr, string fieldName) { return GetBool(dr, fieldName, null); }
        //public static bool? GetBool(DataRow dr, string fieldName, bool? vDefault)
        //{
        //    if (CheckDbNull(dr, fieldName)) { return vDefault; }
        //    return (bool)dr[fieldName];
        //}

        //public static DateTime? GetDateTime(object vObject) { return GetDateTime(vObject, null); }
        //public static DateTime? GetDateTime(object vObject, DateTime? vDefault)
        //{
        //    if (DateTime.TryParse(GetString(vObject), out DateTime dt)) { return dt; }
        //    return vDefault;
        //}
        //public static DateTime? GetDateTime(DataRow dr, string fieldName) { return GetDateTime(dr, fieldName, null); }
        //public static DateTime? GetDateTime(DataRow dr, string fieldName, DateTime? vDefault)
        //{
        //    if (CheckDbNull(dr, fieldName)) { return vDefault; }
        //    return (DateTime)dr[fieldName];
        //}

        public static void DisposeDataTable(ref DataTable dt)
        {
            if (dt != null) { dt.Dispose(); dt = null; }
        }

        //private static void DisposeSqlDataAdapter(ref SqlDataAdapter da)
        //{
        //    try
        //    {
        //        if (da != null)
        //        {
        //            da.Dispose();
        //            da = null;
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        //private static void DisposeSqlCommand(ref SqlCommand com)
        //{
        //    try
        //    {
        //        if (com != null)
        //        {
        //            com.Dispose();
        //            com = null;
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        //private static void DisposeSqlConnection(ref SqlConnection cn)
        //{
        //    try
        //    {
        //        if (cn != null)
        //        {
        //            if (cn.State != System.Data.ConnectionState.Closed) { cn.Close(); }
        //            cn.Dispose();
        //            cn = null;
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        //private static void DisposeSqlTransaction(ref SqlTransaction trans)
        //{
        //    try
        //    {
        //        if (trans != null)
        //        {
        //            trans.Dispose();
        //            trans = null;
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        public class MSSQL
        {
            public static string ConnectionString = @"Server=.\SQLExpress;Database=MvcApp1;User Id=mvcapp1_oper;Password=mvcapp1_oper";

            /// https://www.codeproject.com/Tips/423233/How-to-Connect-to-MySQL-Using-Csharp
            public static int ExecuteNonQuery(string sql, params SqlParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) { return 0; }
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand com = new SqlCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
                        return com.ExecuteNonQuery();
                    }
                }
            }

            public static int ExecuteNonQueryWithTransaction(string sql, params SqlParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) { return 0; }
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    SqlTransaction trans = cn.BeginTransaction();
                    int iReturn = 0;
                    try
                    {
                        using (SqlCommand com = new SqlCommand(sql, cn, trans))
                        {
                            if ((arrayOfParameters?.Length ?? 0) > 0)
                            { com.Parameters.AddRange(arrayOfParameters); }
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
                    cn.Open();
                    if (arrayOfSqlItems == null) { return 0; }
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
                                    if ((o.Value?.Length ?? 0) > 0)/// o.Value is an array of parameters.
                                    { com.Parameters.AddRange(o.Value); }
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
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
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
                    //    if ((arrayOfParameters?.Length ?? 0) > 0)
                    //    { da.SelectCommand.Parameters.AddRange(arrayOfParameters); }
                    //    cn.Open();
                    //    da.Fill(dtReturn);
                    //}
                    using (SqlCommand com = new SqlCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
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
            public static string DatabaseFilePath { get; set; } = @"C:\temp\db.sqlite";
            public static string ConnectionStringTemplate = "Version=3;Data Source={0}";

            public static string GetConnectionString()
            {
                //return string.Format(ConnectionStringTemplate, System.IO.Path.GetFileName(DatabaseFilePath));
                return string.Format(ConnectionStringTemplate, DatabaseFilePath);
            }

            /// Create database file if it does not exist.
            public static void CreateDatabaseFileIfNotExist()
            {
                if (!System.IO.File.Exists(DatabaseFilePath))
                {
                    string folder = System.IO.Path.GetDirectoryName(DatabaseFilePath);
                    if (!System.IO.Directory.Exists(folder))
                    {
                        System.IO.Directory.CreateDirectory(folder);
                    }
                    System.Data.SQLite.SQLiteConnection.CreateFile(DatabaseFilePath);
                }
            }

            public static int ExecuteNonQuery(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) { return 0; }
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    cn.Open();
                    using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
                        return com.ExecuteNonQuery();
                    }
                }
            }

            public static int ExecuteNonQueryWithTransaction(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                if (string.IsNullOrEmpty(sql)) { return 0; }
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    cn.Open();
                    int iReturn = -2;
                    cn.Open();
                    System.Data.SQLite.SQLiteTransaction trans = cn.BeginTransaction();
                    try
                    {
                        using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn, trans))
                        {
                            if ((arrayOfParameters?.Length ?? 0) > 0)
                            { com.Parameters.AddRange(arrayOfParameters); }
                            iReturn = com.ExecuteNonQuery();
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

            /// Return value = Number of rows affected.
            /// sConnectionString = Connection string.
            /// arrayOfSqlItems = Array of SQL items.
            /// KeyValuePair<string, SQLiteParameter[]> o1 = new KeyValuePair<string, SQLiteParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SQLiteParameter[]
            ///        {
            ///           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1001 },
            ///           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test1" }
            ///        }
            ///    );
            /// KeyValuePair<string, SQLiteParameter[]> o2 = new KeyValuePair<string, SQLiteParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SQLiteParameter[]
            ///        {
            ///           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1002 },
            ///           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test2" }
            ///        }
            ///    );
            /// KeyValuePair<string, SQLiteParameter[]> o3 = new KeyValuePair<string, SQLiteParameter[]>
            ///    (
            ///        "INSERT INTO SpeedStage (SpeedStageValue, Description) VALUES (@SpeedStageValue, @Description)",
            ///        new SQLiteParameter[]
            ///        {
            ///           new SQLiteParameter("@SpeedStageValue", SqlDbType.Int) { Value = 1003 },
            ///           new SQLiteParameter("@Description", SqlDbType.VarChar) { Value = "test3" }
            ///        }
            ///    );
            /// Example 1:
            /// int i = ExecuteNonQuery(ConnectionString, o1, o2);
            /// Example 2:
            /// KeyValuePair<string, SQLiteParameter[]>[] arrayOfSqlItems = new KeyValuePair<string, SQLiteParameter[]>[] { o1, o2, o3 };
            /// int i = ExecuteNonQuery(ConnectionString, arrayOfSqlItems);
            public static int ExecuteNonQuery(params KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>[] arrayOfSqlItems)
            {
                if (arrayOfSqlItems == null) { return 0; }
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    cn.Open();
                    int iReturn = 0;
                    System.Data.SQLite.SQLiteTransaction trans = cn.BeginTransaction();
                    try
                    {
                        foreach (KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]> o in arrayOfSqlItems)
                        {
                            if (string.IsNullOrEmpty(o.Key) == false)/// o.Key is SQL.
                            {
                                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(o.Key, cn, trans))
                                {
                                    if ((o.Value?.Length ?? 0) > 0)/// o.Value is array of parameters.
                                    { com.Parameters.AddRange(o.Value); }
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
            public static object ExecuteScalar(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                CreateDatabaseFileIfNotExist();
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { com.Parameters.AddRange(arrayOfParameters); }
                        com.Connection.Open();
                        return com.ExecuteScalar();
                    }
                }
            }

            /// Get a table from database.
            public static System.Data.DataTable SelectDataTable(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
            {
                if (!System.IO.File.Exists(DatabaseFilePath)) { return null; }
                using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
                {
                    System.Data.DataTable dtReturn = new System.Data.DataTable();
                    using (System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter(sql, cn))
                    {
                        if ((arrayOfParameters?.Length ?? 0) > 0)
                        { da.SelectCommand.Parameters.AddRange(arrayOfParameters); }
                        cn.Open();
                        da.Fill(dtReturn);
                    }
                    /// Cannot use SQLiteDataReader in some cases.
                    //using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(sql, cn))
                    //{
                    //    if ((arrayOfParameters?.Length ?? 0) > 0)
                    //    { com.Parameters.AddRange(arrayOfParameters); }
                    //    com.Connection.Open();
                    //    using (System.Data.SQLite.SQLiteDataReader rdr = com.ExecuteReader())
                    //    {
                    //        dtReturn.Load(rdr);
                    //    }
                    //}
                    return dtReturn;
                }
            }

            /// Check if table exists.
            /// Return value = true if exists. Otherwise, false.
            public static bool TableExists(string tableName)
            {
                return Convert.ToInt32(ExecuteScalar(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name",
                    new System.Data.SQLite.SQLiteParameter("@name", System.Data.DbType.String) { Value = tableName })) > 0;
            }

            public static bool CheckConnected()
            {
                try
                {
                    CreateDatabaseFileIfNotExist();
                    using (System.Data.SQLite.SQLiteConnection cn = new System.Data.SQLite.SQLiteConnection(GetConnectionString()))
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
        }

        private static void CreateAppFuncLevelTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("AppFuncLevels")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE AppFuncLevels ("
                + "AppFuncLevelId INTEGER PRIMARY KEY NOT NULL,"
                + "UniqueName varchar(255) NOT NULL UNIQUE,"
                + "DisplayName varchar(255) NOT NULL,"
                + "Description varchar(1023) NULL"
                + ");");
            i = DbHandler.SQLite.ExecuteNonQuery("INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (0,'hidden','Level 0','Level 0 menu item, which is hidden.');"
                + "INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (1,'level1','Level 1','Level 1 menu item, which shows on the navigation bar and always visible.');"
                + "INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (2,'level2','Level 2','Level 2 menu item, which is a dropdown item and only visible if selecting its parent level 1 menu item.');"
                + "INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (3,'level3','Level 3','Level 3 menu item, which is a dropdown item and only visible if selecting its parent level 2 menu item.');"
                );
        }

        private static void CreateAppFunctionTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("AppFunctions")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE AppFunctions ("
                + "AppFunctionId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,"
                + "UniqueName varchar(255) NOT NULL UNIQUE,"
                + "DisplayName varchar(255) NOT NULL,"
                + "ControllerName varchar(255) NULL,"
                + "ActionName varchar(255) NULL,"
                + "AppFuncLevelId int NOT NULL DEFAULT 0,"
                + "ParentId int NOT NULL DEFAULT 0,"
                + "SequentialNum int NOT NULL DEFAULT 1001,"
                + "IsDisabled bit NOT NULL DEFAULT 0,"
                + "IsNavItem bit NOT NULL DEFAULT 1,"
                + "CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
                + "CreatedBy int NOT NULL DEFAULT 0,"
                + "UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
                + "UpdatedBy int NOT NULL DEFAULT 0,"
                + "Description varchar(1023) NULL"
                + ");");
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('home','Home','Home','Index',1,0,100,1,'Home page');"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('index2','Index2','Home','Index2',1,0,200,1,'Index 2');"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('about','About','Home','About',1,0,300,1,'About page');"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('contact','Contact','Home','Contact',1,0,400,1,'Contact page');"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('student','Student','Student','Index',1,0,500,1,'Student page');"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('admin','Administration',NULL,NULL,1,0,600,1,'Administration menu');"
                );
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user','User','User','Index',2,AppFunctionId,100,1,'User page' FROM AppFunctions WHERE UniqueName='admin';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role','Role','Role','Index',2,AppFunctionId,200,1,'Role page' FROM AppFunctions WHERE UniqueName='admin';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc','App Function','AppFunction','Index',2,AppFunctionId,300,1,'Function page' FROM AppFunctions WHERE UniqueName='admin';"
                );
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_details','View User Details','User','Index',3,AppFunctionId,100,0,'View User Details Function' FROM AppFunctions WHERE UniqueName='user';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_create','Create User','User','Index',3,AppFunctionId,200,0,'Create User Function' FROM AppFunctions WHERE UniqueName='user';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_edit','Edit User','User','Index',3,AppFunctionId,300,0,'Edit User Function' FROM AppFunctions WHERE UniqueName='user';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_delete','Delete User','User','Index',3,AppFunctionId,400,0,'Delete User Function' FROM AppFunctions WHERE UniqueName='user';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_assign_roles','Assign Roles to User','User','Index',3,AppFunctionId,500,0,'Assign Roles to User Function' FROM AppFunctions WHERE UniqueName='user';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_details','View Role Details','Role','Index',3,AppFunctionId,100,0,'View Role Details Function' FROM AppFunctions WHERE UniqueName='role';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_create','Create Role','Role','Index',3,AppFunctionId,200,0,'Create Role Function' FROM AppFunctions WHERE UniqueName='role';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_edit','Edit Role','Role','Index',3,AppFunctionId,300,0,'Edit Role Function' FROM AppFunctions WHERE UniqueName='role';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_delete','Delete Role','Role','Index',3,AppFunctionId,400,0,'Delete Role Function' FROM AppFunctions WHERE UniqueName='role';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_assign_users','Assign Users to Role','Role','Index',3,AppFunctionId,500,0,'Assign Users to Role Function' FROM AppFunctions WHERE UniqueName='role';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_assign_func','Assign Functions to Role','Role','Index',3,AppFunctionId,600,0,'Assign Functions to Role Function' FROM AppFunctions WHERE UniqueName='role';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_details','View Function Details','AppFunction','Index',3,AppFunctionId,100,0,'View Function Details Function' FROM AppFunctions WHERE UniqueName='appfunc';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_create','Create Function','AppFunction','Index',3,AppFunctionId,200,0,'Create Function Function' FROM AppFunctions WHERE UniqueName='appfunc';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_edit','Edit Function','AppFunction','Index',3,AppFunctionId,300,0,'Edit Function Function' FROM AppFunctions WHERE UniqueName='appfunc';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_delete','Delete Function','AppFunction','Index',3,AppFunctionId,400,0,'Delete Function Function' FROM AppFunctions WHERE UniqueName='appfunc';"
                );
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2','Menu 2',NULL,NULL,2,AppFunctionId,250,1,'Menu 2 page' FROM AppFunctions WHERE UniqueName='admin';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2a','Menu 2A',NULL,NULL,3,AppFunctionId,100,1,'Menu 2A page' FROM AppFunctions WHERE UniqueName='menu2';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2b','Menu 2B',NULL,NULL,3,AppFunctionId,200,1,'Menu 2B page' FROM AppFunctions WHERE UniqueName='menu2';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2c','Menu 2C',NULL,NULL,3,AppFunctionId,300,1,'Menu 2C page' FROM AppFunctions WHERE UniqueName='menu2';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('test1','Test 1',NULL,NULL,1,0,700,1,'Test 1 menu');"
                );
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1a','Test 1a',NULL,NULL,2,AppFunctionId,100,1,'Test 1a' FROM AppFunctions WHERE UniqueName='test1';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1b','Test 1b',NULL,NULL,2,AppFunctionId,200,1,'Test 1B' FROM AppFunctions WHERE UniqueName='test1';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1c','Test 1c',NULL,NULL,2,AppFunctionId,300,1,'Test 1c' FROM AppFunctions WHERE UniqueName='test1';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1b1','Test 1b1',NULL,NULL,3,AppFunctionId,100,1,'Test 1B1 page' FROM AppFunctions WHERE UniqueName='test1b';"
+ "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1b2','Test 1b2',NULL,NULL,3,AppFunctionId,200,1,'Test 1B2 page' FROM AppFunctions WHERE UniqueName='test1b';"
                );
        }

        private static void CreateMapAppFunctionRoleTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("MapAppFunctionsRoles")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE MapAppFunctionsRoles ("
                + "RoleId int NOT NULL,"
                + "AppFunctionId int NOT NULL,"
                + "CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
                + "CreatedBy int NOT NULL DEFAULT 0,"
                + "UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
                + "UpdatedBy int NOT NULL DEFAULT 0,"
                + "PRIMARY KEY (RoleId,AppFunctionId)"
                + ");");
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId) SELECT (SELECT RoleId FROM Roles WHERE UniqueName='admin'),AppFunctionId FROM AppFunctions;"
                );
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId)"
+ "SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions WHERE UniqueName='home' "
+ "UNION "
+ "SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions WHERE UniqueName='menu2b' "
+ "UNION "
+ "SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions "
+ "WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE UniqueName='menu2b') "
+ "UNION "
+ "SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions "
+ "WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE UniqueName='menu2b')) "
+ "UNION "
+ "SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions "
+ "WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId=12) "
+ "UNION "
+ "SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions "
+ "WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE UniqueName='menu2b')) "
+ ";");
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId) SELECT (SELECT RoleId FROM Roles WHERE UniqueName='main'),AppFunctionId FROM AppFunctions WHERE UniqueName='home';"
+ "INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId) SELECT (SELECT RoleId FROM Roles WHERE UniqueName='main'),AppFunctionId FROM AppFunctions WHERE UniqueName='about';"
                );
        }

        private static void CreateMapRoleUserTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("MapRolesUsers")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE MapRolesUsers ("
+ "UserId int NOT NULL,"
+ "RoleId int NOT NULL,"
+ "CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
+ "CreatedBy int NOT NULL DEFAULT 0,"
+ "UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
+ "UpdatedBy int NOT NULL DEFAULT 0,"
+ "PRIMARY KEY (UserId,RoleId)"
+ ");");
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO MapRolesUsers (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) SELECT UserId,(SELECT RoleId FROM Roles WHERE UniqueName='admin'),@UpdatedDt,0,@UpdatedDt,0 FROM Users WHERE LoginName='admin';"
                + "INSERT INTO MapRolesUsers (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) SELECT UserId,(SELECT RoleId FROM Roles WHERE UniqueName='main'),@UpdatedDt,0,@UpdatedDt,0 FROM Users WHERE LoginName='main';"
                + "INSERT INTO MapRolesUsers (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) SELECT UserId,(SELECT RoleId FROM Roles WHERE UniqueName='oper'),@UpdatedDt,0,@UpdatedDt,0 FROM Users WHERE LoginName='oper';",
                new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime) { Value = DateTime.Now }
                );
        }

        private static void CreateRoleTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("Roles")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE Roles ("
+ "RoleId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,"
+ "UniqueName varchar(255) NOT NULL UNIQUE,"
+ "DisplayName varchar(255) NOT NULL,"
+ "IsDisabled bit NOT NULL Default 0,"
+ "CreatedDt datetime NOT NULL Default CURRENT_TIMESTAMP,"
+ "CreatedBy int NOT NULL DEFAULT 0,"
+ "UpdatedDt datetime NOT NULL Default CURRENT_TIMESTAMP,"
+ "UpdatedBy int NOT NULL DEFAULT 0,"
+ "Description varchar(1023) NULL"
+ ");");
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,Description) VALUES ('admin','Administration',0,'Administration Role');"
                + "INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,Description) VALUES ('main','Maintenance',0,'Maintenance Role');"
                + "INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,Description) VALUES ('oper','Operation',0,'Operation Role');"
                );
        }

        private static void CreateStudentTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("Students")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE Students ("
+ "StudentId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,"
+ "StudentName varchar(255) NOT NULL,"
+ "Age int NOT NULL,"
+ "Email varchar(255) NULL"
+ ");"
                );
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO Students (StudentName,Age,Email) VALUES ('John',18,'john@a.com');"
+ "INSERT INTO Students (StudentName,Age,Email) VALUES ('Steve',21,'steve@a.com');"
+ "INSERT INTO Students (StudentName,Age,Email) VALUES ('Bill',25,'bill@a.com');"
                );
        }

        private static void CreateUserTableIfNotExist()
        {
            if (DbHandler.SQLite.TableExists("Users")) { return; }
            int i = DbHandler.SQLite.ExecuteNonQuery(
                "CREATE TABLE Users ("
+ "UserId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,"
+ "LoginName varchar(255) NOT NULL UNIQUE,"
+ "DisplayName varchar(255) NOT NULL,"
+ "Hash varchar(255) NULL,"
+ "Password varchar(255) NULL,"
+ "IsDisabled bit NOT NULL DEFAULT 0,"
+ "CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
+ "CreatedBy int NOT NULL DEFAULT 0,"
+ "UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,"
+ "UpdatedBy int NOT NULL DEFAULT 0,"
+ "Description varchar(1023) NULL"
+ ");");
            i = DbHandler.SQLite.ExecuteNonQuery(
                "INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('system','System',NULL,'system',0,'System');"
+ "INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('admin','Administrator',NULL,'admin',0,'Administrator');"
+ "INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('main','Maintenance Staff',NULL,'main',0,'Maintenance Staff');"
+ "INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('oper','Operator',NULL,'oper',0,'Operator');"
                );
        }

        public static void CheckAndSetDb()
        {
            IsMssqlConnected = MSSQL.CheckConnected();
            if (!IsMssqlConnected)
            {
                CreateStudentTableIfNotExist();
                CreateUserTableIfNotExist();
                CreateRoleTableIfNotExist();
                CreateMapRoleUserTableIfNotExist();
                CreateAppFuncLevelTableIfNotExist();
                CreateAppFunctionTableIfNotExist();
                CreateMapAppFunctionRoleTableIfNotExist();
            }
        }
    }
}