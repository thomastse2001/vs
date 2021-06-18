using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApp1.DAO
{
    public class MapRolesUsersDao
    {
        public static int AssignByUserIdAndRoleSelectedList(int updatedBy, int userId, List<Models.Role> selectedRoleList)
        {
            if (userId < 0) return 0;
            if (selectedRoleList != null) selectedRoleList.RemoveAll(x => x == null);
            /// if no list, delete all records in table if UserId=@UserId.
            if ((selectedRoleList?.Count ?? 0) < 1)
            {
                string sql = "DELETE FROM MapRolesUsers WHERE UserId=@UserId";
                return DbHandler.IsMssqlConnected ?
                    DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }) :
                    DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32) { Value = userId });
            }
            else
            {
                string sql1 = "DELETE FROM MapRolesUsers WHERE UserId=@UserId AND RoleId=@RoleId";
                string sql2 = "IF NOT EXISTS (SELECT 1 FROM MapRolesUsers WHERE UserId=@UserId AND RoleId=@RoleId) BEGIN INSERT INTO MapRolesUsers"
                            + " (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) VALUES (@UserId,@RoleId,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy) END";
                DateTime tRef = DateTime.Now;
                List<KeyValuePair<string, SqlParameter[]>> paramList = null;
                List<KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>> paramList2 = null;
                List<Models.Role> deletedList = selectedRoleList.FindAll(x => x.IsSelected == false);
                List<Models.Role> addedList = selectedRoleList.FindAll(x => x.IsSelected);
                try
                {
                    if (DbHandler.IsMssqlConnected)
                    {
                        paramList = new List<KeyValuePair<string, SqlParameter[]>>();
                        if ((deletedList?.Count ?? 0) > 0)
                        {
                            foreach (Models.Role o in deletedList)
                            {
                                paramList.Add(new KeyValuePair<string, SqlParameter[]>(sql1,
                                    new SqlParameter[]
                                    {
                                        new SqlParameter("@UserId", SqlDbType.Int){Value = userId},
                                        new SqlParameter("@RoleId", SqlDbType.Int){Value = o.RoleId}
                                    }));
                            }
                        }
                        if ((addedList?.Count ?? 0) > 0)
                        {
                            foreach (Models.Role o in addedList)
                            {
                                paramList.Add(new KeyValuePair<string, SqlParameter[]>(sql2,
                                    new SqlParameter[]
                                    {
                                        new SqlParameter("@UserId", SqlDbType.Int){Value = userId},
                                        new SqlParameter("@RoleId", SqlDbType.Int){Value = o.RoleId},
                                        new SqlParameter("@UpdatedDt", SqlDbType.DateTime){Value = tRef},
                                        new SqlParameter("@UpdatedBy", SqlDbType.Int){Value = updatedBy}
                                    }));
                            }
                        }
                        return paramList.Count < 1 ? 0 : DbHandler.MSSQL.ExecuteNonQuery(paramList.ToArray());
                    }
                    else
                    {
                        paramList2 = new List<KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>>();
                        if ((deletedList?.Count ?? 0) > 0)
                        {
                            foreach (Models.Role o in deletedList)
                            {
                                paramList2.Add(new KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>(sql1,
                                    new System.Data.SQLite.SQLiteParameter[]
                                    {
                                        new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32){Value = userId },
                                        new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32){Value = o.RoleId}
                                    }));
                            }
                        }
                        if ((addedList?.Count ?? 0) > 0)
                        {
                            sql2 = "INSERT INTO MapRolesUsers (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) SELECT @UserId,@RoleId,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy WHERE NOT EXISTS (SELECT 1 FROM MapRolesUsers WHERE UserId=@UserId AND RoleId=@RoleId)";
                            foreach (Models.Role o in addedList)
                            {
                                paramList2.Add(new KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>(sql2,
                                    new System.Data.SQLite.SQLiteParameter[]
                                    {
                                        new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32){ Value=userId },
                                        new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32){ Value=o.RoleId },
                                        new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime){ Value=tRef },
                                        new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32){ Value=updatedBy },
                                    }));
                            }
                        }
                        return paramList2.Count < 1 ? 0 : DbHandler.SQLite.ExecuteNonQuery(paramList2.ToArray());
                    }
                }
                finally
                {
                    if (paramList != null) { paramList.Clear(); paramList = null; }
                    if (paramList2 != null) { paramList2.Clear(); paramList2 = null; }
                }
            }
        }

        public static int AssignByRoleIdAndUserSelectedList(int updatedBy, int roleId, List<Models.User> selectedUserList)
        {
            if (roleId < 0) return 0;
            if (selectedUserList != null) selectedUserList.RemoveAll(x => x == null);
            /// if no list, delete all records in table if RoleId=@RoleId.
            if ((selectedUserList?.Count ?? 0) < 1)
            {
                string sql = "DELETE FROM MapRolesUsers WHRE RoleId=@RoleId";
                return DbHandler.IsMssqlConnected ?
                    DbHandler.MSSQL.ExecuteNonQuery(sql, new SqlParameter("@RoleId", SqlDbType.Int) { Value = roleId }):
                    DbHandler.SQLite.ExecuteNonQuery(sql, new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32) { Value = roleId });
            }
            else
            {
                DateTime tRef = DateTime.Now;
                string sql1 = "DELETE FROM MapRolesUsers WHERE UserId=@UserId AND RoleId=@RoleId";
                string sql2 = "IF NOT EXISTS (SELECT 1 FROM MapRolesUsers WHERE UserId=@UserId AND RoleId=@RoleId) BEGIN INSERT INTO MapRolesUsers"
                    + " (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) VALUES (@UserId,@RoleId,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy) END";
                List<KeyValuePair<string, SqlParameter[]>> paramList = null;
                List<KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>> paramList2 = null;
                List<Models.User> deletedList = selectedUserList.FindAll(x => x.IsSelected == false);
                List<Models.User> addedList = selectedUserList.FindAll(x => x.IsSelected);
                try
                {
                    if (DbHandler.IsMssqlConnected)
                    {
                        paramList = new List<KeyValuePair<string, SqlParameter[]>>();
                        if ((deletedList?.Count ?? 0) > 0)
                        {
                            foreach (Models.User o in deletedList)
                            {
                                paramList.Add(new KeyValuePair<string, SqlParameter[]>(sql1,
                                    new SqlParameter[]
                                    {
                                        new SqlParameter("@RoleId", SqlDbType.Int){Value=roleId},
                                        new SqlParameter("@UserId", SqlDbType.Int){Value=o.UserId}
                                    }));
                            }
                        }
                        if ((addedList?.Count ?? 0) > 0)
                        {
                            foreach (Models.User o in addedList)
                            {
                                paramList.Add(new KeyValuePair<string, SqlParameter[]>(sql2,
                                    new SqlParameter[]
                                    {
                                        new SqlParameter("@UserId", SqlDbType.Int){Value = o.UserId},
                                        new SqlParameter("@RoleId", SqlDbType.Int){Value = roleId},
                                        new SqlParameter("@UpdatedDt", SqlDbType.DateTime){Value = tRef},
                                        new SqlParameter("@UpdatedBy", SqlDbType.Int){Value = updatedBy}
                                    }));
                            }
                        }
                        return paramList.Count < 1 ? 0 : DbHandler.MSSQL.ExecuteNonQuery(paramList.ToArray());
                    }
                    else
                    {
                        paramList2 = new List<KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>>();
                        if ((deletedList?.Count ?? 0) > 0)
                        {
                            foreach (Models.User o in deletedList)
                            {
                                paramList2.Add(new KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>(sql1,
                                    new System.Data.SQLite.SQLiteParameter[]
                                    {
                                        new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32){Value=roleId},
                                        new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32){Value=o.UserId}
                                    }));
                            }
                        }
                        if ((addedList?.Count ?? 0) > 0)
                        {
                            sql2 = "INSERT INTO MapRolesUsers (UserId,RoleId,CreatedDt,CreatedBy,UpdatedDt,UpdatedBy) SELECT @UserId,@RoleId,@UpdatedDt,@UpdatedBy,@UpdatedDt,@UpdatedBy WHERE NOT EXISTS (SELECT 1 FROM MapRolesUsers WHERE UserId=@UserId AND RoleId=@RoleId)";
                            foreach (Models.User o in addedList)
                            {
                                paramList2.Add(new KeyValuePair<string, System.Data.SQLite.SQLiteParameter[]>(sql2,
                                    new System.Data.SQLite.SQLiteParameter[]
                                    {
                                        new System.Data.SQLite.SQLiteParameter("@UserId", DbType.Int32){Value=o.UserId},
                                        new System.Data.SQLite.SQLiteParameter("@RoleId", DbType.Int32){Value=roleId},
                                        new System.Data.SQLite.SQLiteParameter("@UpdatedDt", DbType.DateTime){Value=tRef},
                                        new System.Data.SQLite.SQLiteParameter("@UpdatedBy", DbType.Int32){Value=updatedBy}
                                    }));
                            }
                        }
                        return paramList2.Count < 1 ? 0 : DbHandler.SQLite.ExecuteNonQuery(paramList2.ToArray());
                    }
                }
                finally
                {
                    if (paramList != null) { paramList.Clear(); paramList = null; }
                    if (paramList2 != null) { paramList2.Clear(); paramList2 = null; }
                }
            }
        }
    }
}