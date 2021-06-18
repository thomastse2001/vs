using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class MapAppFunctionsRolesDao
    {
        public static int AssignByRoleIdAndAppFunctionIdList(int updatedBy, int roleId, params int[] appFunctionIdArray)
        {
            if (roleId < 0) return 0;
            /// if no list, delete all records in table if RoleId=@RoleId.
            if ((appFunctionIdArray?.Length ?? 0) < 1)
            {
                string sql = "DELETE FROM MapAppFunctionsRoles WHERE RoleId=@RoleId";
                return DbHandler.IsMssqlConnected ?
                    DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId }):
                    DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = roleId });
            }
            else
            {
                //List<Models.AppFunction> deletedList = selectedAppFunctionList.FindAll(x => x.IsSelected == false);
                //if ((deletedList?.Count ?? 0) > 0)
                //{
                //    foreach (Models.AppFunction o in deletedList)
                //    {
                //        paramList.Add(new KeyValuePair<string, SqlParameter[]>(
                //            "DELETE FROM MapAppFunctionsRoles WHERE RoleId=@RoleId AND AppFunctionId=@AppFunctionId",
                //            new SqlParameter[]
                //            {
                //                new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId },
                //                new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = o.AppFunctionId }
                //            }));
                //    }
                //}
                //List<Models.AppFunction> addedList = selectedAppFunctionList.FindAll(x => x.IsSelected);
                //if ((addedList?.Count ?? 0) > 0)
                //{
                //    DateTime tRef = DateTime.Now;
                //    foreach (Models.AppFunction o in addedList)
                //    {
                //        paramList.Add(new KeyValuePair<string, SqlParameter[]>(
                //            "IF NOT EXISTS (SELECT 1 FROM MapAppFunctionsRoles WHERE RoleId=@RoleId AND AppFunctionId=@AppFunctionId) BEGIN INSERT INTO MapAppFunctionsRoles"
                //            + " (RoleId,AppFunctionId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) VALUES (@RoleId,@AppFunctionId,@CreatedDt,@CreatedBy,@UpdatedDt,@UpdatedBy) END",
                //            new SqlParameter[]
                //            {
                //                new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId },
                //                new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = o.AppFunctionId },
                //                new SqlParameter("@CreatedDt", SqlDbType.DateTime){Value = tRef},
                //                new SqlParameter("@CreatedBy", SqlDbType.Int){Value = updatedBy},
                //                new SqlParameter("@UpdatedDt", SqlDbType.DateTime){Value = tRef},
                //                new SqlParameter("@UpdatedBy", SqlDbType.Int){Value = updatedBy}
                //            }));
                //    }
                //}
                DateTime tRef = DateTime.Now;
                string sql1 = "DELETE FROM MapAppFunctionsRoles WHERE RoleId=@RoleId AND (NOT AppFunctionId IN (" + string.Join(",", appFunctionIdArray.Select(i => i.ToString()).ToArray()) + "))";
                string sql2 = "IF NOT EXISTS (SELECT 1 FROM MapAppFunctionsRoles WHERE RoleId=@RoleId AND AppFunctionId=@AppFunctionId) BEGIN INSERT INTO MapAppFunctionsRoles"
                        + " (RoleId,AppFunctionId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) VALUES (@RoleId,@AppFunctionId,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy) END";
                List<KeyValuePair<string, SqlParameter[]>> paramList = null;
                List<KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>> paramList2 = null;
                try
                {
                    if (DbHandler.IsMssqlConnected)
                    {
                        paramList = new List<KeyValuePair<string, SqlParameter[]>>()
                        {
                            new KeyValuePair<string, SqlParameter[]>(sql1,
                            new SqlParameter[]
                            {
                                new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId }
                            })
                        };
                        foreach (int i in appFunctionIdArray)
                        {
                            paramList.Add(new KeyValuePair<string, SqlParameter[]>(sql2,
                                new SqlParameter[]
                                {
                                    new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId },
                                    new SqlParameter("@AppFunctionId", SqlDbType.Int) { Value = i },
                                    new SqlParameter("@UpdatedDt", SqlDbType.DateTime) {Value = tRef },
                                    new SqlParameter("@UpdatedBy", SqlDbType.Int) {Value = updatedBy }
                                }));
                        }
                        return DbHandler.MSSQL.ExecuteNonQuery(paramList.ToArray());
                    }
                    else
                    {
                        paramList2 = new List<KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>>()
                        {
                            new KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>(sql1,
                            new System.Data.SQLite.SQLiteParameter[]
                            {
                                new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = roleId }
                            })
                        };
                        sql2 = "INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) SELECT @RoleId,@AppFunctionId,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy WHERE NOT EXISTS (SELECT 1 FROM MapAppFunctionsRoles WHERE RoleId=@RoleId AND AppFunctionId=@AppFunctionId)";
                        foreach (int i in appFunctionIdArray)
                        {
                            paramList2.Add(new KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>(sql2,
                                new System.Data.SQLite.SQLiteParameter[]
                                {
                                    new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32){ Value = roleId },
                                    new System.Data.SQLite.SQLiteParameter("@AppFunctionId", DbType.Int32){ Value = i },
                                    new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime){ Value = tRef },
                                    new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32){ Value = updatedBy }
                                }));
                        }
                        return DbHandler.SQLite.ExecuteNonQuery(paramList2.ToArray());
                    }
                }
                finally
                {
                    if (paramList != null) { paramList.Clear(); paramList = null; }
                    if (paramList2 != null) { paramList2.Clear(); paramList2 = null; }
                }
            }
        }

        public static bool CanAccessAppFunctionByUserId(int userId, string uniqueName)
        {
            if (string.IsNullOrWhiteSpace(uniqueName) || userId < 0) return false;
            string sql = "SELECT 1 FROM MapAppFunctionsRoles fr INNER JOIN AppFunctions f ON f.AppFunctionId=fr.AppFunctionId INNER JOIN MapRolesUsers ru ON ru.RoleId=fr.RoleId WHERE f.UniqueName=@UniqueName AND ru.UserId=@UserId";
            if (DbHandler.IsMssqlConnected)
            {
                return Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql,
                    new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = uniqueName },
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
                    )) > 0;
            }
            else
            {
                return Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql,
                    new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = uniqueName },
                    new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = userId }
                    )) > 0;
            }
        }

        public static bool CanAccessAppFunctionByLoginName(string loginName, string uniqueName)
        {
            if (string.IsNullOrWhiteSpace(uniqueName) || string.IsNullOrWhiteSpace(loginName)) return false;
            string sql = "SELECT 1 FROM MapAppFunctionsRoles fr INNER JOIN AppFunctions f ON f.AppFunctionId = fr.AppFunctionId INNER JOIN MapRolesUsers ru ON ru.RoleId = fr.RoleId INNER JOIN Users u ON u.UserId=ru.UserId WHERE f.UniqueName=@UniqueName AND u.LoginName=@LoginName";
            if (DbHandler.IsMssqlConnected)
            {
                return Convert.ToInt32(DbHandler.MSSQL.ExecuteScalar(sql,
                    new SqlParameter("@UniqueName", SqlDbType.VarChar) { Value = uniqueName },
                    new SqlParameter("@LoginName", SqlDbType.VarChar) { Value = loginName }
                    )) > 0;
            }
            else
            {
                return Convert.ToInt32(DbHandler.SQLite.ExecuteScalar(sql,
                    new System.Data.SQLite.SQLiteParameter("@UniqueName", DbType.AnsiString) { Value = uniqueName },
                    new System.Data.SQLite.SQLiteParameter("@LoginName", DbType.AnsiString) { Value = loginName }
                    )) > 0;
            }
        }
    }
}