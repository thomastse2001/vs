using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using AspNetCore6WebApp.Entities;

namespace AspNetCore6WebApp.DLL
{
    public interface IExcelEntityDao<T> where T : IExcelEntity
    {
        int GetMaxVersion();
        IEnumerable<T>? GetFromDataTable(int updateVersion, int createdBy, DateTime createdDt, DataTable? dt, out string? errorList);
        int BatchInsertAndUpdate(int updateVersion, IEnumerable<T> os);
    }

    public class ExcelEntityDao
    {
        private readonly AspNetCore6WebAppDbContext db;
        private readonly ILogger<ProductDao> _logger;

        public ExcelEntityDao(AspNetCore6WebAppDbContext db,
            ILogger<ProductDao> logger)
        {
            this.db = db;
            _logger = logger;
        }

        public int BatchInsertAndUpdateWithDbSet<T>(int updateVersion, IEnumerable<T> os, DbSet<T> dbSet) where T : class, IExcelEntity
        {
            if (os == null) return 0;
            if (dbSet == null) return -1;
            using var trans = db.Database.BeginTransaction();
            try
            {
                /// Insert new records.
                _logger.LogWarning("Insert into database.");
                dbSet.AddRange(os);
                int i = db.SaveChanges();
                /// Delete the outdated.
                _logger.LogWarning("Delete the outdated.");
                dbSet.RemoveRange(dbSet.Where(o => o.Version < updateVersion));
                db.SaveChanges();
                /// Commit.
                _logger.LogWarning("Commit.");
                trans.Commit();
                return i;
            }
            catch (Exception ex)
            {
                _logger.LogError("BatchInsertAndUpdate");
                _logger.LogError("{ex}", ex.ToString());
                trans.Rollback();
                return -1;
            }
        }

        //public int BatchInsertAndUpdate<T>(int updateVersion, IEnumerable<T> os) where T : IExcelEntity
        //{
        //    Type t = typeof(T);
        //    if (t == typeof(Product))
        //        return BatchInsertAndUpdateWithDbSet<Product>(updateVersion, os, db.Products);
        //    else
        //        return BatchInsertAndUpdateWithDbSet<Vendor>(updateVersion, os, db.Vendors);
        //}
    }
}
