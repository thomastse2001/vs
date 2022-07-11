using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.AppFunctions
{
    public class IndexModel : PageModel
    {
        private readonly IAppFunctionBo _appFunctionBo;
        public IEnumerable<AppFunction>? AppFunctions { get; private set; } = null;

        public IndexModel(IAppFunctionBo appFunctionBo)
        {
            this._appFunctionBo = appFunctionBo;
        }

        public IActionResult OnGet()
        {
            AppFunctions = _appFunctionBo.GetByCriteria(1, 0, null);
            if (AppFunctions == null) return NotFound();
            return Page();
        }
    }
}
