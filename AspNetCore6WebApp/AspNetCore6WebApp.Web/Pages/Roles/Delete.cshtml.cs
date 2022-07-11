using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Roles
{
    public class DeleteModel : PageModel
    {
        private readonly IRoleBo _roleBo;

        [BindProperty]
        public Role? Role { get; set; } = null;

        public DeleteModel(IRoleBo roleBo)
        {
            _roleBo = roleBo;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();
            Role = _roleBo.GetById(id.GetValueOrDefault());
            if (Role == null) return NotFound();
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null) return NotFound();
            _roleBo.Delete(id.GetValueOrDefault());
            return RedirectToPage("./Index");
        }
    }
}
