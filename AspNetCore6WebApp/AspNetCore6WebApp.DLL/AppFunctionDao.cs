using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface IAppFunctionDao
    {
        public int AddOrUpdate(AppFunction o);
        AppFunction? Delete(int id);
        AppFunction? GetById(int id);
        IEnumerable<AppFunction>? GetByCriteria(int appFuncLevelId, int parentId, bool? isNavItem);
        bool UniqueNameExists(string uniqueName);
        bool UniqueNameExists(string uniqueName, int id);
    }

    public class AppFunctionDao : IAppFunctionDao
    {
        private readonly AspNetCore6WebAppDbContext db;
        private readonly IAppFuncLevelDao _appFuncLevelDao;
        private readonly IUserDao _userDao;

        public AppFunctionDao(AspNetCore6WebAppDbContext db, IAppFuncLevelDao appFuncLevelDao, IUserDao userDao)
        {
            this.db = db;
            this._appFuncLevelDao = appFuncLevelDao;
            this._userDao = userDao;
        }

        /// Return value = id
        public int AddOrUpdate(AppFunction o)
        {
            if (o.AppFunctionId == 0)
            {
                db.Add(o);
            }
            else
            {
                var entity = db.AppFunctions.Attach(o);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            db.SaveChanges();
            return o.AppFunctionId;
        }

        public AppFunction? Delete(int id)
        {
            var o = GetById(id);
            if (o != null) db.AppFunctions.Remove(o);
            db.SaveChanges();
            return o;
        }

        public AppFunction? GetById(int id)
        {
            var o = db.AppFunctions.Find(id);
            if (o != null)
            {
                o.AppFuncLevelDisplayName = _appFuncLevelDao.GetById(o.AppFuncLevelId)?.DisplayName;
                o.CreatedByDisplayName = _userDao.GetById(o.CreatedBy.GetValueOrDefault())?.DisplayName;
                o.UpdatedByDisplayName = _userDao.GetById(o.UpdatedBy.GetValueOrDefault())?.DisplayName;
            }
            return o;
        }

        private IQueryable<AppFunction>? GetCommonQuery()
        {
            return from o in db.AppFunctions
                   join l in db.AppFuncLevels on o.AppFuncLevelId equals l.AppFuncLevelId into ls
                   from l in ls.DefaultIfEmpty()
                   join c in db.Users on o.CreatedBy equals c.UserId into cs
                   from c in cs.DefaultIfEmpty()
                   join u in db.Users on o.UpdatedBy equals u.UserId into us
                   from u in us.DefaultIfEmpty()
                   select new AppFunction
                   {
                       AppFunctionId = o.AppFunctionId,
                       UniqueName = o.UniqueName,
                       DisplayName = o.DisplayName,
                       ControllerName = o.ControllerName,
                       ActionName = o.ActionName,
                       AppFuncLevelId = o.AppFuncLevelId,
                       ParentId = o.ParentId,
                       SequentialNum = o.SequentialNum,
                       IsDisabled = o.IsDisabled,
                       IsNavItem = o.IsNavItem,
                       CreatedDt = o.CreatedDt,
                       CreatedBy = o.CreatedBy,
                       UpdatedDt = o.UpdatedDt,
                       UpdatedBy = o.UpdatedBy,
                       Description = o.Description,
                       AppFuncLevelDisplayName = l.DisplayName,
                       CreatedByDisplayName = c.DisplayName,
                       UpdatedByDisplayName = u.DisplayName,
                       ChildList = null // GetByCriteria(appFuncLevelId + 1, o.AppFunctionId, isNavItem)
                   };
        }

        public IEnumerable<AppFunction>? GetByCriteria(int appFuncLevelId, int parentId, bool? isNavItem)
        {
            if (appFuncLevelId < 0 || appFuncLevelId > 3) return null;// Enumerable.Empty<AppFunction>();
            //var list = from o in db.AppFunctions
            //       join l in db.AppFuncLevels on o.AppFuncLevelId equals l.AppFuncLevelId into ls
            //       from l in ls.DefaultIfEmpty()
            //       join c in db.Users on o.CreatedBy equals c.UserId into cs
            //       from c in cs.DefaultIfEmpty()
            //       join u in db.Users on o.UpdatedBy equals u.UserId into us
            //       from u in us.DefaultIfEmpty()
            //       where o.AppFuncLevelId == appFuncLevelId && o.ParentId == parentId
            //       && (isNavItem == null || o.IsNavItem == isNavItem.GetValueOrDefault())
            //       orderby o.SequentialNum, o.DisplayName, o.UniqueName
            //       select new AppFunction
            //       {
            //           AppFunctionId = o.AppFunctionId,
            //           UniqueName = o.UniqueName,
            //           DisplayName = o.DisplayName,
            //           ControllerName = o.ControllerName,
            //           ActionName = o.ActionName,
            //           AppFuncLevelId = o.AppFuncLevelId,
            //           ParentId = o.ParentId,
            //           SequentialNum = o.SequentialNum,
            //           IsDisabled = o.IsDisabled,
            //           IsNavItem = o.IsNavItem,
            //           CreatedDt = o.CreatedDt,
            //           CreatedBy = o.CreatedBy,
            //           UpdatedDt = o.UpdatedDt,
            //           UpdatedBy = o.UpdatedBy,
            //           Description = o.Description,
            //           AppFuncLevelDisplayName = l.DisplayName,
            //           CreatedByDisplayName = c.DisplayName,
            //           UpdatedByDisplayName = u.DisplayName,
            //           ChildList = null // GetByCriteria(appFuncLevelId + 1, o.AppFunctionId, isNavItem)
            //       };
            var list = GetCommonQuery()?.Where(
                o => o.AppFuncLevelId == appFuncLevelId && o.ParentId == parentId
                && (isNavItem == null || o.IsNavItem == isNavItem.GetValueOrDefault()))
                .OrderBy(o => o.SequentialNum).ThenBy(o => o.DisplayName).ThenBy(o => o.UniqueName).ToList();
            if (list?.Any() ?? false)
            {
                foreach (var o in list)
                {
                    o.ChildList = GetByCriteria(appFuncLevelId + 1, o.AppFunctionId, isNavItem);
                }
            }
            return list;
        }

        //public IQueryable<AppFunction>? GetByUniqueName(string? uniqueName)
        //{
        //    return from o in db.AppFunctions
        //           join l in db.AppFuncLevels on o.AppFuncLevelId equals l.AppFuncLevelId into ls
        //           from l in ls.DefaultIfEmpty()
        //           join c in db.Users on o.CreatedBy equals c.UserId into cs
        //           from c in cs.DefaultIfEmpty()
        //           join u in db.Users on o.UpdatedBy equals u.UserId into us
        //           from u in us.DefaultIfEmpty()
        //           where string.IsNullOrEmpty(uniqueName)
        //           || (string.IsNullOrEmpty(o.UniqueName) == false && o.UniqueName!.ToLower().StartsWith(uniqueName.ToLower()))
        //           select new AppFunction
        //           {
        //               AppFunctionId = o.AppFunctionId,
        //               UniqueName = o.UniqueName,
        //               DisplayName = o.DisplayName,
        //               AppFuncLevelId = o.AppFuncLevelId,
        //               ParentId = o.ParentId,
        //               SequentialNum = o.SequentialNum,
        //               IsDisabled = o.IsDisabled,
        //               IsNavItem = o.IsNavItem,
        //               CreatedDt = o.CreatedDt,
        //               CreatedBy = o.CreatedBy,
        //               UpdatedDt = o.UpdatedDt,
        //               UpdatedBy = o.UpdatedBy,
        //               Description = o.Description,
        //               AppFuncLevelDisplayName = l.DisplayName,
        //               CreatedByDisplayName = c.DisplayName,
        //               UpdatedByDisplayName = u.DisplayName
        //           };
        //}

        public bool UniqueNameExists(string uniqueName)
        {
            if (string.IsNullOrEmpty(uniqueName)) return false;
            uniqueName = uniqueName.ToLower();
            return db.AppFunctions.Any(o => string.IsNullOrEmpty(o.UniqueName) == false && o.UniqueName.ToLower().Equals(uniqueName));
        }

        public bool UniqueNameExists(string uniqueName, int id)
        {
            if (string.IsNullOrEmpty(uniqueName)) return false;
            uniqueName = uniqueName.ToLower();
            return db.AppFunctions.Any(o => o.AppFunctionId != id && string.IsNullOrEmpty(o.UniqueName) == false && o.UniqueName.ToLower().Equals(uniqueName));
        }
    }
}
