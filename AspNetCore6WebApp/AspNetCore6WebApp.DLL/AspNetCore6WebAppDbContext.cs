using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public class AspNetCore6WebAppDbContext : DbContext
    {
        public AspNetCore6WebAppDbContext(DbContextOptions<AspNetCore6WebAppDbContext> options) : base(options) { }

        /// Working with Nullable Reference Types
        /// https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types
        /// Nullable Reference types in C# – Best practices
        /// Set the variable to null! means that it is non-nullable.
        /// https://www.dotnetcurry.com/csharp/nullable-reference-types-csharp
        public DbSet<AppFuncLevel> AppFuncLevels { get; set; } = null!;
        public DbSet<AppFunction> AppFunctions { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<MapAppFunctionRole> MapAppFunctionsRoles { get; set; } = null!;
        public DbSet<MapRoleUser> MapRolesUsers { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<SubCategory> SubCategories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Vendor> Vendors { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MapAppFunctionRole>().HasKey(o => new { o.AppFunctionId, o.RoleId });
            modelBuilder.Entity<MapRoleUser>().HasKey(o => new { o.RoleId, o.UserId });
        }
    }
}
