using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserBo _userBo;

        [BindProperty(SupportsGet = true)]
        public string SearchText { get; set; } = String.Empty;
        public IEnumerable<User>? Users { get; private set; } = Enumerable.Empty<User>();

        public IndexModel(IUserBo userBo)
        {
            _userBo = userBo;
        }

        public IActionResult OnGet()
        {
            Users = _userBo.GetByName(SearchText);
            if (Users == null) return NotFound();
            return Page();
        }
    }
}
