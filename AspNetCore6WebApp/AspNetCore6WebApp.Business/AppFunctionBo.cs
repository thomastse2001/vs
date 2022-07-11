using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IAppFunctionBo
    {
        int AddOrUpdate(AppFunction o);
        AppFunction? Delete(int id);
        AppFunction? GetById(int id);
        IEnumerable<AppFunction>? GetByCriteria(int appFuncLevelId, int parentId, bool? isNavItem);
        List<KeyValuePair<string, string>> ValidateForm(AppFunction o);
    }

    public class AppFunctionBo : IAppFunctionBo
    {
        private readonly IAppFunctionDao _appFunctionDao;

        public AppFunctionBo(IAppFunctionDao appFunctionDao)
        {
            _appFunctionDao = appFunctionDao;
        }

        public int AddOrUpdate(AppFunction o)
        {
            if (o.AppFunctionId == 0)
            {
                o.CreatedBy = o.UpdatedBy;
                o.CreatedDt = o.UpdatedDt;
            }
            return _appFunctionDao.AddOrUpdate(o);
        }

        public AppFunction? Delete(int id)
        {
            return _appFunctionDao.Delete(id);
        }

        public AppFunction? GetById(int id)
        {
            return _appFunctionDao.GetById(id);
        }

        public IEnumerable<AppFunction>? GetByCriteria(int appFuncLevelId, int parentId, bool? isNavItem)
        {
            return _appFunctionDao.GetByCriteria(appFuncLevelId, parentId, isNavItem);
        }

        public List<KeyValuePair<string, string>> ValidateForm(AppFunction o)
        {
            List<KeyValuePair<string, string>> errorList = new();
            if (string.IsNullOrEmpty(o.UniqueName)) errorList.Add(new KeyValuePair<string, string>("AppFunction.UniqueName", "Unique Name cannot be empty."));
            else
            {
                if (o.UniqueName.Length > Param.MaxLength.AppFunction.UniqueName) errorList.Add(new KeyValuePair<string, string>("AppFunction.UniqueName", string.Format("Unique Name cannot be more than {0} characters.", Param.MaxLength.AppFunction.UniqueName)));
                if ((o.AppFunctionId > 0 && _appFunctionDao.UniqueNameExists(o.UniqueName, o.AppFunctionId))
                    || (o.AppFunctionId == 0 && _appFunctionDao.UniqueNameExists(o.UniqueName)))
                    errorList.Add(new KeyValuePair<string, string>(string.Empty, "Unique Name already exists."));
            }
            if (string.IsNullOrEmpty(o.DisplayName) == false && o.DisplayName.Length > Param.MaxLength.AppFunction.DisplayName) errorList.Add(new KeyValuePair<string, string>("AppFunction.DisplayName", string.Format("Display Name cannot be more than {0} characters.", Param.MaxLength.AppFunction.DisplayName)));
            if (string.IsNullOrEmpty(o.ControllerName) == false && o.ControllerName.Length > Param.MaxLength.AppFunction.ControllerName) errorList.Add(new KeyValuePair<string, string>("AppFunction.ControllerName", string.Format("Controller Name cannot be more than {0} characters.", Param.MaxLength.AppFunction.ControllerName)));
            if (string.IsNullOrEmpty(o.ActionName) == false && o.ActionName.Length > Param.MaxLength.AppFunction.ActionName) errorList.Add(new KeyValuePair<string, string>("AppFunction.ActionName", string.Format("Action Name cannot be more than {0} characters.", Param.MaxLength.AppFunction.ActionName)));
            if (string.IsNullOrEmpty(o.Description) == false && o.Description.Length > Param.MaxLength.AppFunction.Description) errorList.Add(new KeyValuePair<string, string>("AppFunction.Description", string.Format("Description cannot be more than {0} characters.", Param.MaxLength.AppFunction.Description)));
            return errorList;
        }
    }
}
