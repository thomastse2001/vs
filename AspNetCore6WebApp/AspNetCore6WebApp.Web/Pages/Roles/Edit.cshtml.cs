using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Roles
{
    public class EditModel : PageModel
    {
        private readonly IRoleBo _roleBo;

        [BindProperty]
        public Role? Role { get; set; } = null;

        public EditModel(IRoleBo roleBo)
        {
            this._roleBo = roleBo;
        }

        public IActionResult OnGet(int? id)
        {
            Role = id == null ? new Role() : _roleBo.GetById(id.GetValueOrDefault());
            if (Role == null) return NotFound();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (Role == null) return NotFound();
            if (!string.IsNullOrEmpty(Role.UniqueName)) Role.UniqueName = Role.UniqueName.Trim();
            /// Validate.
            List<KeyValuePair<string, string>> errorList = _roleBo.ValidateForm(Role);
            if (errorList.Any())
            {
                foreach (var err in errorList)
                {
                    ModelState.AddModelError(err.Key, err.Value);
                }
                return Page();
            }
            /// Update database.
            Role.UpdatedDt = DateTime.Now;
            Role.UpdatedBy = 0;
            int returnId = _roleBo.AddOrUpdate(Role);
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
