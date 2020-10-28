using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp1.Controllers
{
    public class RoleController : Controller
    {
        /// GET: Role
        public ActionResult Index()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role")) { return RedirectToAction("NoAccess", "home"); }
            /// Get data from database.
            string userIdString = Session["UserId"].ToString();
            IList<Models.Role> list = DAO.RoleDao.GetAll();
            ViewBag.TotalCount = list == null ? 0 : list.Count;
            ViewBag.CanCreate = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "role_create");
            ViewBag.CanEdit = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "role_edit");
            ViewBag.CanDelete = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "role_delete");
            ViewBag.CanDetails = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "role_details");
            ViewBag.CanAssignUsers = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "role_assign_users");
            ViewBag.CanAssignFunc = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "role_assign_func");
            return View(list);
        }

        /// http://localhost/role/details/1
        /// http://localhost/role/details?id=1
        public ActionResult Details(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_details")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.RoleDao.GetUnit(id));
        }

        /// http://localhost/role/create
        public ActionResult Create()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_create")) { return RedirectToAction("NoAccess", "home"); }
            return View();
        }

        /// From Views/Role/Create.cshtml
        /// To Views/Role/CreateConfirm.cshtml
        [HttpPost]
        public ActionResult Create(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_create")) { return RedirectToAction("NoAccess", "home"); }
            if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "Role is NULL.");
                return View(o);
            }
            /// check whether name already exists in database.
            if (string.IsNullOrEmpty(o.UniqueName)) { o.UniqueName = o.UniqueName.Trim(); }
            if (DAO.RoleDao.UniqueNameExists(o.UniqueName))
            {
                ModelState.AddModelError(string.Empty, "Unique Name already exists.");
                return View(o);
            }
            return View("CreateConfirm", o);
        }

        /// From Views/Role/CreateConfirm.cshtml
        /// To Views/Role/CreateResult.cshtml
        [HttpPost]
        public ActionResult CreateConfirm(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_create")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null || string.IsNullOrWhiteSpace(o.UniqueName)) { return RedirectToAction("Create"); }
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int userId = DAO.UserDao.GetIdByLoginname("admin");
                if (userId < 0) { userId = 0; }
                o.UpdatedBy = o.CreatedBy = userId;
                /// Update database.
                int i = DAO.RoleDao.InsertUnit(o);
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

        /// From Views/Role/CreateResult.cshtml
        [HttpPost]
        public ActionResult CreateResult(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_create")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }

        /// http://localhost/role/edit/1
        /// http://localhost/role/edit?id=1
        public ActionResult Edit(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_edit")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.RoleDao.GetUnit(id));
        }

        /// From Views/Role/Edit.cshtml
        /// To Views/Role/EditConfirm.cshtml
        [HttpPost]
        public ActionResult Edit(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_edit")) { return RedirectToAction("NoAccess", "home"); }
            if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "Role is NULL.");
                return View(o);
            }
            /// check whether name already exists in database.
            if (string.IsNullOrEmpty(o.UniqueName)) { o.UniqueName = o.UniqueName.Trim(); }
            if (DAO.RoleDao.UniqueNameExists(o.UniqueName, o.RoleId))
            {
                ModelState.AddModelError(string.Empty, "Unique Name already exists.");
                return View(o);
            }
            return View("EditConfirm", o);
        }

        /// From Views/Role/EditConfirm.cshtml
        /// To Views/Role/EditResult.cshtml
        [HttpPost]
        public ActionResult EditConfirm(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_edit")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null || string.IsNullOrWhiteSpace(o.UniqueName)) { return RedirectToAction("Index"); }
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int userId = DAO.UserDao.GetIdByLoginname("admin");
                if (userId < 0) { userId = 0; }
                o.UpdatedBy = o.CreatedBy = userId;
                /// Update database.
                int i = DAO.RoleDao.UpdateUnit(o);
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

        /// From Views/Role/EditResult.cshtml
        [HttpPost]
        public ActionResult EditResult(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_edit")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }

        /// http://localhost/role/delete/1
        /// http://localhost/role/delete?id=1
        public ActionResult Delete(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_delete")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.RoleDao.GetUnit(id));
        }

        /// http://localhost/role/delete
        [HttpPost]
        public ActionResult Delete(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_delete")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null) { return RedirectToAction("Index"); }
            /// update database.
            if (DAO.RoleDao.DeleteUnit(o.RoleId) < 1)
            {
                ModelState.AddModelError(string.Empty, "Fail to delete it in database.");
                return View(o);
            }
            return RedirectToAction("Index");
        }

        public ActionResult AssignUsersToRole(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_users")) { return RedirectToAction("NoAccess", "home"); }
            Models.Role role = DAO.RoleDao.GetUnit(id);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Role is NULL.");
                return View();
            }
            role.UserList = DAO.UserDao.GetListSelectedByRoleId(id);
            ViewBag.selectedUserIdString = string.Join(",", role.UserList?.FindAll(x => x.IsSelected)?.Select(x => x.UserId.ToString())?.ToArray());
            return View(role);
        }

        [HttpPost]
        public ActionResult AssignUsersToRole(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_users")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "Role is NULL.");
                return View(o);
            }
            string selectedValuesString = BAL.CommonHelper.GetSessionValue(Request, "UiSelectedValues");
            o.UserList = DAO.UserDao.GetListSelectedByUserId(BAL.CommonHelper.ConvertStringToIntArray(selectedValuesString, ","));
            ViewBag.selectedUserIdString = string.Join(",", o.UserList?.FindAll(x => x != null && x.IsSelected)?.Select(x => x.UserId.ToString())?.ToArray());
            /// https://www.pluralsight.com/guides/asp.net-mvc-getting-default-data-binding-right-for-hierarchical-views
            return View("AssignUsersToRoleConfirm", o);
        }

        [HttpPost]
        public ActionResult AssignUsersToRoleConfirm(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_users")) { return RedirectToAction("NoAccess", "home"); }
            string selectedValuesString = BAL.CommonHelper.GetSessionValue(Request, "UiSelectedValues");
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int[] intArray = BAL.CommonHelper.ConvertStringToIntArray(selectedValuesString, ",");
                List<Models.User> userList = null;
                if(intArray != null && intArray.Length > 0)
                {
                    userList = DAO.UserDao.GetAll();
                    foreach(int i in intArray)
                    {
                        userList.FindAll(x => x.UserId == i)?.ForEach(x =>
                         {
                             x.IsSelected = true;
                         });
                    }
                }
                int iReturn = DAO.MapRolesUsersDao.AssignByRoleIdAndUserSelectedList(0, o.RoleId, userList);
                o.UserList = userList;
                return View("AssignUsersToRoleResult", o);
            }
            else
            {
                o.UserList = DAO.UserDao.GetListSelectedByUserId(BAL.CommonHelper.ConvertStringToIntArray(selectedValuesString, ","));
                ViewBag.selectedUserIdString = string.Join(",", o.UserList?.FindAll(x => x.IsSelected)?.Select(x => x.UserId.ToString())?.ToArray());
                return View("AssignUsersToRole", o);
            }
        }

        /// From Views/User/AssignUsersToRoleResult.cshtml
        [HttpPost]
        public ActionResult AssignUsersToRoleResult(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_users")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }

        public ActionResult AssignFuncToRole(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_func")) { return RedirectToAction("NoAccess", "home"); }
            Models.Role role = DAO.RoleDao.GetUnit(id);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Role is NULL.");
                return View();
            }
            role.AppFunctionList = DAO.AppFunctionDao.GetListSelectedByRoleId(id, 1, 0);
            return View(role);
        }

        [HttpPost]
        public ActionResult AssignFuncToRole(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_func")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "Role is NULL.");
                return View(o);
            }
            int[] intArray = BAL.CommonHelper.ConvertStringToIntArray(BAL.CommonHelper.GetSessionValue(Request, "IsSelectedArray"), ",");
            o.AppFunctionList = DAO.AppFunctionDao.GetListSelectedByAppFunctionId(1, 0, intArray);
            return View("AssignFuncToRoleConfirm", o);
        }

        [HttpPost]
        public ActionResult AssignFuncToRoleConfirm(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_func")) { return RedirectToAction("NoAccess", "home"); }
            int[] intArray = BAL.CommonHelper.ConvertStringToIntArray(BAL.CommonHelper.GetSessionValue(Request, "IsSelectedArray"), ",");
            o.AppFunctionList = DAO.AppFunctionDao.GetListSelectedByAppFunctionId(1, 0, intArray);
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int iReturn = DAO.MapAppFunctionsRolesDao.AssignByRoleIdAndAppFunctionIdList(0, o.RoleId, intArray);
                return View("AssignFuncToRoleResult", o);
            }
            else
            {
                return View("AssignFuncToRole", o);
            }
        }

        [HttpPost]
        public ActionResult AssignFuncToRoleResult(Models.Role o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "role_assign_func")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }
    }
}