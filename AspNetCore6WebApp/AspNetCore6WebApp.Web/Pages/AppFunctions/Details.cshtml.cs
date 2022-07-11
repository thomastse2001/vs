using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.AppFunctions
{
    public class DetailsModel : PageModel
    {
        private readonly IAppFunctionBo _appFunctionBo;

        public AppFunction? AppFunction { get; set; } = null;
        [TempData]
        public string? Message { get; set; } = null;

        public DetailsModel(IAppFunctionBo appFunctionBo)
        {
            this._appFunctionBo = appFunctionBo;
        }

        public IActionResult OnGet(int id)
        {
            AppFunction = _appFunctionBo.GetById(id);
            if (AppFunction == null) return NotFound();
            return Page();
        }
    }
}
