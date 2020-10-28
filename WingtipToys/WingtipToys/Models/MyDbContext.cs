using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

namespace WingtipToys.Models
{
    public class MyDbContext : DbContext
    {
        /// <summary>
        /// SQLite with C#.Net and Entity Framework
        /// https://www.codeproject.com/Articles/1158937/SQLite-with-Csharp-Net-and-Entity-Framework
        /// </summary>
        public MyDbContext() : 
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = @"C:\temp\WingtipToys.sqlite" }.ConnectionString
            }, true)
        { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> ShoppingCartItems { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}

//using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Web;
//using System.Data.Entity;

//namespace WingtipToys.Models
//{
//    public class ProductContext : DbContext
//    {
//        public DbSet<Category> Categories { get; set; }
//        public DbSet<Product> Products { get; set; }
//        public ProductContext() : base("WingtipToys") { }
//        //public ProductContext() : base(DLL.DbHelper.SQLite.GetConnectionString()) { }
//    }
//}