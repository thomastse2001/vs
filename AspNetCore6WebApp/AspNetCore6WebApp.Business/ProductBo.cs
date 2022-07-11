using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.DLL;

namespace AspNetCore6WebApp.Business
{
    public interface IProductBo
    {
        int Count(string? searchText);
        IQueryable<Product>? GetByCriteria(string? searchText);
    }

    public class ProductBo : IProductBo
    {
        private readonly IProductDao _productDao;

        public ProductBo(IProductDao productDao)
        {
            _productDao = productDao;
        }

        public int Count(string? searchText)
        {
            return _productDao.Count(searchText);
        }

        public IQueryable<Product>? GetByCriteria(string? searchText)
        {
            return _productDao.GetByCriteria(searchText);
        }
    }
}
