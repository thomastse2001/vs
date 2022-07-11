using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Microsoft.Extensions.Logging;
using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface IUserDao
    {
        int AddOrUpdate(User o);
        void Delete(int id);
        User? GetById(int id);
        IQueryable<User>? GetByName(string? name);
        bool LoginNameExists(string loginName);
        bool LoginNameExists(string loginName, int userId);
    }

    public class UserDao : IUserDao
    {
        private readonly AspNetCore6WebAppDbContext db;

        public UserDao(AspNetCore6WebAppDbContext db)
        {
            this.db = db;
        }

        /// Return value = id
        public int AddOrUpdate(User o)
        {
            if (o.UserId == 0)
            {
                db.Add(o);
            }
            else
            {
                var entity = db.Users.Attach(o);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            db.SaveChanges();
            return o.UserId;
        }

        public void Delete(int id)
        {
            var o = db.Users.Find(id);
            if (o != null)
            {
                db.Users.Remove(o);
                db.SaveChanges();
            }
        }

        private IQueryable<User>? GetCommonQuery()
        {
            return from o in db.Users
                   join d in db.Departments on o.DepartmentId equals d.DepartmentId into ds
                   from d in ds.DefaultIfEmpty()
                   join cat in db.Categories on o.CategoryId equals cat.CategoryId into cats
                   from cat in cats.DefaultIfEmpty()
                   join s in db.SubCategories on o.SubCategoryId equals s.SubCategoryId into ss
                   from s in ss.DefaultIfEmpty()
                   join c in db.Users on o.CreatedBy equals c.UserId into cs
                   from c in cs.DefaultIfEmpty()
                   join u in db.Users on o.UpdatedBy equals u.UserId into us
                   from u in us.DefaultIfEmpty()
                   select new User
                   {
                       UserId = o.UserId,
                       LoginName = o.LoginName,
                       DisplayName = o.DisplayName,
                       Hash = o.Hash,
                       Password = o.Password,
                       DepartmentId = o.DepartmentId,
                       CategoryId = o.CategoryId,
                       SubCategoryId = o.SubCategoryId,
                       Birthday = o.Birthday,
                       IsDisabled = o.IsDisabled,
                       CreatedDt = o.CreatedDt,
                       CreatedBy = o.CreatedBy,
                       UpdatedDt = o.UpdatedDt,
                       UpdatedBy = o.UpdatedBy,
                       Description = o.Description,
                       DepartmentDisplayName = d.DisplayName,
                       CategoryCode = cat.Code,
                       CategoryDisplayName = cat.DisplayName,
                       SubCategoryCode = s.Code,
                       SubCategoryDisplayName = s.DisplayName,
                       CreatedByDisplayName = c.DisplayName,
                       UpdatedByDisplayName = u.DisplayName
                   };
        }

        public IQueryable<User>? GetByName(string? name)
        {
            if (name != null) name = name.ToLower();
            return GetCommonQuery()?.Where(o => string.IsNullOrEmpty(name)
            || (string.IsNullOrEmpty(o.LoginName) == false && o.LoginName.ToLower().StartsWith(name))
            || (string.IsNullOrEmpty(o.DisplayName) == false && o.DisplayName.ToLower().StartsWith(name)));
        }

        public User? GetById(int id)
        {
            var o = db.Users.Find(id);
            if (o != null)
            {
                o.DepartmentDisplayName = db.Departments.Find(o.DepartmentId)?.DisplayName;
                o.CreatedByDisplayName = GetById(o.CreatedBy)?.DisplayName;
                o.UpdatedByDisplayName = GetById(o.UpdatedBy)?.DisplayName;
                Category? c = db.Categories.Find(o.CategoryId);
                o.CategoryCode = c?.Code;
                o.CategoryDisplayName = c?.DisplayName;
                SubCategory? s = db.SubCategories.Find(o.SubCategoryId);
                o.SubCategoryCode = s?.Code;
                o.SubCategoryDisplayName = s?.DisplayName;
            }
            return o;
        }

        public bool LoginNameExists(string loginName)
        {
            if (string.IsNullOrEmpty(loginName)) return false;
            //return db.Users.Any(o => string.IsNullOrEmpty(o.LoginName) == false && o.LoginName.Equals(loginName, StringComparison.OrdinalIgnoreCase));
            loginName = loginName.ToLower();
            return db.Users.Any(o => string.IsNullOrEmpty(o.LoginName) == false && o.LoginName.ToLower().Equals(loginName));
        }

        public bool LoginNameExists(string loginName, int userId)
        {
            if (string.IsNullOrEmpty(loginName)) return false;
            //return db.Users.Any(o => o.UserId != userId && string.IsNullOrEmpty(o.LoginName) == false && o.LoginName.Equals(loginName, StringComparison.OrdinalIgnoreCase));
            loginName = loginName.ToLower();
            return db.Users.Any(o => o.UserId != userId && string.IsNullOrEmpty(o.LoginName) == false && o.LoginName.ToLower().Equals(loginName));
        }
    }
}
