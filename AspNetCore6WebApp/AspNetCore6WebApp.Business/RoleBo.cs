using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IRoleBo
    {
        int AddOrUpdate(Role o);
        void Delete(int id);
        Role? GetById(int id);
        IQueryable<Role>? GetByName(string? name);
        List<KeyValuePair<string, string>> ValidateForm(Role o);
    }

    public class RoleBo : IRoleBo
    {
        private readonly IRoleDao _roleDao;

        public RoleBo(IRoleDao roleDao)
        {
            _roleDao = roleDao;
        }

        /// Return value = id
        public int AddOrUpdate(Role o)
        {
            if (o.RoleId == 0)
            {
                o.CreatedBy = o.UpdatedBy;
                o.CreatedDt = o.UpdatedDt;
            }
            return _roleDao.AddOrUpdate(o);
        }

        public void Delete(int id)
        {
            _roleDao.Delete(id);
        }

        public Role? GetById(int id)
        {
            return _roleDao.GetById(id);
        }

        public IQueryable<Role>? GetByName(string? name)
        {
            return _roleDao.GetByName(name);
        }

        public List<KeyValuePair<string, string>> ValidateForm(Role o)
        {
            List<KeyValuePair<string, string>> errorList = new();
            if (string.IsNullOrEmpty(o.UniqueName)) errorList.Add(new KeyValuePair<string, string>("Role.UniqueName", "Unique Name cannot be empty."));
            else
            {
                if (o.UniqueName.Length > Param.MaxLength.Role.UniqueName) errorList.Add(new KeyValuePair<string, string>("Role.UniqueName", string.Format("Unique Name cannot be more than {0} characters.", Param.MaxLength.Role.UniqueName)));
                if ((o.RoleId > 0 && _roleDao.UniqueNameExists(o.UniqueName, o.RoleId))
                    || (o.RoleId == 0 && _roleDao.UniqueNameExists(o.UniqueName)))
                    errorList.Add(new KeyValuePair<string, string>(string.Empty, "Unique Name already exists."));
            }
            if (string.IsNullOrEmpty(o.DisplayName) == false &&  o.DisplayName.Length > Param.MaxLength.Role.DisplayName) errorList.Add(new KeyValuePair<string, string>("Role.DisplayName", string.Format("Display Name cannot be more than {0} characters.", Param.MaxLength.Role.DisplayName)));
            if (string.IsNullOrEmpty(o.Description) == false && o.Description.Length > Param.MaxLength.Role.Description) errorList.Add(new KeyValuePair<string, string>("Role.Description", string.Format("Description cannot be more than {0} characters.", Param.MaxLength.Role.Description)));
            return errorList;
        }
    }
}
