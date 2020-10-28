using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp1.Controllers
{
    public class UserController : Controller
    {
        /// GET: User
        public ActionResult Index()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user")) { return RedirectToAction("NoAccess", "home"); }

            /// Get data from database.
            string userIdString = Session["UserId"].ToString();
            IList<Models.User> list = DAO.UserDao.GetAll();
            ViewBag.TotalCount = list == null ? 0 : list.Count;
            ViewBag.CanCreate = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "user_create");
            ViewBag.CanEdit = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "user_edit");
            ViewBag.CanDelete = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "user_delete");
            ViewBag.CanDetails = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "user_details");
            ViewBag.CanAssignRoles = BAL.AccessBO.CanAccessAppFunctionByUserId(userIdString, "user_assign_roles");
            //return View();
            return View(list);
        }

        /// http://localhost/user/details/1
        /// http://localhost/user/details?id=1
        public ActionResult Details(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_details")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.UserDao.GetUnit(id));
        }

        /// http://localhost/user/create
        public ActionResult Create()
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_create")) { return RedirectToAction("NoAccess", "home"); }
            return View();
        }

        /// From Views/User/Create.cshtml
        /// To Views/User/CreateConfirm.cshtml
        [HttpPost]
        public ActionResult Create(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_create")) { return RedirectToAction("NoAccess", "home"); }
            if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "User is NULL.");
                return View(o);
            }
            /// check whether name already exists in database.
            if (string.IsNullOrEmpty(o.LoginName)) { o.LoginName = o.LoginName.Trim(); }
            if (DAO.UserDao.LoginNameExists(o.LoginName))
            {
                ModelState.AddModelError(string.Empty, "Login Name already exists.");
                return View(o);
            }
            /// Password.
            //if (o.Password == null) { o.Password = ""; }
            o.Hash = BAL.CommonHelper.ComputeHashFromString(o.Password ?? "");
            o.Password = o.RetypedPassword = null;
            return View("CreateConfirm", o);
        }

        /// From Views/User/CreateConfirm.cshtml
        /// To Views/User/CreateResult.cshtml
        [HttpPost]
        public ActionResult CreateConfirm(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_create")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null || string.IsNullOrWhiteSpace(o.LoginName)) { return RedirectToAction("Create"); }
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int userId = DAO.UserDao.GetIdByLoginname("admin");
                if (userId < 0) { userId = 0; }
                o.UpdatedBy = o.CreatedBy = userId;

                /// Update database.
                int i = DAO.UserDao.InsertUnit(o);
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

        /// From Views/User/CreateResult.cshtml
        [HttpPost]
        public ActionResult CreateResult(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_create")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }

        /// http://localhost/user/edit/1
        /// http://localhost/user/edit?id=1
        public ActionResult Edit(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_edit")) { return RedirectToAction("NoAccess", "home"); }
            //var std = studentList.Where(s => s != null && s.StudentId == id).FirstOrDefault();
            //return View(StudentDao.GetRecordById(id));
            Models.User o = DAO.UserDao.GetUnit(id);
            o.Password = o.RetypedPassword = null;
            return View(o);
        }

        /// From Views/User/Edit.cshtml
        /// To Views/User/EditConfirm.cshtml
        [HttpPost]
        public ActionResult Edit(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_edit")) { return RedirectToAction("NoAccess", "home"); }
            if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "User is NULL.");
                return View(o);
            }
            /// check whether name already exists in database.
            if (string.IsNullOrEmpty(o.LoginName)) { o.LoginName = o.LoginName.Trim(); }
            if (DAO.UserDao.LoginNameExists(o.LoginName, o.UserId))
            {
                ModelState.AddModelError(string.Empty, "Login Name already exists.");
                return View(o);
            }
            /// Password.
            if (o.IsUpdateHash)
            {
                //if (o.Password == null) { o.Password = ""; }
                o.Hash = BAL.CommonHelper.ComputeHashFromString(o.Password ?? "");
                o.Password = o.RetypedPassword = null;
            }
            return View("EditConfirm", o);
        }

        /// From Views/User/EditConfirm.cshtml
        /// To Views/User/EditResult.cshtml
        [HttpPost]
        public ActionResult EditConfirm(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_edit")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null || string.IsNullOrWhiteSpace(o.LoginName)) { return RedirectToAction("Index"); }

            ////// check whether name already exists in database.
            ////if (string.IsNullOrEmpty(o.LoginName)) { o.LoginName = o.LoginName.Trim(); }
            ////bool nameExists = DAO.UserDao.LoginNameExists(o.LoginName, o.UserId);
            ////if (nameExists)
            ////{
            ////    ModelState.AddModelError(string.Empty, "Login Name already exists.");
            ////    return View("Edit", o);
            ////    //return RedirectToAction("Edit", o);
            ////}

            ////// Password.
            ////if (o.Password == null) { o.Password = ""; }
            ////o.Hash = BAL.CommonHelper.ComputeHashFromString(o.Password);
            ////o.Password = o.RetypedPassword = null;

            ////int currentUiStatus = 0;
            ////if (int.TryParse(BAL.CommonHelper.GetSessionValue(Request, "UiStatus"), out currentUiStatus)) { }

            ////if (BAL.CommonHelper.UiStatus.Progress == currentUiStatus) { return View(o); }
            ////else if (BAL.CommonHelper.UiStatus.Confirm == currentUiStatus)
            ////{
            ////    // If clicking the "Confirm".
            ////    if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            ////    {
            ////        int userId = DAO.UserDao.GetIdByLoginname("admin");
            ////        if (userId < 0) { userId = 0; }
            ////        o.UpdatedBy = o.CreatedBy = userId;

            ////        // Update database.
            ////        int i = DAO.UserDao.UpdateRecord(o);
            ////        if (i == 1)
            ////        {
            ////            return View("EditResult", o);
            ////        }
            ////        else if (i < 1)
            ////        {
            ////            ModelState.AddModelError(string.Empty, "Fail to update it in database.");
            ////            return View("Edit", o);
            ////        }
            ////        else
            ////        {
            ////            ModelState.AddModelError(string.Empty, "More than ONE record are updated in database.");
            ////            return View("Edit", o);
            ////        }
            ////    }
            ////    else
            ////    {
            ////        return View("Edit", o);
            ////    }
            ////}
            ////else { return RedirectToAction("Index"); }

            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int userId = DAO.UserDao.GetIdByLoginname("admin");
                if (userId < 0) { userId = 0; }
                o.UpdatedBy = o.CreatedBy = userId;
                /// Update database.
                int i = DAO.UserDao.UpdateUnit(o);
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

        /// From Views/User/EditResult.cshtml
        [HttpPost]
        public ActionResult EditResult(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_edit")) { return RedirectToAction("NoAccess", "home"); }
            ////int currentUiStatus = 0;
            ////if (int.TryParse(BAL.CommonHelper.GetSessionValue(Request, "UiStatus"), out currentUiStatus)) { }
            ////if (o == null || string.IsNullOrWhiteSpace(o.LoginName) || currentUiStatus != BAL.CommonHelper.UiStatus.Result) { return View("Edit", o); }
            return View(o);
        }

        /// http://localhost/user/delete/1
        /// http://localhost/user/delete?id=1
        public ActionResult Delete(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_delete")) { return RedirectToAction("NoAccess", "home"); }
            return View(DAO.UserDao.GetUnit(id));
        }

        /// http://localhost/user/delete
        [HttpPost]
        public ActionResult Delete(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_delete")) { return RedirectToAction("NoAccess", "home"); }
            if (o == null) { return RedirectToAction("Index"); }
            /// update database.
            if (DAO.UserDao.DeleteUnit(o.UserId) < 1)
            {
                ModelState.AddModelError(string.Empty, "Fail to delete it in database.");
                return View(o);
            }
            return RedirectToAction("Index");
        }

        public ActionResult AssignRolesToUser(int id)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_assign_roles")) { return RedirectToAction("NoAccess", "home"); }
            Models.User user = DAO.UserDao.GetUnit(id);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User is NULL.");
                return View();
            }
            user.RoleList = DAO.RoleDao.GetListSelectedByUserId(id);
            ViewBag.selectedRoleIdString = string.Join(",", user.RoleList?.FindAll(x => x.IsSelected)?.Select(x => x.RoleId.ToString())?.ToArray());
            return View(user);
        }

        [HttpPost]
        public ActionResult AssignRolesToUser(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_assign_roles")) { return RedirectToAction("NoAccess", "home"); }
            //if (!ModelState.IsValid) { return View(o); }
            if (o == null)
            {
                ModelState.AddModelError(string.Empty, "User is NULL.");
                return View(o);
            }
            string selectedValuesString = BAL.CommonHelper.GetSessionValue(Request, "UiSelectedValues");
            o.RoleList = DAO.RoleDao.GetListSelectedByRoleId(BAL.CommonHelper.ConvertStringToIntArray(selectedValuesString, ","));
            ViewBag.selectedRoleIdString = string.Join(",", o.RoleList?.FindAll(x => x != null && x.IsSelected)?.Select(x => x.RoleId.ToString())?.ToArray());
            /// https://www.pluralsight.com/guides/asp.net-mvc-getting-default-data-binding-right-for-hierarchical-views
            return View("AssignRolesToUserConfirm", o);
        }

        [HttpPost]
        public ActionResult AssignRolesToUserConfirm(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_assign_roles")) { return RedirectToAction("NoAccess", "home"); }
            string selectedValuesString = BAL.CommonHelper.GetSessionValue(Request, "UiSelectedValues");
            /// If clicking the "Confirm".
            if ("confirm".Equals(BAL.CommonHelper.GetSessionValue(Request, "UiConfirm")?.ToLower()))
            {
                int[] intArray = BAL.CommonHelper.ConvertStringToIntArray(selectedValuesString, ",");
                List<Models.Role> roleList = null;
                if (intArray != null && intArray.Length > 0)
                {
                    roleList = DAO.RoleDao.GetAll();
                    foreach (int i in intArray)
                    {
                        roleList.FindAll(x => x.RoleId == i)?.ForEach(x =>
                        {
                            x.IsSelected = true;
                        });
                    }
                }
                int iReturn = DAO.MapRolesUsersDao.AssignByUserIdAndRoleSelectedList(0, o.UserId, roleList);
                o.RoleList = roleList;
                return View("AssignRolesToUserResult", o);
            }
            else
            {
                o.RoleList = DAO.RoleDao.GetListSelectedByRoleId(BAL.CommonHelper.ConvertStringToIntArray(selectedValuesString, ","));
                ViewBag.selectedRoleIdString = string.Join(",", o.RoleList?.FindAll(x => x.IsSelected)?.Select(x => x.RoleId.ToString())?.ToArray());
                return View("AssignRolesToUser", o);
            }
        }

        /// From Views/User/AsignRolesToUserResult.cshtml
        [HttpPost]
        public ActionResult AssignRolesToUserResult(Models.User o)
        {
            /// Authentication and authorisation.
            if (Session["UserId"] == null) { return RedirectToAction("Login", "home"); }
            if (!BAL.AccessBO.CanAccessAppFunctionByUserId(Session["UserId"].ToString(), "user_assign_roles")) { return RedirectToAction("NoAccess", "home"); }
            return View(o);
        }
    }
}