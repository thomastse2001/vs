using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using System.Data;
using AspNetCore6WebApp.Entities;
using TT;

namespace AspNetCore6WebApp.DLL
{
    public interface IProductDao : IExcelEntityDao<Product>
    {
        int Count(string? searchText);
        IQueryable<Product>? GetByCriteria(string? name);
    }

    public class ProductDao : IProductDao
    {
        private readonly AspNetCore6WebAppDbContext db;
        private readonly IDbHelper _dbHelper;
        private readonly ILogger<ProductDao> _logger;

        public ProductDao(AspNetCore6WebAppDbContext db,
            IDbHelper dbHelper,
            ILogger<ProductDao> logger)
        {
            this.db = db;
            _dbHelper = dbHelper;
            _logger = logger;
        }

        public int Count(string? searchText)
        {
            if (searchText != null) searchText = searchText.ToLower();
            return string.IsNullOrWhiteSpace(searchText) ?
                db.Products.Count() :
                db.Products.Count(o =>
                (string.IsNullOrEmpty(o.Code) == false && o.Code.ToLower().StartsWith(searchText))
                || (string.IsNullOrEmpty(o.Name) == false && o.Name.ToLower().StartsWith(searchText))
                || (string.IsNullOrEmpty(o.Description) == false && o.Description.ToLower().StartsWith(searchText)));
        }

        public IQueryable<Product>? GetByCriteria(string? searchText)
        {
            if (searchText != null) searchText = searchText.ToLower();
            return from o in db.Products
                   join c in db.Users on o.CreatedBy equals c.UserId into cs
                   from c in cs.DefaultIfEmpty()
                   join u in db.Users on o.UpdatedBy equals u.UserId into us
                   from u in us.DefaultIfEmpty()
                   where string.IsNullOrEmpty(searchText)
                   || (string.IsNullOrEmpty(o.Code) == false && o.Code!.ToLower().StartsWith(searchText))
                   || (string.IsNullOrEmpty(o.Name) == false && o.Name!.ToLower().StartsWith(searchText))
                   || (string.IsNullOrEmpty(o.Description) == false && o.Description!.ToLower().StartsWith(searchText))
                   select new Product
                   {
                       ProductId = o.ProductId,
                       Code = o.Code,
                       Name = o.Name,
                       Description = o.Description,
                       ProductTypeId = o.ProductTypeId,
                       Price = o.Price,
                       Price2 = o.Price2,
                       DiscountRate = o.DiscountRate,
                       Discount = o.Discount,
                       IsEnabled = o.IsEnabled,
                       Version = o.Version,
                       CreatedDt = o.CreatedDt,
                       CreatedBy = o.CreatedBy,
                       UpdatedDt = o.UpdatedDt,
                       UpdatedBy = o.UpdatedBy,
                       CreatedByDisplayName = c.DisplayName,
                       UpdatedByDisplayName = u.DisplayName
                   };
        }

        public int GetMaxVersion()
        {
            return db.Products.Max(o => o.Version).GetValueOrDefault();
        }

        private Product? GetFromDataRow(int updateVersion, int createdBy, DateTime createdDt, DataRow dr, out string? errorMessage)
        {
            errorMessage = null;
            try
            {
                string? code = _dbHelper.GetStringFromDb(dr, Param.Excel.ColumnHeader.Product.Code);
                if (string.IsNullOrWhiteSpace(code))
                {
                    _logger.LogError("GetFromDataFow. Code cannot be empty.");
                    return null;
                }
                return new Product
                {
                    Code = code,
                    Name = _dbHelper.GetStringFromDb(dr, Param.Excel.ColumnHeader.Product.Name),
                    Description = _dbHelper.GetStringFromDb(dr, Param.Excel.ColumnHeader.Product.Description),
                    ProductTypeId = _dbHelper.GetIntFromDb(dr, Param.Excel.ColumnHeader.Product.ProductTypeId),
                    Price = _dbHelper.GetDecimalFromDb(dr, Param.Excel.ColumnHeader.Product.Price),
                    Price2 = _dbHelper.GetDecimalFromDb(dr, Param.Excel.ColumnHeader.Product.Price2),
                    DiscountRate = _dbHelper.GetDecimalFromDb(dr, Param.Excel.ColumnHeader.Product.DiscountRate),
                    Discount = _dbHelper.GetDecimalFromDb(dr, Param.Excel.ColumnHeader.Product.Discount),
                    IsEnabled = true,
                    Version = updateVersion,
                    CreatedDt = createdDt,
                    CreatedBy = createdBy,
                    UpdatedDt = createdDt,
                    UpdatedBy = createdBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("GetFromDataFow.");
                _logger.LogError("{ex}", ex.ToString());
                return null;
            }
        }

        public IEnumerable<Product>? GetFromDataTable(int updateVersion, int createdBy, DateTime createdDt, DataTable? dt, out string? errorList)
        {
            errorList = null;
            if (dt == null || (dt.Rows?.Count ?? 0) < 1) return null;
            //return from dr in dt!.AsEnumerable()
            //       select GetFromDataFow(updateVersion, createdBy, createdDt, dr);
            List<Product>? list = new();
            foreach (DataRow dr in dt.Rows!)
            {
                if (dr != null)
                {
                    Product? o = GetFromDataRow(updateVersion, createdBy, createdDt, dr, out string? errorMessage);
                    if (string.IsNullOrWhiteSpace(errorMessage)) errorList += errorMessage + Environment.NewLine;
                    if (o != null) list.Add(o);
                }
            }
            return list.AsEnumerable();
        }

        public int BatchInsertAndUpdate(int updateVersion, IEnumerable<Product> os)
        {
            using var trans = db.Database.BeginTransaction();
            try
            {
                /// Insert new records.
                _logger.LogWarning("Insert into database.");
                db.Products.AddRange(os);
                int i = db.SaveChanges();
                /// Delete the outdated.
                _logger.LogWarning("Delete the outdated.");
                db.Products.RemoveRange(db.Products.Where(o => o.Version < updateVersion));
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
    }
}
