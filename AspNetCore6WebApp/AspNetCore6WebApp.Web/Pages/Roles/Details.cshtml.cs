using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Roles
{
    public class DetailsModel : PageModel
    {
        private readonly IRoleBo _roleBo;

        public Role? Role { get; set; } = null;
        [TempData]
        public string? Message { get; set; } = null;

        public DetailsModel(IRoleBo roleBo)
        {
            _roleBo = roleBo;
        }

        public IActionResult OnGet(int id)
        {
            Role = _roleBo.GetById(id);
            if (Role == null) return NotFound();
            return Page();
        }
    }
}
