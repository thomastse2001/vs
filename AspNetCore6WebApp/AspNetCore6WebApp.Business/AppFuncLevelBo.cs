using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IAppFuncLevelBo
    {
        string? GetDisplayNameById(int id);
        IQueryable<AppFuncLevel>? GetByUniqueName(string? name);
    }

    public class AppFuncLevelBo : IAppFuncLevelBo
    {
        private readonly IAppFuncLevelDao _appFuncLevelDao;

        public AppFuncLevelBo(IAppFuncLevelDao appFuncLevelDao)
        {
            _appFuncLevelDao = appFuncLevelDao;
        }

        public string? GetDisplayNameById(int id)
        {
            return _appFuncLevelDao.GetById(id)?.DisplayName;
        }

        public IQueryable<AppFuncLevel>? GetByUniqueName(string? name)
        {
            return _appFuncLevelDao.GetByUniqueName(name);
        }
    }
}
