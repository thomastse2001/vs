using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.ModelBinding;

namespace WingtipToys
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IQueryable<Models.Product> GetProduct([QueryString("productID")] int? productId)
        {
            if ((productId ?? 0) < 1) return null;
            var db = new Models.MyDbContext();
            IQueryable<Models.Product> query = db.Products;
            return query.Where(p => p.ProductID == productId);
        }
    }
}