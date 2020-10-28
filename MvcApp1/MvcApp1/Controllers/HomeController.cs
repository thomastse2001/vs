using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp1.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "home")) { return RedirectToAction("NoAccess", "home"); }

            ///// throw exception for demo.
            //throw new Exception("This is unhandled exception");

            /// Cannot get this data in the third request.
            /// http://www.tutorialsteacher.com/mvc/tempdata-in-asp.net-mvc
            TempData["LastVisited"] = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");

            return View();
        }

        /// http://localhost/home/index2
        public ActionResult Index2()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "index2")) { return RedirectToAction("NoAccess", "home"); }

            string LastVisited = "";
            if (TempData.ContainsKey("LastVisited")) { LastVisited = TempData["LastVisited"].ToString(); }
            ViewBag.LastVisited = LastVisited;

            return View();
        }

        public ActionResult About()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "about")) { return RedirectToAction("NoAccess", "home"); }

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "contact")) { return RedirectToAction("NoAccess", "home"); }

            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Health()
        {
            ViewBag.SqlServerIsConnected = DAO.DbHandler.MSSQL.CheckConnected();
            ViewBag.SQLiteIsConnected = DAO.DbHandler.SQLite.CheckConnected();
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Login(Models.LoginUser o)
        {
            if (!ModelState.IsValid)
            {
                return View(o);
            }
            DAO.DbHandler.CheckAndSetDb();
            Models.User validateUser = BAL.AccessBO.LoginAuthentication(o.LoginName, o.Password);
            if (validateUser == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot find user or wrong password.");
                return View(o);
            }
            /// https://www.aspsnippets.com/Articles/ASPNet-MVC-Keep-User-Logged-in-and-automatically-Login-User-using-Forms-Authentication-and-Cookies.aspx
            System.Web.Security.FormsAuthentication.SetAuthCookie(validateUser.LoginName, false);
            Session["UserId"] = validateUser.UserId.ToString();
            Session["LoginName"] = validateUser.LoginName;
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult NoAccess()
        {
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            return View();
        }
    }
}