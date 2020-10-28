using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using System.Web.ModelBinding;

namespace WingtipToys
{
    public partial class ProductList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IQueryable<Models.Product> GetProducts([QueryString("id")] int? categoryId)
        {
            var db = new Models.MyDbContext();
            IQueryable<Models.Product> query = db.Products;
            return (categoryId ?? 0) > 0 ? query.Where(p => p.CategoryID == categoryId) : query;
        }
    }
}