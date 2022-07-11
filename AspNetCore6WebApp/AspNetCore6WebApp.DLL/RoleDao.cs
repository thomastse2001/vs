using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface IRoleDao
    {
        int AddOrUpdate(Role o);
        void Delete(int id);
        Role? GetById(int id);
        IQueryable<Role>? GetByName(string? name);
        bool UniqueNameExists(string uniqueName);
        bool UniqueNameExists(string uniqueName, int id);
    }

    public class RoleDao : IRoleDao
    {
        private readonly AspNetCore6WebAppDbContext db;
        private readonly IUserDao _userDao;

        public RoleDao(AspNetCore6WebAppDbContext db, IUserDao userDao)
        {
            this.db = db;
            this._userDao = userDao;
        }

        /// Return value = id
        public int AddOrUpdate(Role o)
        {
            if (o.RoleId == 0)
            {
                db.Add(o);
            }
            else
            {
                var entity = db.Roles.Attach(o);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            db.SaveChanges();
            return o.RoleId;
        }

        public void Delete(int id)
        {
            var o = db.Roles.Find(id);
            if (o != null)
            {
                db.Roles.Remove(o);
                db.SaveChanges();
            }
        }

        public Role? GetById(int id)
        {
            var o = db.Roles.Find(id);
            if (o != null)
            {
                o.CreatedByDisplayName = _userDao.GetById(o.CreatedBy.GetValueOrDefault())?.DisplayName;
                o.UpdatedByDisplayName = _userDao.GetById(o.UpdatedBy.GetValueOrDefault())?.DisplayName;
            }
            return o;
        }

        private IQueryable<Role>? GetCommonQuery()
        {
            return from o in db.Roles
                   join c in db.Users on o.CreatedBy equals c.UserId into cs
                   from c in cs.DefaultIfEmpty()
                   join u in db.Users on o.UpdatedBy equals u.UserId into us
                   from u in us.DefaultIfEmpty()
                   select new Role
                   {
                       RoleId = o.RoleId,
                       UniqueName = o.UniqueName,
                       DisplayName = o.DisplayName,
                       IsDisabled = o.IsDisabled,
                       CreatedDt = o.CreatedDt,
                       CreatedBy = o.CreatedBy,
                       UpdatedDt = o.UpdatedDt,
                       UpdatedBy = o.UpdatedBy,
                       Description = o.Description,
                       CreatedByDisplayName = c.DisplayName,
                       UpdatedByDisplayName = u.DisplayName
                   };
        }

        public IQueryable<Role>? GetByName(string? name)
        {
            if (name != null) name = name.ToLower();
            return GetCommonQuery()?.Where(o => string.IsNullOrEmpty(name)
            || (string.IsNullOrEmpty(o.UniqueName) == false && o.UniqueName.ToLower().StartsWith(name))
            || (string.IsNullOrEmpty(o.DisplayName) == false && o.DisplayName.ToLower().StartsWith(name)));
        }

        public bool UniqueNameExists(string uniqueName)
        {
            if (string.IsNullOrEmpty(uniqueName)) return false;
            uniqueName = uniqueName.ToLower();
            return db.Roles.Any(o => string.IsNullOrEmpty(o.UniqueName) == false && o.UniqueName.ToLower().Equals(uniqueName));
        }

        public bool UniqueNameExists(string uniqueName, int id)
        {
            if (string.IsNullOrEmpty(uniqueName)) return false;
            uniqueName = uniqueName.ToLower();
            return db.Roles.Any(o => o.RoleId != id && string.IsNullOrEmpty(o.UniqueName) == false && o.UniqueName.ToLower().Equals(uniqueName));
        }
    }
}
