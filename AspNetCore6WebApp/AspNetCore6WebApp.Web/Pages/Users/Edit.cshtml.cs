using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCore6WebApp.Entities;
using AspNetCore6WebApp.Business;

namespace AspNetCore6WebApp.Web.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly ICategoryBo _categoryBo;
        private readonly IDepartmentBo _departmentBo;
        private readonly ISubCategoryBo _subCategoryBo;
        private readonly IUserBo _userBo;

        [BindProperty]
        public new User? User { get; set; } = null;// if no new keyword, it generates Compiler Warning (level 2) CS0108.
        [BindProperty(SupportsGet = true)]
        public int? DepartmentId { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; } = 0;
        public SelectList? DepartmentSelectList { get; set; } = null;
        public SelectList? CategorySelectList { get; set; } = null;
        public SelectList? CategoryAllSelectList { get; private set; } = null;
        public SelectList? SubCategorySelectList { get; set; } = null;
        public SelectList? SubCategoryAllSelectList { get; private set; } = null;

        public EditModel(ICategoryBo categoryBo,
            IDepartmentBo departmentBo,
            ISubCategoryBo subCategoryBo,
            IUserBo userBo)
        {
            _categoryBo = categoryBo;
            _departmentBo = departmentBo;
            _subCategoryBo = subCategoryBo;
            _userBo = userBo;
        }

        private static Category GetDefaultCategory()
        {
            return new Category { CategoryId = 0, DisplayName = "-- Select --" };
        }

        private static SubCategory GetDefaultSubCategory()
        {
            return new SubCategory { SubCategoryId = 0, DisplayName = "-- Select --" };
        }

        private List<Category> GetCategroies(int? departmentId)
        {
            var list = _categoryBo.GetByDepartmentId(departmentId)?.ToList();
            if (list?.Any() ?? false) list.Insert(0, GetDefaultCategory());
            else list = new List<Category>() { GetDefaultCategory() };
            return list;
        }

        private List<SubCategory> GetSubCategories(int? categoryId)
        {
            var list = _subCategoryBo.GetByCategoryId(categoryId)?.ToList();
            if (list?.Any() ?? false) list.Insert(0, GetDefaultSubCategory());
            else list = new List<SubCategory> { GetDefaultSubCategory() };
            return list;
        }

        public IActionResult OnGet(int? id)
        {
            User = id == null ? new User() : _userBo.GetById(id.GetValueOrDefault());
            if (User == null) return NotFound();
            DepartmentSelectList = new SelectList(_departmentBo.GetAll()?.ToList() ?? new List<Department>(), nameof(Department.DepartmentId), nameof(Department.DisplayName));
            CategorySelectList = new SelectList(GetCategroies(User.DepartmentId), nameof(Category.CategoryId), nameof(Category.DisplayName));
            CategoryAllSelectList = new SelectList(_categoryBo.GetAll()?.ToList() ?? new List<Category>(), nameof(Category.CategoryId), nameof(Category.DisplayName));
            SubCategorySelectList = new SelectList(GetSubCategories(User.CategoryId), nameof(SubCategory.SubCategoryId), nameof(SubCategory.DisplayName));
            SubCategoryAllSelectList = new SelectList(_subCategoryBo.GetAll()?.ToList() ?? new List<SubCategory>(), nameof(SubCategory.SubCategoryId), nameof(SubCategory.DisplayName));
            return Page();
        }

        public JsonResult OnGetCategories()
        {
            return new JsonResult(GetCategroies(DepartmentId));
        }

        public JsonResult OnGetSubCategories()
        {
            return new JsonResult(GetSubCategories(CategoryId));
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (User == null) return NotFound();
            if (!string.IsNullOrEmpty(User.LoginName)) User.LoginName = User.LoginName.Trim();
            /// Validate.
            List<KeyValuePair<string, string>> errorList = _userBo.ValidateForm(User);
            if (errorList.Any())
            {
                foreach (var err in errorList)
                {
                    ModelState.AddModelError(err.Key, err.Value);
                }
                return Page();
            }
            /// Update database.
            User.UpdatedDt = DateTime.Now;
            User.UpdatedBy = 0;
            int returnId = _userBo.AddOrUpdate(User);
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
