using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface ICategoryDao
    {
        Category? GetById(int id);
        IQueryable<Category>? GetAll();
        IQueryable<Category>? GetByDepartmentId(int departmentId);
    }

    public class CategoryDao : ICategoryDao
    {
        private readonly AspNetCore6WebAppDbContext db;

        public CategoryDao(AspNetCore6WebAppDbContext db)
        {
            this.db = db;
        }

        public Category? GetById(int id)
        {
            return db.Categories.Find(id);
        }

        public IQueryable<Category>? GetAll()
        {
            return db.Categories;
        }

        public IQueryable<Category>? GetByDepartmentId(int departmentId)
        {
            return db.Categories.Where(o => o.DepartmentId == departmentId);
        }
    }
}
