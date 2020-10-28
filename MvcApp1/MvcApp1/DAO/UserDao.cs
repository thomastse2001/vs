using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class UserDao
    {
        private static Models.User Mapping(DataRow dr)
        {
            object o = DbHandler.GetObjectFromDb(dr, "UserId");
            int? i = DbHandler.IsMssqlConnected ? (int?)o: (int?)(long?)o;
            return i == null ? null : new Models.User()
            {
                UserId = i.GetValueOrDefault(),
                LoginName = (string)DbHandler.GetObjectFromDb(dr, "LoginName"),
                DisplayName = (string)DbHandler.GetObjectFromDb(dr, "DisplayName"),
                Hash = (string)DbHandler.GetObjectFromDb(dr, "Hash"),
                Password = (string)DbHandler.GetObjectFromDb(dr, "Password"),
                IsDisabled = (bool?)DbHandler.GetObjectFromDb(dr, "IsDisabled") ?? false,
                CreatedDt = (DateTime?)DbHandler.GetObjectFromDb(dr, "CreatedDt") ?? DateTime.MinValue,
                CreatedBy = (int?)DbHandler.GetObjectFromDb(dr, "CreatedBy") ?? 0,
                UpdatedDt = (DateTime?)DbHandler.GetObjectFromDb(dr, "UpdatedDt") ?? DateTime.MinValue,
                UpdatedBy = (int?)DbHandler.GetObjectFromDb(dr, "UpdatedBy") ?? 0,
                Description = (string)DbHandler.GetObjectFromDb(dr, "Description"),
                /// For Details.
                CreatedByDisplayName = (string)DbHandler.GetObjectFromDb(dr, "CreatedByDisplayName"),
                UpdatedByDisplayName = (string)DbHandler.GetObjectFromDb(dr, "UpdatedByDisplayName"),
                /// For UI editing.
                IsUpdateHash = false,
                RetypedPassword = "",
                /// For MapRolesUsers.
                IsSelected = Convert.ToBoolean(DbHandler.GetObjectFromDb(dr, "IsSelected")),
                /// For Assign roles to a specific user.
                RoleList = null
            };
        }

        public static Models.User GetUnit(int UserId)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT u0.*,u1.DisplayName AS CreatedByDisplayName,u2.DisplayName AS UpdatedByDisplayName"
                    + " FROM Users u0 LEFT JOIN Users u1 ON u0.CreatedBy=u1.UserId"
                    + " LEFT JOIN Users u2 ON u0.UpdatedBy=u2.UserId"
                    + " WHERE u0.UserId=@UserId";
                if (DbHandler.IsMssqlConnected)
                {
                    dt = DbHandler.MSSQL.SelectDataTable(sql, new SqlParameter("@UserId", SqlDbType.Int) { Value = UserId });
                }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql, new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = UserId });
                }
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                return Mapping(dt.Rows[0]);
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        /// Return value = UserId. If the record cannot be found, return -1.
        public static int GetIdByLoginname(string loginname)
        {
            string sql = "SELECT UserId FROM Users WHERE LoginName=@LoginName";
            return DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(loginname) })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(loginname) }));
        }

        public static List<Models.User> GetListBySqlFromMssql(string sql, params SqlParameter[] arrayOfParameters)
        {
            DataTable dt = null;
            try
            {
                dt = DbHandler.MSSQL.SelectDataTable(sql, arrayOfParameters);
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                List<Models.User> rList = new List<Models.User>();
                foreach (DataRow dr in dt.Rows)
                {
                    Models.User o = Mapping(dr);
                    if (o != null) { rList.Add(o); }
                }
                return rList;
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        public static List<Models.User> GetListBySqlFromSQLite(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
        {
            DataTable dt = null;
            try
            {
                dt = DbHandler.SQLite.SelectDataTable(sql, arrayOfParameters);
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                List<Models.User> rList = new List<Models.User>();
                foreach (DataRow dr in dt.Rows)
                {
                    Models.User o = Mapping(dr);
                    if (o != null) { rList.Add(o); }
                }
                return rList;
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        public static List<Models.User> GetAll()
        {
            string sql = "SELECT * FROM Users";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql):
                GetListBySqlFromSQLite(sql);
        }

        public static bool LoginNameExists(string LoginName)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE LoginName=@LoginName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = LoginName })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString) { Value = LoginName }))
                ) > 0;
        }

        public static bool LoginNameExists(string LoginName, int UserId)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE UserId!=@UserId AND LoginName=@LoginName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql,
                new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = LoginName },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = UserId })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql,
                new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString) { Value = LoginName },
                new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = UserId }))
                ) > 0;
        }

        public static int InsertUnit(Models.User o)
        {
            if (o == null) { return -1; }
            string sql = "INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy,Description) VALUES (@LoginName,@DisplayName,@Hash,@Password,@IsDisabled,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy,@Description)";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.LoginName) },
                new SqlParameter("@DisplayName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new SqlParameter("@Hash", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Hash) },
                new SqlParameter("@Password", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Password) },
                new SqlParameter("@IsDisabled", SqlDbType.Bit) { Value = o.IsDisabled },
                new SqlParameter("@UpdatedDt", SqlDbType.DateTime) { Value = o.UpdatedDt },
                new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = o.UpdatedBy },
                new SqlParameter("@Description", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Description) }):
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.LoginName) },
                new System.Data.SQLite.SQLiteParameter("@DisplayName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new System.Data.SQLite.SQLiteParameter("@Hash", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Hash) },
                new System.Data.SQLite.SQLiteParameter("@Password", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Password) },
                new System.Data.SQLite.SQLiteParameter("@IsDisabled", DbType.Boolean) { Value = o.IsDisabled },
                new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime) { Value = o.UpdatedDt },
                new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32) { Value = o.UpdatedBy },
                new System.Data.SQLite.SQLiteParameter("@Description", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Description) });
        }

        /// Return Value = number of records affected.
        public static int DeleteUnit(int id)
        {
            string sql = "DELETE FROM Users WHERE UserId=@UserId";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@UserId", SqlDbType.Int) { Value = id }):
                DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = id });
        }

        /// Return value = number of records affected.
        public static int UpdateUnit(Models.User o)
        {
            if (o == null) { return -1; }
            string sql = "UPDATE Users SET LoginName=@LoginName,DisplayName=@DisplayName,IsDisabled=@IsDisabled,UpdatedDt=@UpdatedDt,UpdatedBy=@UpdatedBy,Description=@Description" +
                (o.IsUpdateHash ? ",Hash=@Hash,Password=@Password" : "") +
                " WHERE UserId=@UserId";
            List<SqlParameter> list = null;
            List<System.Data.SQLite.SQLiteParameter> list2 = null;
            try
            {
                if (DbHandler.IsMssqlConnected)
                {
                    list = new List<SqlParameter>()
                    {
                        new SqlParameter("@UserId", SqlDbType.Int) { Value = o.UserId },
                        new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.LoginName) },
                        new SqlParameter("@DisplayName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                        new SqlParameter("@IsDisabled", SqlDbType.Bit) { Value = o.IsDisabled },
                        new SqlParameter("@UpdatedDt", SqlDbType.DateTime) { Value = o.UpdatedDt },
                        new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = o.UpdatedBy },
                        new SqlParameter("@Description", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Description) }
                    };
                    if (o.IsUpdateHash)
                    {
                        list.Add(new SqlParameter("@Hash", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Hash) });
                        list.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Password) });
                    }
                    return DbHandler.MSSQL.ExecuteNonQuery(sql, list.ToArray());
                }
                else
                {
                    list2 = new List<System.Data.SQLite.SQLiteParameter>()
                    {
                        new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32){ Value = o.UserId },
                        new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString){ Value = DbHandler.GetObjectToDb(o.LoginName) },
                        new System.Data.SQLite.SQLiteParameter("@DisplayName", DbType.AnsiString){ Value = DbHandler.GetObjectToDb(o.DisplayName) },
                        new System.Data.SQLite.SQLiteParameter("@IsDisabled", DbType.Boolean){ Value = o.IsDisabled },
                        new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime){ Value = o.UpdatedDt },
                        new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32){ Value = o.UpdatedBy },
                        new System.Data.SQLite.SQLiteParameter("@Description", DbType.AnsiString){ Value = DbHandler.GetObjectToDb(o.Description) }
                    };
                    if (o.IsUpdateHash)
                    {
                        list2.Add(new System.Data.SQLite.SQLiteParameter("@Hash", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Hash) });
                        list2.Add(new System.Data.SQLite.SQLiteParameter("@Password", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Password) });
                    }
                    return DbHandler.SQLite.ExecuteNonQuery(sql, list2.ToArray());
                }
            }
            finally
            {
                if (list != null) { list.Clear(); list = null; }
                if (list2 != null) { list2.Clear(); list2 = null; }
            }
        }

        /// Get all users in a specific role.
        public static List<Models.User> GetListByRoleId(int roleId)
        {
            string sql = "SELECT * FROM Users WHERE UserId IN (SELECT UserId FROM MapRolesUsers WHERE RoleId=@RoleId)";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql, new SqlParameter("@RoleId", SqlDbType.Int){ Value = roleId }):
                GetListBySqlFromSQLite(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = roleId });
        }

        /// Get a list of all users if they are selected by a specific role.
        public static List<Models.User> GetListSelectedByRoleId(int roleId)
        {
            string sql = "SELECT u.*,CASE WHEN (SELECT 1 FROM MapRolesUsers m WHERE m.UserId=u.UserId AND m.RoleId=@RoleId)=1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsSelected FROM Users u";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql, new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId }):
                GetListBySqlFromSQLite(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = roleId });
        }

        /// Get a list of all users with selected by UserId.
        public static List<Models.User> GetListSelectedByUserId(params int[] arrayOfUserId)
        {
            string s;
            if ((arrayOfUserId?.Length ?? 0) < 1) { s = "CAST(0 AS bit)"; }
            else { s = "CASE WHEN UserId IN (" + string.Join(",", arrayOfUserId.Select(i => i.ToString()).ToArray()) + ") THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END"; }
            string sql = "SELECT *," + s + " AS IsSelected FROM Users";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql):
                GetListBySqlFromSQLite(sql);
        }

        public static Models.User LoginAuthentication(string loginname, string password)
        {
            if (string.IsNullOrWhiteSpace(loginname)) { return null; }
            string hash = BAL.CommonHelper.ComputeHashFromString(password ?? "");
            string sql = "SELECT * FROM Users WHERE LoginName=@LoginName AND (Hash=@Hash OR (NOT(Password IS NULL OR Password='') AND Password=@Password))";
            List<Models.User> list = DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql,
                new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = loginname },
                new SqlParameter("@Hash", SqlDbType.VarChar) { Value = hash },
                new SqlParameter("@Password", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(password) }) :
                GetListBySqlFromSQLite(sql,
                new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString) { Value = loginname },
                new System.Data.SQLite.SQLiteParameter("@Hash", DbType.AnsiString) { Value = hash },
                new System.Data.SQLite.SQLiteParameter("@Password", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(password) }
                );
            return list?.FirstOrDefault();
        }
    }
}