using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class AppFuncLevelDao
    {
        private static Models.AppFuncLevel Mapping(DataRow dr)
        {
            object o = DbHandler.GetObjectFromDb(dr, "AppFuncLevelId");
            int? i = DbHandler.IsMssqlConnected ? (int?)o : (int?)(long?)o;
            return i == null ? null : new Models.AppFuncLevel()
            {
                AppFuncLevelId = i.GetValueOrDefault(),
                UniqueName = (string)DbHandler.GetObjectFromDb(dr, "UniqueName"),
                DisplayName = (string)DbHandler.GetObjectFromDb(dr, "DisplayName"),
                Description = (string)DbHandler.GetObjectFromDb(dr, "Description")
            };
        }

        public static Models.AppFuncLevel GetUnit(int id)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT * FROM AppFuncLevels WHERE AppFuncLevelId=@AppFuncLevelId";
                if (DbHandler.IsMssqlConnected)
                {
                    dt = DbHandler.MSSQL.SelectDataTable(sql,
                        new SqlParameter("@AppFuncLevelId", SqlDbType.Int) { Value = id });
                }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql,
                        new System.Data.SQLite.SQLiteParameter("@AppFuncLevelId", System.Data.DbType.Int32) { Value = id });
                }
                if ((dt?.Rows.Count ?? 0) > 0) { return null; }
                return Mapping(dt.Rows[0]);
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        public static Models.AppFuncLevel GetDefaultUnit() { return GetUnit(1); }

        public static List<Models.AppFuncLevel> GetAll()
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT * FROM AppFuncLevels ORDER BY AppFuncLevelId";
                if (DbHandler.IsMssqlConnected)
                {
                    dt = DbHandler.MSSQL.SelectDataTable(sql);
                }
                else
                {
                    dt = DbHandler.SQLite.SelectDataTable(sql);
                }
                if ((dt?.Rows.Count ?? 0) < 1) { return null; }
                List<Models.AppFuncLevel> rList = new List<Models.AppFuncLevel>();
                foreach (DataRow dr in dt.Rows)
                {
                    Models.AppFuncLevel o = Mapping(dr);
                    if (o != null) { rList.Add(o); }
                }
                return rList;
            }
            finally { DbHandler.DisposeDataTable(ref dt); }
        }

        //public static IEnumerable<Models.AppFuncLevel> GetSelectionList()
        //{
        //    return GetAll();
        //}

        /// Return value = id. If the record cannot be found, return -1.
        public static int GetIdByUniqueName(string uniqueName)
        {
            string sql = "SELECT AppFuncLevelId FROM AppFuncLevels WHERE UniqueName=@UniqueName";
            return DbHandler.IsMssqlConnected ?
                Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql, new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = DbHandler.GetObjectToDb(uniqueName) })) :
                Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql, new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = DbHandler.GetObjectToDb(uniqueName) }));
        }
    }
}