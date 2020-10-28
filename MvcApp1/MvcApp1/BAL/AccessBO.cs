using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

namespace MvcApp1.BAL
{
    public class AccessBO
    {
        public static Models.User LoginAuthentication(string loginname, string password)
        {
            return DAO.UserDao.LoginAuthentication(loginname, password);
        }

        public static bool CanAccessAppFunctionByUserId(string userIdString, string uniqueName)
        {
            if (int.TryParse(userIdString, out int userId))
            {
                return DAO.MapAppFunctionsRolesDao.CanAccessAppFunctionByUserId(userId, uniqueName);
            }
            return false;
        }

        public static bool CanAccessAppFunctionByLoginName(string loginName, string uniqueName)
        {
            return DAO.MapAppFunctionsRolesDao.CanAccessAppFunctionByLoginName(loginName, uniqueName);
        }
    }
}