using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;
using AspNetCore6WebApp.Web.Pages.Shared;
using TT;

namespace AspNetCore6WebApp.Web.Pages.Vendors
{
    public class IndexModel : PageModel
    {
        private readonly IVendorBo _vendorBo;
        private readonly IPagingHelper _pagingHelper;

        [BindProperty(SupportsGet = true)]
        public string SearchText { get; set; } = String.Empty;
        [BindProperty(SupportsGet = true)]
        public int P { get; set; } = 1;
        //public int PageCount { get; private set; } = 1;
        //public int RecordsPerPage { get; private set; } = 10;
        public IPagingHelper Pager = new PagingHelper();
        public IEnumerable<Vendor>? Vendors { get; private set; } = null;

        public IndexModel(IVendorBo vendorBo,
            IPagingHelper pagingHelper)
        {
            _vendorBo = vendorBo;
            _pagingHelper = pagingHelper;
        }

        public IActionResult OnGet()
        {
            //Vendors = _vendorBo.GetByCriteria(SearchTerm);

            //int recordCount = _vendorBo.Count(SearchTerm);
            //PageCount = (int)Math.Ceiling(1.0 * recordCount / RecordsPerPage);
            //Vendors = _vendorBo.GetByCriteria2(SearchTerm, PageNum, RecordsPerPage);
            //Vendors = _vendorBo.GetByCriteria(SearchTerm)?.OrderBy(o => o.VendorId).Skip((PageNum - 1) * RecordsPerPage).Take(RecordsPerPage);

            Pager.PageNum = this.P;
            Pager.PageCount = _pagingHelper.GetPageCount(_vendorBo.Count(SearchText), Pager.ItemsPerPage);
            Vendors = _pagingHelper.GetList(_vendorBo.GetByCriteria(SearchText)?.OrderBy(o => o.VendorId), Pager.PageNum, Pager.ItemsPerPage);

            if (Vendors == null) return NotFound();
            return Page();
        }
    }
}
