using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface IDepartmentDao
    {
        Department? GetById(int id);
        IQueryable<Department>? GetAll();
    }

    public class DepartmentDao : IDepartmentDao
    {
        private readonly AspNetCore6WebAppDbContext db;

        public DepartmentDao(AspNetCore6WebAppDbContext db)
        {
            this.db = db;
        }

        public Department? GetById(int id)
        {
            return db.Departments.Find(id);
        }

        public IQueryable<Department>? GetAll()
        {
            return db.Departments;
        }
    }
}
