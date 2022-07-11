using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface ISubCategoryDao
    {
        SubCategory? GetById(int id);
        IQueryable<SubCategory>? GetAll();
        IQueryable<SubCategory>? GetByCategoryId(int categoryId);
    }

    public class SubCategoryDao : ISubCategoryDao
    {
        private readonly AspNetCore6WebAppDbContext db;

        public SubCategoryDao(AspNetCore6WebAppDbContext db)
        {
            this.db = db;
        }

        public SubCategory? GetById(int id)
        {
            return db.SubCategories.Find(id);
        }

        public IQueryable<SubCategory>? GetAll()
        {
            return db.SubCategories;
        }

        public IQueryable<SubCategory>? GetByCategoryId(int categoryId)
        {
            return db.SubCategories.Where(o => o.CategoryId == categoryId);
        }
    }
}
