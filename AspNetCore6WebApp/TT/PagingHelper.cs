using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT
{
    public interface IPagingHelper
    {
        int ItemsPerPage { get; set; }
        int PageCount { get; set; }
        int PageNum { get; set; }
        int GetPageCount(int recordCount, int itemsPerPage);
        IQueryable<T>? GetList<T>(IQueryable<T>? input, int pageNum, int itemsPerPage);
    }

    public class PagingHelper : IPagingHelper
    {
        /// https://getbootstrap.com/docs/5.0/components/pagination/
        /// https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/sort-filter-page?view=aspnetcore-6.0
        /// https://learningprogramming.net/net/asp-net-mvc/pagination-with-entity-framework-in-asp-net-mvc/

        public int ItemsPerPage { get; set; } = 10;
        public int PageCount { get; set; } = 1;
        public int PageNum { get; set; } = 1;

        public int GetPageCount(int recordCount, int itemsPerPage)
        {
            if (itemsPerPage < 1) return 0;
            return (int)Math.Ceiling(1.0 * recordCount / itemsPerPage);
        }

        public IQueryable<T>? GetList<T>(IQueryable<T>? input, int pageNum, int itemsPerPage)
        {
            if (pageNum < 1) pageNum = 1;
            if (itemsPerPage < 1) itemsPerPage = 1;
            return input?.Skip((pageNum - 1) * itemsPerPage).Take(itemsPerPage);
        }
    }
}
