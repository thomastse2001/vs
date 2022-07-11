using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Roles
{
    public class IndexModel : PageModel
    {
        private readonly IRoleBo _roleBo;
        [BindProperty(SupportsGet = true)]
        public string? SearchText { get; set; } = null;
        public IEnumerable<Role>? Roles { get; private set; } = Enumerable.Empty<Role>();

        public IndexModel(IRoleBo roleBo)
        {
            _roleBo = roleBo;
        }

        public IActionResult OnGet()
        {
            Roles = _roleBo.GetByName(SearchText);
            if (Roles == null) return NotFound();
            return Page();
        }
    }
}
