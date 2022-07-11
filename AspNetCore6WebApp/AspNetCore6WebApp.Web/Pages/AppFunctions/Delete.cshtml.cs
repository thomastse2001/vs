using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.AppFunctions
{
    public class DeleteModel : PageModel
    {
        private readonly IAppFunctionBo _appFunctionBo;

        [BindProperty]
        public AppFunction? AppFunction { get; set; } = null;

        public DeleteModel(IAppFunctionBo appFunctionBo)
        {
            this._appFunctionBo = appFunctionBo;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();
            AppFunction = _appFunctionBo.GetById(id.GetValueOrDefault());
            if (AppFunction == null) return NotFound();
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null) return NotFound();
            AppFunction = _appFunctionBo.GetById(id.GetValueOrDefault());
            if (AppFunction != null)
            {
                _appFunctionBo.Delete(id.GetValueOrDefault());
            }
            return RedirectToPage("./Index");
        }
    }
}
