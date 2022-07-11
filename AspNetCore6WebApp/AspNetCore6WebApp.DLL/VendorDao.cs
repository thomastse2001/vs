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
    public interface IVendorDao : IExcelEntityDao<Vendor>
    {
        int Count(string? searchText);
        IQueryable<Vendor>? GetByCriteria(string? searchText);
    }

    public class VendorDao : IVendorDao
    {
        private readonly AspNetCore6WebAppDbContext db;
        private readonly IDbHelper _dbHelper;
        private readonly ILogger<VendorDao> _logger;

        public VendorDao(AspNetCore6WebAppDbContext db,
            IDbHelper dbHelper,
            ILogger<VendorDao> logger)
        {
            this.db = db;
            _dbHelper = dbHelper;
            _logger = logger;
        }

        public int Count(string? searchText)
        {
            if (searchText != null) searchText = searchText.ToLower();
            return string.IsNullOrWhiteSpace(searchText) ?
                db.Vendors.Count() :
                db.Vendors.Count(o =>
                (string.IsNullOrEmpty(o.Name) == false && o.Name.ToLower().StartsWith(searchText))
                || (string.IsNullOrEmpty(o.Description) == false && o.Description.ToLower().StartsWith(searchText)));
        }

        public IQueryable<Vendor>? GetByCriteria(string? searchText)
        {
            if (searchText != null) searchText = searchText.ToLower();
            return from o in db.Vendors
                   join c in db.Users on o.CreatedBy equals c.UserId into cs
                   from c in cs.DefaultIfEmpty()
                   join u in db.Users on o.UpdatedBy equals u.UserId into us
                   from u in us.DefaultIfEmpty()
                   where string.IsNullOrEmpty(searchText)
                   || (string.IsNullOrEmpty(o.Name) == false && o.Name!.ToLower().StartsWith(searchText))
                   || (string.IsNullOrEmpty(o.Description) == false && o.Description!.ToLower().StartsWith(searchText))
                   select new Vendor
                   {
                       VendorId = o.VendorId,
                       Name = o.Name,
                       Description = o.Description,
                       IsDisabled = o.IsDisabled,
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
            return db.Vendors.Max(o => o.Version).GetValueOrDefault();
        }

        private Vendor? GetFromDataRow(int updateVersion, int createdBy, DateTime createdDt, DataRow dr, out string? errorMessage)
        {
            errorMessage = null;
            try
            {
                string? name = _dbHelper.GetStringFromDb(dr, Param.Excel.ColumnHeader.Vendor.Name);
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogError("GetFromDataFow. Name cannot be empty.");
                    return null;
                }
                return new Vendor
                {
                    Name = name,
                    Description = _dbHelper.GetStringFromDb(dr, Param.Excel.ColumnHeader.Vendor.Description),
                    IsDisabled = false,
                    Version = updateVersion,
                    CreatedDt = createdDt,
                    CreatedBy = createdBy,
                    UpdatedDt = createdDt,
                    UpdatedBy = createdBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("GetFromDataFow. ");
                _logger.LogError("{ex}", ex.ToString());
                return null;
            }
        }

        public IEnumerable<Vendor>? GetFromDataTable(int updateVersion, int createdBy, DateTime createdDt, DataTable? dt, out string? errorList)
        {
            errorList = null;
            if (dt == null || (dt.Rows?.Count ?? 0) < 1) return null;

            List<Vendor>? list = new();
            foreach (DataRow dr in dt.Rows!)
            {
                if (dr != null)
                {
                    Vendor? o = GetFromDataRow(updateVersion, createdBy, createdDt, dr, out string? errorMessage);
                    if (string.IsNullOrWhiteSpace(errorMessage)) errorList += errorMessage + Environment.NewLine;
                    if (o != null) list.Add(o);
                }
            }
            return list.AsEnumerable();
        }

        public int BatchInsertAndUpdate(int updateVersion, IEnumerable<Vendor> os)
        {
            using var trans = db.Database.BeginTransaction();
            try
            {
                /// Insert new records.
                _logger.LogWarning("Insert into database.");
                db.Vendors.AddRange(os);
                int i = db.SaveChanges();
                /// Delete the outdated.
                _logger.LogWarning("Delete the outdated.");
                db.Vendors.RemoveRange(db.Vendors.Where(o => o.Version < updateVersion));
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
