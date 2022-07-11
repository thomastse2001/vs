using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface ICategoryBo
    {
        Category? GetById(int id);
        IQueryable<Category>? GetAll();
        IQueryable<Category>? GetByDepartmentId(int? departmentId);
    }

    public class CategoryBo : ICategoryBo
    {
        private readonly ICategoryDao _categoryDao;

        public CategoryBo(ICategoryDao categoryDao)
        {
            _categoryDao = categoryDao;
        }

        public Category? GetById(int id)
        {
            return _categoryDao.GetById(id);
        }

        public IQueryable<Category>? GetAll()
        {
            return _categoryDao.GetAll();
        }

        public IQueryable<Category>? GetByDepartmentId(int? departmentId)
        {
            return _categoryDao.GetByDepartmentId(departmentId.GetValueOrDefault());
        }
    }
}
