using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface ISubCategoryBo
    {
        SubCategory? GetById(int id);
        IQueryable<SubCategory>? GetAll();
        IQueryable<SubCategory>? GetByCategoryId(int? categoryId);
    }

    public class SubCategoryBo : ISubCategoryBo
    {
        private readonly ISubCategoryDao _subCategoryDao;

        public SubCategoryBo(ISubCategoryDao subCategoryDao)
        {
            _subCategoryDao = subCategoryDao;
        }

        public SubCategory? GetById(int id)
        {
            return _subCategoryDao.GetById(id);
        }

        public IQueryable<SubCategory>? GetAll()
        {
            return _subCategoryDao.GetAll();
        }

        public IQueryable<SubCategory>? GetByCategoryId(int? categoryId)
        {
            return _subCategoryDao.GetByCategoryId(categoryId.GetValueOrDefault());
        }
    }
}
