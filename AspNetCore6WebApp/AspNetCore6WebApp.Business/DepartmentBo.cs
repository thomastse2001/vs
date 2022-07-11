using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IDepartmentBo
    {
        Department? GetById(int id);
        IQueryable<Department>? GetAll();
    }

    public class DepartmentBo : IDepartmentBo
    {
        private readonly IDepartmentDao _departmentDao;

        public DepartmentBo(IDepartmentDao departmentDao)
        {
            _departmentDao = departmentDao;
        }

        public Department? GetById(int id)
        {
            return _departmentDao.GetById(id);
        }

        public IQueryable<Department>? GetAll()
        {
            return _departmentDao.GetAll();
        }
    }
}
