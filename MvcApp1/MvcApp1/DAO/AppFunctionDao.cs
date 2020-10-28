using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class AppFunctionDao
    {
        private static Models.AppFunction Mapping(DataRow dr)
        {
            object o = DbHandler.GetObjectFromDb(dr, "AppFunctionId");
            int? i = DbHandler.IsMssqlConnected ? (int?)o : (int?)(long?)o;
            return i == null ? null : new Models.AppFunction()
            {
                AppFunctionId = i.GetValueOrDefault(),
                UniqueName = (string)DbHandler.GetObjectFromDb(dr, "UniqueName"),
                DisplayName = (string)DbHandler.GetObjectFromDb(dr, "DisplayName"),
                ControllerName = (string)DbHandler.GetObjectFromDb(dr, "ControllerName"),
                ActionName = (string)DbHandler.GetObjectFromDb(dr, "ActionName"),
                AppFuncLevelId = (int?)DbHandler.GetObjectFromDb(dr, "AppFuncLevelId") ?? 0,
                ParentId = (int?)DbHandler.GetObjectFromDb(dr, "ParentId") ?? 0,
                SequentialNum = (int?)DbHandler.GetObjectFromDb(dr, "SequentialNum") ?? 1001,
                IsDisabled = (bool?)DbHandler.GetObjectFromDb(dr, "IsDisabled") ?? false,
                IsNavItem = (bool?)DbHandler.GetObjectFromDb(dr, "IsNavItem") ?? false,
                CreatedDt = (DateTime?)DbHandler.GetObjectFromDb(dr, "CreatedDt") ?? DateTime.MinValue,
                CreatedBy = (int?)DbHandler.GetObjectFromDb(dr, "CreatedBy") ?? 0,
                UpdatedDt = (DateTime?)DbHandler.GetObjectFromDb(dr, "UpdatedDt") ?? DateTime.MinValue,
                UpdatedBy = (int?)DbHandler.GetObjectFromDb(dr, "UpdatedBy") ?? 0,
                Description = (string)DbHandler.GetObjectFromDb(dr, "Description"),
                /// For Details.
                CreatedByDisplayName = (string)DbHandler.GetObjectFromDb(dr, "CreatedByDisplayName"),
                UpdatedByDisplayName = (string)DbHandler.GetObjectFromDb(dr, "UpdatedByDisplayName"),
                /// For MapAppFunctionRole.
                IsSelected = Convert.ToBoolean(DbHandler.GetObjectFromDb(dr, "IsSelected"))
            };
        }

        //private static Models.AppFunction MappingNavigationObject(DataRow dr)
        //{
        //    if (dr == null) { return null; }
        //    int? ii = (int?)DbHandler.GetObjectFromDb(dr, "AppFunctionId");
        //    if (ii == null) { return null; }
        //    return new Models.AppFunction()
        //    {
        //        AppFunctionId = ii.GetValueOrDefault(),
        //        UniqueName = (string)DbHandler.GetObjectFromDb(dr, "UniqueName"),
        //        DisplayName = (string)DbHandler.GetObjectFromDb(dr, "DisplayName"),
        //        ActionName = (string)DbHandler.GetObjectFromDb(dr, "ActionName") ?? "",
        //        ControllerName = (string)DbHandler.GetObjectFromDb(dr, "ControllerName") ?? ""
        //    };
        //}

        public static List<Models.AppFunction> GetNavigationList(int AppFuncLevelId, int ParentId) { return GetList(AppFuncLevelId, ParentId, true, true); }
        public static List<Models.AppFunction> GetList(int AppFuncLevelId, int ParentId, bool? IsNavItem, bool isShort)
        {
            if (AppFuncLevelId > 3 || AppFuncLevelId < 0) { return null; }
            DataTable dt = null;
            List<SqlParameter> listOfPara = null;
            List<System.Data.SQLite.SQLiteParameter> listOfPara2 = null;
            try
            {
                string sql;
                if (isShort) { sql = "SELECT AppFunctionId,UniqueName,DisplayName,ActionName,ControllerName,ParentId,AppFuncLevelId FROM AppFunctions WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId" + (IsNavItem.HasValue ? " AND IsNavItem=@IsNavItem" : "") + " ORDER BY SequentialNum,DisplayName,UniqueName"; }
                else { sql = "SELECT * FROM AppFunctions WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId" + (IsNavItem.HasValue ? " AND IsNavItem=@IsNavItem" : "") + " ORDER BY SequentialNum,DisplayName,UniqueName"; }
                if (DbHandler.IsMssqlConnected)
                {
                    listOfPara = new List<SqlParameter>()
                    {
                        new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = AppFuncLevelId },
                        new SqlParameter("@ParentId", SqlDbType.Int) { Value = ParentId }
                    };
                    if (IsNavItem.HasValue) { listOfPara.Add(new SqlParameter("@IsNavItem", SqlDbType.Bit) { Value = IsNavItem.GetValueOrDefault() }); }
                    dt = DbHandler.MSSQL.SelectDataTable(sql, listOfPara.ToArray());
                }
                else
                {
                    listOfPara2 = new List<System.Data.SQLite.SQLiteParameter>()
                    {
                        new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", DbType.Int32) { Value = AppFuncLevelId },
                        new System.Data.SQLite.SQLiteParameter("@ParentId", DbType.Int32) { Value = ParentId }
                    };
                    if (IsNavItem.HasValue) { listOfPara2.Add(new System.Data.SQLite.SQLiteParameter("@IsNavItem", DbType.Boolean) { Value = IsNavItem.GetValueOrDefault() }); }
                    dt = DbHandler.SQLite.SelectDataTable(sql, listOfPara2.ToArray());
                }
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                List<Models.AppFunction> rList = new List<Models.AppFunction>();
                foreach (DataRow dr in dt.Rows)
                {
                    Models.AppFunction o = Mapping(dr);
                    if (o != null)
                    {
                        //o.ParentId = ParentId;
                        //o.AppFuncLevelId = AppFuncLevelId;
                        o.ChildList = GetList(AppFuncLevelId + 1, o.AppFunctionId, IsNavItem, isShort);
                        rList.Add(o);
                    }
                }
                return rList;
            }
            finally
            {
                DbHandler.DisposeDataTable(ref dt);
                if (listOfPara != null) { listOfPara.Clear(); listOfPara = null; }
                if (listOfPara2 != null) { listOfPara2.Clear(); listOfPara2 = null; }
            }
        }

        public static List<Models.AppFunction> GetListBySqlFromMssql(string sql, params SqlParameter[] arrayOfParameters)
        {
            DataTable dt = null;
            try
            {
                dt = DbHandler.MSSQL.SelectDataTable(sql, arrayOfParameters);
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                List<Models.AppFunction> rList = new List<Models.AppFunction>();
                foreach (DataRow dr in dt.Rows)
                {
                    Models.AppFunction o = Mapping(dr);
                    if (o != null) { rList.Add(o); }
                }
                return rList;
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        public static List<Models.AppFunction> GetListBySqlFromSQLite(string sql, params System.Data.SQLite.SQLiteParameter[] arrayOfParameters)
        {
            DataTable dt = null;
            try
            {
                dt = DbHandler.SQLite.SelectDataTable(sql, arrayOfParameters);
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                List<Models.AppFunction> rList = new List<Models.AppFunction>();
                foreach (DataRow dr in dt.Rows)
                {
                    Models.AppFunction o = Mapping(dr);
                    if (o != null) { rList.Add(o); }
                }
                return rList;
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        public static List<Models.AppFunction> GetNavigationListByUserId(int UserId, int AppFuncLevelId, int ParentId)
        {
            string sql = "SELECT AppFunctionId,UniqueName,DisplayName,ActionName,ControllerName,ParentId,AppFuncLevelId FROM AppFunctions WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId AND IsNavItem=@IsNavItem AND AppFunctionId IN (SELECT AppFunctionId FROM MapAppFunctionsRoles WHERE RoleId IN (SELECT RoleId FROM MapRolesUsers WHERE UserId=@UserId)) ORDER BY SequentialNum,DisplayName,UniqueName";
            List<Models.AppFunction> list = null;
            if (DbHandler.IsMssqlConnected)
            {
                list = GetListBySqlFromMssql(sql,
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = UserId },
                    new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = AppFuncLevelId },
                    new SqlParameter("@ParentId", SqlDbType.Int) { Value = ParentId },
                    new SqlParameter("@IsNavItem", SqlDbType.Bit) { Value = true });
            }
            else
            {
                list = GetListBySqlFromSQLite(sql,
                    new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = UserId },
                    new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", DbType.Int32) { Value = AppFuncLevelId },
                    new System.Data.SQLite.SQLiteParameter("@ParentId", DbType.Int32) { Value = ParentId },
                    new System.Data.SQLite.SQLiteParameter("@IsNavItem", DbType.Boolean) { Value = true });
            }
            if ((list?.Count ?? 0) < 1) { return null; }
            foreach (Models.AppFunction o in list)
            {
                if (o != null)
                {
                    o.ChildList = GetNavigationListByUserId(UserId, AppFuncLevelId + 1, o.AppFunctionId);
                }
            }
            return list;
        }

        //public static List<Models.AppFunction> GetList(int AppFuncLevelId, int ParentId)
        //{
        //    if (AppFuncLevelId > 3 || AppFuncLevelId < 0) { return null; }
        //    DataTable dt = null;
        //    List<SqlParameter> listOfPara = null;
        //    try
        //    {
        //        string sSql = "SELECT * FROM AppFunctions WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId ORDER BY SequentialNum,DisplayName,UniqueName";
        //        SqlParameter[] array1 = new SqlParameter[]
        //        {
        //            new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = AppFuncLevelId },
        //            new SqlParameter("@ParentId", SqlDbType.Int) { Value = ParentId }
        //        };
        //        dt = DbHandler.MssqlSelectDataTable2(DbHandler.ConnectionString, sSql, array1);
        //        if (dt == null || dt.Rows.Count < 1) { return null; }
        //        List<Models.AppFunction> rList = new List<Models.AppFunction>();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            Models.AppFunction o 
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        public static Models.AppFunction GetUnit(int id)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT f.*,u1.DisplayName AS CreatedByDisplayName,u2.DisplayName AS UpdatedByDisplayName"
                    + " FROM AppFunctions f LEFT JOIN Users u1 ON f.CreatedBy=u1.UserId"
                    + " LEFT JOIN Users u2 ON f.UpdatedBy=u2.UserId"
                    + " WHERE f.AppFunctionId=@AppFunctionId";
                if (DbHandler.IsMssqlConnected)
                { dt = DbHandler.MSSQL.SelectDataTable(sql, new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = id }); }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql, new System.Data.SQLite.SQLiteParameter("@AppFunctionId", DbType.Int32) { Value = id });
                }
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                return Mapping(dt.Rows[0]);
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        /// Return value = id. If the record cannot be found, return -1.
        public static int GetIdByUniqueName(string uniqueName)
        {
            string sql = "SELECT AppFunctionId FROM AppFunctions WHERE UniqueName=@UniqueName";
            return DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(uniqueName) })) :
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(uniqueName) }));
        }

        public static List<Models.AppFunction> GetAll()
        {
            string sql = "SELECT * FROM AppFunctions";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql):
                GetListBySqlFromSQLite(sql);
        }

        public static bool UniqueNameExists(string UniqueName)
        {
            string sql = "SELECT COUNT(*) FROM AppFunctions WHERE UniqueName=@UniqueName";
            return (
                DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = UniqueName })) :
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = UniqueName }))
                ) > 0;
        }

        public static bool UniqueNameExists(string UniqueName, int AppFunctionId)
        {
            string sql = "SELECT COUNT(*) FROM AppFunctions WHERE AppFunctionId!=@AppFunctionId AND UniqueName=@UniqueName";
            return (
                DbHandler.IsMssqlConnected?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = UniqueName }, new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = AppFunctionId })):
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = UniqueName }))
                ) > 0;
        }

        /// Insert the record.
        public static int InsertUnit(Models.AppFunction o)
        {
            string sql = "INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsDisabled,IsNavItem,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy,Description) VALUES (@UniqueName,@DisplayName,@ControllerName,@ActionName,@AppFuncLevelId,@ParentId,@SequentialNum,@IsDisabled,@IsNavItem,GETDATE(),@CreatedBy,GETDATE(),@UpdatedBy,@Description)";
            return o == null ? -1 : (
                DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new SqlParameter("@DisplayName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new SqlParameter("@ControllerName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.ControllerName) },
                new SqlParameter("@ActionName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.ActionName) },
                new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = o.AppFuncLevelId },
                new SqlParameter("@ParentId", SqlDbType.Int) { Value = o.ParentId },
                new SqlParameter("@SequentialNum", SqlDbType.Int) { Value = o.SequentialNum },
                new SqlParameter("@IsDisabled", SqlDbType.Bit) { Value = o.IsDisabled },
                new SqlParameter("@IsNavItem", SqlDbType.Bit) { Value = o.IsNavItem },
                //new SqlParameter("@CreatedDt", SqlDbType.DateTime) { Value = o.CreatedDt },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = o.CreatedBy },
                //new SqlParameter("@CreatedByDisplayName", SqlDbType.VarChar) { Value = o.CreatedByDisplayName },
                //new SqlParameter("@UpdatedDt", SqlDbType.DateTime) { Value = o.UpdatedDt },
                new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = o.UpdatedBy },
                //new SqlParameter("@UpdatedByDisplayName", SqlDbType.VarChar) { Value = o.UpdatedByDisplayName },
                new SqlParameter("@Description", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Description) }
                ):
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new System.Data.SQLite.SQLiteParameter("@DisplayName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new System.Data.SQLite.SQLiteParameter("@ControllerName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.ControllerName) },
                new System.Data.SQLite.SQLiteParameter("@ActionName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.ActionName) },
                new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", DbType.Int32) { Value = DbHandler.GetObjectToDb(o.AppFuncLevelId) },
                new System.Data.SQLite.SQLiteParameter("@ParentId", DbType.Int32) { Value = DbHandler.GetObjectToDb(o.ParentId) },
                new System.Data.SQLite.SQLiteParameter("@SequentialNum", DbType.Int32) { Value = DbHandler.GetObjectToDb(o.SequentialNum) },
                new System.Data.SQLite.SQLiteParameter("@IsDisabled", DbType.Boolean) { Value = DbHandler.GetObjectToDb(o.IsDisabled) },
                new System.Data.SQLite.SQLiteParameter("@IsNavItem", DbType.Boolean) { Value = DbHandler.GetObjectToDb(o.IsNavItem) },
                //new System.Data.SQLite.SQLiteParameter("@CreatedDt", DbType.DateTime) { Value = DbHandler.GetObjectToDb(o.CreatedDt) },
                new System.Data.SQLite.SQLiteParameter("@CreatedBy", DbType.Int32) { Value = DbHandler.GetObjectToDb(o.CreatedBy) },
                //new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime) { Value = DbHandler.GetObjectToDb(o.UpdatedDt) },
                new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32) { Value = DbHandler.GetObjectToDb(o.UpdatedBy) },
                new System.Data.SQLite.SQLiteParameter("@Description", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Description) }
                ));
        }

        /// Return Value = number of records affected.
        public static int DeleteUnit(int id)
        {
            string sql = "DELETE FROM AppFunctions WHERE AppFunctionId=@AppFunctionId";
            return DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = id }) :
                DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@AppFunctionId", DbType.Int32) { Value = id });
        }

        /// Return value = number of records affected.
        public static int UpdateUnit(Models.AppFunction o)
        {
            //if (o == null) { return -1; }
            string sql = "UPDATE AppFunctions SET UniqueName=@UniqueName,DisplayName=@DisplayName" +
                ",ControllerName=@ControllerName,ActionName=@ActionName,AppFuncLevelId=@AppFuncLevelId,ParentId=@ParentId,SequentialNum=@SequentialNum,IsNavItem=@IsNavItem" +
                ",IsDisabled=@IsDisabled,UpdatedDt=GETDATE(),UpdatedBy=@UpdatedBy,Description=@Description" +
                " WHERE AppFunctionId=@AppFunctionId";
            return o == null ? -1 : (
                DbHandler.IsMssqlConnected ?
                DbHandler.MSSQL.ExecuteNonQuery(sql,
                new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = o.AppFunctionId },
                new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new SqlParameter("@DisplayName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new SqlParameter("@ControllerName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.ControllerName) },
                new SqlParameter("@ActionName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.ActionName) },
                new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = o.AppFuncLevelId },
                new SqlParameter("@ParentId", SqlDbType.Int) { Value = o.ParentId },
                new SqlParameter("@SequentialNum", SqlDbType.Int) { Value = o.SequentialNum },
                new SqlParameter("@IsDisabled", SqlDbType.Bit) { Value = o.IsDisabled },
                new SqlParameter("@IsNavItem", SqlDbType.Bit) { Value = o.IsNavItem },
                new SqlParameter("@UpdatedBy", SqlDbType.Int) { Value = o.UpdatedBy },
                new SqlParameter("@Description", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(o.Description) }
                ):
                DbHandler.SQLite.ExecuteNonQuery(sql,
                new System.Data.SQLite.SQLiteParameter("@AppFunctionId", DbType.Int32) { Value = o.AppFunctionId },
                new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.UniqueName) },
                new System.Data.SQLite.SQLiteParameter("@DisplayName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.DisplayName) },
                new System.Data.SQLite.SQLiteParameter("@ControllerName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.ControllerName) },
                new System.Data.SQLite.SQLiteParameter("@ActionName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.ActionName) },
                new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", DbType.Int32) { Value = o.AppFuncLevelId },
                new System.Data.SQLite.SQLiteParameter("@ParentId", DbType.Int32) { Value = o.ParentId },
                new System.Data.SQLite.SQLiteParameter("@SequentialNum", DbType.Int32) { Value = o.SequentialNum },
                new System.Data.SQLite.SQLiteParameter("@IsDisabled", DbType.Boolean) { Value = o.IsDisabled },
                new System.Data.SQLite.SQLiteParameter("@IsNavItem", DbType.Boolean) { Value = o.IsNavItem },
                new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32) { Value = o.UpdatedBy },
                new System.Data.SQLite.SQLiteParameter("@Description", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(o.Description) }
                ));
        }

        public static IEnumerable<Models.AppFunction> GetAllParentList()
        {
            List<Models.AppFunction> rList;
            DataTable dt = null;
            try
            {
                rList = new List<Models.AppFunction>()
                {
                    new Models.AppFunction()
                    {
                        AppFunctionId = 0,
                        UniqueName = "system",
                        DisplayName = "System",
                        AppFuncLevelId = 0,
                        ParentId = 0,
                        SequentialNum = 0,
                        IsDisabled = false,
                        IsNavItem = false
                    }
                };
                string sql = "SELECT * FROM AppFunctions WHERE AppFuncLevelId<3 ORDER BY AppFuncLevelId,SequentialNum,DisplayName,UniqueName";
                if (DbHandler.IsMssqlConnected)
                { dt = DbHandler.MSSQL.SelectDataTable(sql); }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql);
                }
                if ((dt?.Rows.Count ?? 0) > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Models.AppFunction o = Mapping(dr);
                        if (o != null) { rList.Add(o); }
                    }
                }
                return rList;
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        public static List<Models.AppFunction> GetListByRoleId(int roleId)
        {
            string sql = "SELECT * FROM AppFunctions WHERE AppFunctionId IN (SELECT AppFunctionId FROM MapAppFunctionsRoles WHERE RoleId=@RoleId)";
            return DbHandler.IsMssqlConnected ?
                GetListBySqlFromMssql(sql, new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId }) :
                GetListBySqlFromSQLite(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = roleId });
        }

        /// Get a list of all users if they are selected by a specific role.
        public static List<Models.AppFunction> GetListSelectedByRoleId(int RoleId, int AppFuncLevelId, int ParentId)
        {
            //return GetListBySql("SELECT f.*,CASE WHEN (SELECT 1 FROM MapAppFunctionsRoles m WHERE m.AppFunctionId=f.AppFunctionId AND m.RoleId=@RoleId)=1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsSelected FROM AppFunctions f",
            //    new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId });

            //if (AppFuncLevelId > 3 || AppFuncLevelId < 0) { return null; }
            //DataTable dt = null;
            //try
            //{
            //    dt = DbHandler.MSSQL.SelectDataTable(
            //        "SELECT f.*,CASE WHEN (SELECT 1 FROM MapAppFunctionsRoles m WHERE m.AppFunctionId=f.AppFunctionId AND m.RoleId=@RoleId)=1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsSelected FROM AppFunctions f WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId ORDER BY SequentialNum,DisplayName,UniqueName",
            //        new SqlParameter("@RoleId", SqlDbType.Int) { Value = RoleId },
            //        new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = AppFuncLevelId },
            //        new SqlParameter("@ParentId", SqlDbType.Int) { Value = ParentId });
            //    if ((dt?.Rows.Count ?? 0) < 1) { return null; }
            //    List<Models.AppFunction> rList = new List<Models.AppFunction>();
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        Models.AppFunction o = Mapping(dr);
            //        if (o != null)
            //        {
            //            o.ChildList = GetListSelectedByRoleId(RoleId, AppFuncLevelId + 1, o.AppFunctionId);
            //            rList.Add(o);
            //        }
            //    }
            //    return rList;
            //}
            //finally
            //{
            //    DbHandler.DisposeDataTable(ref dt);
            //}

            if (AppFuncLevelId > 3 || AppFuncLevelId < 0) { return null; }
            string sql = "SELECT f.*,CASE WHEN (SELECT 1 FROM MapAppFunctionsRoles m WHERE m.AppFunctionId=f.AppFunctionId AND m.RoleId=@RoleId)=1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsSelected FROM AppFunctions f WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId ORDER BY SequentialNum,DisplayName,UniqueName";
            List<Models.AppFunction> list;
            if (DbHandler.IsMssqlConnected)
            {
                list = GetListBySqlFromMssql(sql,
                    new SqlParameter("@RoleId", SqlDbType.Int) { Value = RoleId },
                    new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = AppFuncLevelId },
                    new SqlParameter("@ParentId", SqlDbType.Int) { Value = ParentId });
            }
            else
            {
                list = GetListBySqlFromSQLite(sql,
                    new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = RoleId },
                    new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", DbType.Int32) { Value = AppFuncLevelId },
                    new System.Data.SQLite.SQLiteParameter("@ParentId", DbType.Int32) { Value = ParentId });
            }
            if ((list?.Count ?? 0) < 1) { return null; }
            foreach (Models.AppFunction o in list)
            {
                if (o != null)
                {
                    o.ChildList = GetListSelectedByRoleId(RoleId, AppFuncLevelId + 1, o.AppFunctionId);
                }
            }
            return list;
        }

        public static List<Models.AppFunction> GetListSelectedByAppFunctionId(int AppFuncLevelId, int ParentId, params int[] arrayOfAppFunctionId)
        {
            string s;
            if ((arrayOfAppFunctionId?.Length ?? 0) < 1) { s = "CAST(0 AS bit)"; }
            else { s = "CASE WHEN AppFunctionId IN (" + string.Join(",", arrayOfAppFunctionId.Select(i => i.ToString()).ToArray()) + ") THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END"; }
            string sql = "SELECT *," + s + " AS IsSelected FROM AppFunctions WHERE AppFuncLevelId=@AppFuncLevelId AND ParentId=@ParentId ORDER BY SequentialNum,DisplayName,UniqueName";
            List<Models.AppFunction> list;
            if (DbHandler.IsMssqlConnected)
            {
                list = GetListBySqlFromMssql(sql,
                    new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = AppFuncLevelId },
                    new SqlParameter("@ParentId", SqlDbType.Int) { Value = ParentId });
            }
            else
            {
                list = GetListBySqlFromSQLite(sql,
                    new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", DbType.Int32) { Value = AppFuncLevelId },
                    new System.Data.SQLite.SQLiteParameter("@ParentId", DbType.Int32) { Value = ParentId });
            }
            if ((list?.Count ?? 0) < 1) { return null; }
            foreach (Models.AppFunction o in list)
            {
                if (o != null)
                {
                    o.ChildList = GetListSelectedByAppFunctionId(AppFuncLevelId + 1, o.AppFunctionId, arrayOfAppFunctionId);
                }
            }
            return list;
        }
    }
}