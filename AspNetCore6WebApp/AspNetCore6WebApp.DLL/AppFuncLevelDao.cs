using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface IAppFuncLevelDao
    {
        AppFuncLevel? GetById(int id);
        IQueryable<AppFuncLevel>? GetByUniqueName(string? name);
    }

    public class AppFuncLevelDao : IAppFuncLevelDao
    {
        private readonly AspNetCore6WebAppDbContext db;

        public AppFuncLevelDao(AspNetCore6WebAppDbContext db)
        {
            this.db = db;
        }

        public AppFuncLevel? GetById(int id)
        {
            return db.AppFuncLevels.Find(id);
        }

        public IQueryable<AppFuncLevel>? GetByUniqueName(string? name)
        {
            return from o in db.AppFuncLevels
                   where string.IsNullOrEmpty(name) 
                   || (string.IsNullOrEmpty(o.DisplayName) == false && o.DisplayName.ToLower().StartsWith(name.ToLower()))
                   select o;
        }
    }
}
