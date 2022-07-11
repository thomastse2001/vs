using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IVendorBo
    {
        int Count(string? searchText);
        IQueryable<Vendor>? GetByCriteria(string? searchText);
        //IQueryable<Vendor>? GetByCriteria2(string? name, int pageNum, int recordsPerPage);
    }

    public class VendorBo : IVendorBo
    {
        private readonly IVendorDao _vendorDao;

        public VendorBo(IVendorDao vendorDao)
        {
            _vendorDao = vendorDao;
        }

        public int Count(string? searchText)
        {
            return _vendorDao.Count(searchText);
        }

        public IQueryable<Vendor>? GetByCriteria(string? searchText)
        {
            return _vendorDao.GetByCriteria(searchText);
        }

        //public IQueryable<Vendor>? GetByCriteria2(string? searchText, int pageNum, int recordsPerPage)
        //{
        //    if (pageNum < 1 || recordsPerPage < 1) return null;
        //    return _vendorDao.GetByCriteria(searchText)?.OrderBy(o => o.VendorId).Skip((pageNum - 1) * recordsPerPage).Take(recordsPerPage);
        //}
    }
}
