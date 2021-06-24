using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class RoleDao
    {
        private static Models.Role Mapping(DataRow dr)
        {
            object o = DbHandler.GetObjectFromDb(dr, "RoleId");
            int? i = DbHandler.IsMssqlConnected ? (int?)o : (int?)(long?)o;
            return i == null ? null : new Models.Role()
            {
                RoleId = i.GetValueOrDefault(),
                UniqueName = (string)DbHandler.GetObjectFromDb(dr, "UniqueName"),
                DisplayName = (string)DbHandler.GetObjectFromDb(dr, "DisplayName"),
                IsDisabled = (bool)DbHandler.GetObjectFromDb(dr, "IsDisabled"),
                CreatedDt = (DateTime?)DbHandler.GetObjectFromDb(dr, "CreatedDt") ?? DateTime.MinValue,
                CreatedBy = (int?)DbHandler.GetObjectFromDb(dr, "CreatedBy") ?? 0,
                UpdatedDt = (DateTime?)DbHandler.GetObjectFromDb(dr, "UpdatedDt") ?? DateTime.MinValue,
                UpdatedBy = (int?)DbHandler.GetObjectFromDb(dr, "UpdatedBy") ?? 0,
                Description = (string)DbHandler.GetObjectFromDb(dr, "Description"),
                /// For Details.
                CreatedByDisplayName = (string)DbHandler.GetObjectFromDb(dr, "CreatedByDisplayName"),
                UpdatedByDisplayName = (string)DbHandler.GetObjectFromDb(dr, "UpdatedByDisplayName"),
                /// For MapRolesUsers.
                IsSelected = Convert.ToBoolean(DbHandler.GetObjectFromDb(dr, "IsSelected")),
                /// For Assign users to a specific role.
                UserList = null
            };
        }

        public static Models.Role GetUnit(int RoleId)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT r.*,u1.DisplayName AS CreatedByDisplayName,u2.DisplayName AS UpdatedByDisplayName"
                    + " FROM Roles r LEFT JOIN Users u1 ON r.CreatedBy=u1.UserId"
                    + " LEFT JOIN Users u2 ON r.UpdatedBy=u2.UserId"
                    + " WHERE r.RoleId=@RoleId";
                dt = DbHandler.IsMssqlConnected ?
                    DbHandler.MSSQL.SelectDataTable(sql, new SqlParameter("@RoleId", SqlDbType.Int) { Value = RoleId }) :
                    DbHandler.SQLite.SelectDataTable(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = RoleId });
                if ((dt?.Rows.Count ?? 0) < 1) return null;
                return Mapping(dt.Rows[0]);
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        /// Return value = RoleId. If the record cannot be found, return -1.
        public static int GetIdByUniqueName(string uniqueName)
        {
            string sql = "SELECT RoleId FROM Roles WHERE UniqueName=@UniqueName";
            return DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(uniqueName) })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(uniqueName) }));
        }

        private static List<Models.Role> GetListByDataTable(DataTable dt)
        {
            if ((dt?.Rows.Count ?? 0) < 1) return null;
            List<Models.Role> rList = new List<Models.Role>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.Role o = Mapping(dr);
                if (o != null) rList.Add(o);
            }
            return rList;
        }

        public static List<Models.Role> GetListBySqlFromMssql(string sql, params SqlParameter[] arrayOfParameters)
        {
            //DataTable dt = null;
            //try
            //{
            //    dt = DbHandler.MSSQL.SelectDataTable(sql, arrayOfParameters);
            //    if ((dt?.Rows.Count ?? 0) < 1) return null;
            //    List<Models.Role> rList = new List<Models.Role>();
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        Models.Role o = Mapping(dr);
            //        if (o != null) rList.Add(o);
            //    }
            //    return rList;
            //}
            //finally { DbHandler.DisposeDataTable(ref dt); }
            return GetListByDataTable(DbHandler.MSSQL.SelectDataTable(sql, arrayOfParameters));
		}

        public static List<Models.Role> GetListBySqlFromSQLite(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
        {
            //DataTable dt = null;
            //try
            //{
            //    dt = DbHandler.SQLite.SelectDataTable(sql, arrayOfParameters);
            //    if ((dt?.Rows.Count ?? 0) < 1) return null;
            //    List<Models.Role> rList = new List<Models.Role>();
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        Models.Role o = Mapping(dr);
            //        if (o != null) rList.Add(o);
            //    }
            //    return rList;
            //}
            //finally { DbHandler.DisposeDataTable(ref dt); }
            return GetListByDataTable(DbHandler.SQLite.SelectDataTable(sql, arrayOfParameters));
        }

        public static List<Models.Role> GetAll()
        {
            string sql = "SELECT * FROM Roles";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql):
                GetListBySqlFromSQLite(sql);
        }

        public static bool UniqueNameExists(string UniqueName)
        {
            string sql = "SELECT COUNT(*) FROM Roles WHERE UniqueName=@UniqueName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = UniqueName })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = UniqueName }))
                ) > 0;
        }

        public static bool UniqueNameExists(string UniqueName, int RoleId)
        {
            string sql = "SELECT COUNT(*) FROM Roles WHERE RoleId!=@RoleId AND UniqueName=@UniqueName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql,
                new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = UniqueName },
                new SqlParameter("@RoleId", SqlDbType.Int) { Value = RoleId })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql,
                new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = UniqueName },
                new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = RoleId }))
                ) > 0;
        }

        public static int InsertUnit(Models.Role o)
        {
            if (o == null) { return -1; }
            string sql = "INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy,Description) VALUES (@UniqueName,@DisplayName,@IsDisabled,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy,@Description)";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new SqlParameter("@DisplayName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new SqlParameter("@IsDisabled", SqlDbType.Bit) { Value = o.IsDisabled },
                new SqlParameter("@UpdatedDt", SqlDbType.DateTime) { Value = o.UpdatedDt },
                new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = o.UpdatedBy },
                new SqlParameter("@Description", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Description) }) :
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new System.Data.SQLite.SQLiteParameter("@DisplayName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new System.Data.SQLite.SQLiteParameter("@IsDisabled", DbType.Boolean) { Value = o.IsDisabled },
                new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime) { Value = o.UpdatedDt },
                new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32) { Value = o.UpdatedBy },
                new System.Data.SQLite.SQLiteParameter("@Description", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Description) });
        }

        /// Return Value = the number of records affected.
        public static int DeleteUnit(int id)
        {
            string sql = "DELETE FROM Roles WHERE RoleId=@RoleId";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@RoleId", SqlDbType.Int) { Value = id }):
                DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = id });
        }

        /// Return value = the number of records affected.
        public static int UpdateUnit(Models.Role o)
        {
            if (o == null) { return -1; }
            string sql = "UPDATE Roles SET UniqueName=@UniqueName,DisplayName=@DisplayName" +
                ",IsDisabled=@IsDisabled,UpdatedDt=@UpdateDt,UpdatedBy=@UpdatedBy,Description=@Description" +
                " WHERE RoleId=@RoleId";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@RoleId", SqlDbType.Int) { Value = o.RoleId },
                new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new SqlParameter("@DisplayName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new SqlParameter("@IsDisabled", SqlDbType.Bit) { Value = o.IsDisabled },
                new SqlParameter("@UpdateDt", SqlDbType.DateTime) { Value = o.UpdatedDt },
                new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = o.UpdatedBy },
                new SqlParameter("@Description", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Description) }):
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = o.RoleId },
                new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new System.Data.SQLite.SQLiteParameter("@DisplayName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new System.Data.SQLite.SQLiteParameter("@IsDisabled", DbType.Boolean) { Value = o.IsDisabled },
                new System.Data.SQLite.SQLiteParameter("@UpdateDt", DbType.DateTime) { Value = o.UpdatedDt },
                new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32) { Value = o.UpdatedBy },
                new System.Data.SQLite.SQLiteParameter("@Description", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Description) });
        }

        ///  Get a list of all roles by RoleId.
        public static List<Models.Role> GetListByRoleId(params int[] arrayOfRoleId)
        {
            if ((arrayOfRoleId?.Length ?? 0) < 1) return null;
            /// https://coderwall.com/p/oea7uq/convert-simple-int-array-to-string-c
            string sql = "SELECT * FROM Roles WHERE RoleId IN (" + string.Join(",", arrayOfRoleId.Select(i => i.ToString()).ToArray()) + ")";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql) :
                GetListBySqlFromSQLite(sql);
        }

        /// Get a list of roles in a specific user.
        public static List<Models.Role> GetListByUserId(int userId)
        {
            string sql = "SELECT * FROM Roles WHERE RoleId IN (SELECT RoleId FROM MapRolesUsers WHERE UserId=@UserId)";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql, new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }):
                GetListBySqlFromSQLite(sql, new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = userId });
        }

        /// Get a list of all roles if they are selected by a specific user.
        public static List<Models.Role> GetListSelectedByUserId(int userId)
        {
            //return GetListBySql("SELECT r.*,CASE WHEN m.UserId=@UserId THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsSelected FROM Roles r LEFT JOIN MapRolesUsers m ON m.RoleId=r.RoleId", new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
            string sql = "SELECT r.*,CASE WHEN (SELECT 1 FROM MapRolesUsers m WHERE m.RoleId=r.RoleId AND m.UserId=@UserId)=1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsSelected FROM Roles r";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql, new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }):
                GetListBySqlFromSQLite(sql, new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = userId });
        }

        /// Get a list of all roles with selected by RoleId.
        public static List<Models.Role> GetListSelectedByRoleId(params int[] arrayOfRoldId)
        {
            string s;
            if ((arrayOfRoldId?.Length ?? 0) < 1) s = "CAST(0 AS bit)";
            else s = "CASE WHEN RoleId IN (" + string.Join(",", arrayOfRoldId.Select(i => i.ToString()).ToArray()) + ") THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END";
            string sql = "SELECT *," + s + " AS IsSelected FROM Roles";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql):
                GetListBySqlFromSQLite(sql);
        }
    }
}