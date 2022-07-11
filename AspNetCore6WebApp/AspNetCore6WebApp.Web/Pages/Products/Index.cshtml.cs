using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;
using AspNetCore6WebApp.Web.Pages.Shared;
using TT;

namespace AspNetCore6WebApp.Web.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductBo _productBo;
        private readonly IPagingHelper _pagingHelper;

        [BindProperty(SupportsGet = true)]
        public string SearchText { get; set; } = String.Empty;
        [BindProperty(SupportsGet = true)]
        public int P { get; set; } = 1;
        public IPagingHelper Pager = new PagingHelper();
        public IEnumerable<Product>? Products { get; private set; } = null;

        public IndexModel(IProductBo productBo,
            IPagingHelper pagingHelper)
        {
            _productBo = productBo;
            _pagingHelper = pagingHelper;
        }

        public IActionResult OnGet()
        {
            Pager.PageNum = this.P;
            Pager.PageCount = _pagingHelper.GetPageCount(_productBo.Count(SearchText), Pager.ItemsPerPage);
            Products = _pagingHelper.GetList(_productBo.GetByCriteria(SearchText)?.OrderBy(o => o.ProductId), Pager.PageNum, Pager.ItemsPerPage);

            if (Products == null) return NotFound();
            return Page();
        }
    }
}
