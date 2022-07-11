using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCore6WebApp.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //DateTime dt = DateTime.Now;
            //_logger.LogWarning("{dt:yyyy-MM-dd HH:mm:ss}", dt);
            //var ex = new Exception("It is new exception.");
            //_logger.LogError("{ex}", ex.ToString());
            //_logger.LogError("--{ex.ToString()}", ex);
        }
    }
}