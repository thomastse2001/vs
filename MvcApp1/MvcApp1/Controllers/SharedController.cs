using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp1.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HeaderNavBar()
        {
            if (int.TryParse(Session["UserId"]?.ToString(), out int userId))
            {
                return PartialView("_HeadernavBar", DAO.AppFunctionDao.GetNavigationListByUserId(userId, 1, 0));
            }
            return PartialView("_HeadernavBar", null);
        }
    }
}