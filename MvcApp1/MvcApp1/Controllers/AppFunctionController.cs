using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp1.Controllers
{
    public class AppFunctionController : Controller
    {
        /// GET: AppFunction
        public ActionResult Index()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc")) { return RedirectToAction("NoAccess", "home"); }
            /// Get data from database.
            string userIdString = Session["UserId"].ToString();
            IList<Models.AppFunction> list = DAO.AppFunctionDao.GetList(1, 0, null, false);
            //ViewBag.TotalCount = list == null ? 0 : list.Count;
            ViewBag.CanCreate = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "appfunc_create");
            ViewBag.CanEdit = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "appfunc_edit");
            ViewBag.CanDelete = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "appfunc_delete");
            ViewBag.CanDetails = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "appfunc_details");
            return View(list);
        }

        /// http://localhost/AppFunction/details/1
        /// http://localhost/AppFunction/details?id=1
        public ActionResult Details(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_details")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.AppFunctionDao.GetUnit(id));
        }

        /// http://localhost/AppFunction/create
        public ActionResult Create()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_create")) { return RedirectToAction("NoAccess", "home"); }
            return View();
        }

        /// From Views/AppFunction/Create.cshtml
        /// To Views/AppFunction/CreateConfirm.cshtml
        [HttpPost]
        public ActionResult Create(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_create")) { return RedirectToAction("NoAccess", "home"); }
            if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "AppFunction is NULL.");
                return View(o);
            }
            /// check whether name already exists in database.
            if (string.IsNullOrEmpty(o.UniqueName)) { o.UniqueName = o.UniqueName.Trim(); }
            if (DAO.AppFunctionDao.UniqueNameExists(o.UniqueName))
            {
                ModelState.AddModelError(string.Empty, "Unique Name already exists.");
                return View(o);
            }
            return View("CreateConfirm", o);
        }

        /// From Views/AppFunction/CreateConfirm.cshtml
        /// To Views/AppFunction/CreateResult.cshtml
        [HttpPost]
        public ActionResult CreateConfirm(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_create")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null || string.IsNullOrWhiteSpace(o.UniqueName)) { return RedirectToAction("Create"); }
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int userId = DAO.UserDao.GetIdByLoginname("admin");
                if (userId < 0) { userId = 0; }
                o.UpdatedBy = o.CreatedBy = userId;
                /// Update database.
                int i = DAO.AppFunctionDao.InsertUnit(o);
                if (i == 1)
                {
                    return View("CreateResult", o);
                }
                else if (i < 1)
                {
                    ModelState.AddModelError(string.Empty, "Fail to create it in database.");
                    return View("Create", o);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "More than ONE record are created in database.");
                    return View("Create", o);
                }
            }
            else
            {
                return View("Create", o);
            }
        }

        /// From Views/AppFunction/CreateResult.cshtml
        [HttpPost]
        public ActionResult CreateResult(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_create")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }

        /// http://localhost/AppFunction/edit/1
        /// http://localhost/AppFunction/edit?id=1
        public ActionResult Edit(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_edit")) { return RedirectToAction("NoAccess", "home"); }
            Models.AppFunction o = DAO.AppFunctionDao.GetUnit(id);
            return View(o);
        }

        /// From Views/User/Edit.cshtml
        /// To Views/User/EditConfirm.cshtml
        [HttpPost]
        public ActionResult Edit(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_edit")) { return RedirectToAction("NoAccess", "home"); }
            if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "AppFunction is NULL.");
                return View(o);
            }
            /// check whether name already exists in database.
            if (string.IsNullOrEmpty(o.UniqueName)) { o.UniqueName = o.UniqueName.Trim(); }
            if (DAO.AppFunctionDao.UniqueNameExists(o.UniqueName, o.AppFunctionId))
            {
                ModelState.AddModelError(string.Empty, "Unique Name already exists.");
                return View(o);
            }
            return View("EditConfirm", o);
        }

        /// From Views/AppFunction/EditConfirm.cshtml
        /// To Views/AppFunction/EditResult.cshtml
        [HttpPost]
        public ActionResult EditConfirm(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_edit")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null || string.IsNullOrWhiteSpace(o.UniqueName)) { return RedirectToAction("Index"); }
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int userId = DAO.UserDao.GetIdByLoginname("admin");
                if (userId < 0) { userId = 0; }
                o.UpdatedBy = o.CreatedBy = userId;
                /// Update database.
                int i = DAO.AppFunctionDao.UpdateUnit(o);
                if (i == 1)
                {
                    return View("EditResult", o);
                }
                else if (i < 1)
                {
                    ModelState.AddModelError(string.Empty, "Fail to update it in database.");
                    return View("Edit", o);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "More than ONE record are updated in database.");
                    return View("Edit", o);
                }
            }
            else
            {
                return View("Edit", o);
            }
        }

        /// From Views/AppFunction/EditResult.cshtml
        [HttpPost]
        public ActionResult EditResult(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_edit")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }

        /// http://localhost/AppFunction/delete/1
        /// http://localhost/AppFunction/delete?id=1
        public ActionResult Delete(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_delete")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.AppFunctionDao.GetUnit(id));
        }

        /// http://localhost/AppFunction/delete
        [HttpPost]
        public ActionResult Delete(Models.AppFunction o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "appfunc_delete")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null) { return RedirectToAction("Index"); }
            /// update database.
            if (DAO.AppFunctionDao.DeleteUnit(o.AppFunctionId) < 1)
            {
                ModelState.AddModelError(string.Empty, "Fail to delete it in database.");
                return View(o);
            }
            return RedirectToAction("Index");
        }
    }
}