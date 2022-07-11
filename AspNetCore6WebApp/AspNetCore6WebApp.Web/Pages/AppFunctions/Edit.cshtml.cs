using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.AppFunctions
{
    public class EditModel : PageModel
    {
        private readonly IAppFuncLevelBo _appFuncLevelBo;
        private readonly IAppFunctionBo _appFunctionBo;

        [BindProperty]
        public AppFunction? AppFunction { get; set; } = null;
        public SelectList? AppFuncLevelSelectList { get; set; } = null;

        public EditModel(IAppFuncLevelBo appFuncLevelBo, IAppFunctionBo appFunctionBo)
        {
            this._appFuncLevelBo = appFuncLevelBo;
            this._appFunctionBo = appFunctionBo;
        }

        public IActionResult OnGet(int? id)
        {
            AppFunction = id == null ? new AppFunction() : _appFunctionBo.GetById(id.GetValueOrDefault());
            if (AppFunction == null) return NotFound();
            AppFuncLevelSelectList = new SelectList(_appFuncLevelBo.GetByUniqueName(null), nameof(AppFunction.AppFuncLevelId), nameof(AppFunction.DisplayName));
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (AppFunction == null) return NotFound();
            if (!string.IsNullOrEmpty(AppFunction.UniqueName)) AppFunction.UniqueName = AppFunction.UniqueName.Trim();
            /// Validate
            List<KeyValuePair<string, string>> errorList = _appFunctionBo.ValidateForm(AppFunction);
            if (errorList.Any())
            {
                foreach (var err in errorList)
                {
                    ModelState.AddModelError(err.Key, err.Value);
                }
                return Page();
            }
            /// Update database.
            AppFunction.UpdatedDt = DateTime.Now;
            AppFunction.UpdatedBy = 0;
            int returnId = _appFunctionBo.AddOrUpdate(AppFunction);
            if (returnId > 0)
            {
                TempData["Message"] = "Saved!";
                return RedirectToPage("./Details", new { id = returnId });
            }
            ModelState.AddModelError(string.Empty, "Fail to update database");
            return Page();
        }
    }
}
