using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IUserBo _userBo;

        public new User? User { get; set; } = null;
        [TempData]
        public string? Message { get; set; } = null;

        public DetailsModel(IUserBo userBo)
        {
            _userBo = userBo;
        }

        public IActionResult OnGet(int id)
        {
            User = _userBo.GetById(id);
            if (User == null) return NotFound();
            return Page();
        }
    }
}
