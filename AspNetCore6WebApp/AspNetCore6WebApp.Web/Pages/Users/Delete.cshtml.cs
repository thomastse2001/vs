using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly IUserBo _userBo;

        [BindProperty]
        public new User? User { get; set; }

        public DeleteModel(IUserBo userBo)
        {
            _userBo = userBo;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();
            User = _userBo.GetById(id.GetValueOrDefault());
            if (User == null) return NotFound();
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null) return NotFound();
            //User = _userBo.GetById(id.GetValueOrDefault());
            //if (User != null)
            //{
            //    _userBo.Delete(id.GetValueOrDefault());
            //}
            _userBo.Delete(id.GetValueOrDefault());
            return RedirectToPage("./Index");
        }
    }
}
