using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Collections;
using System.Web.ModelBinding;

namespace WingtipToys
{
    public partial class ShoppingCart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logic.ShoppingCartActions usersShoopingCart = new Logic.ShoppingCartActions())
            {
                decimal cartTotal = usersShoopingCart.GetTotal();
                if (cartTotal > 0)
                {
                    lblTotal.Text = string.Format("{0:c}", cartTotal);
                }
                else
                {
                    LabelTotalText.Text = "";
                    lblTotal.Text = "";
                    ShoppingCartTitle.InnerText = "Shopping Cart is Empty";
                    UpdateBtn.Visible = false;
                }
            }
        }

        public List<Models.CartItem> GetShoppingCartItems()
        {
            return new Logic.ShoppingCartActions().GetCartItems();
        }

        public List<Models.CartItem> UpdateCartItems()
        {
            using (Logic.ShoppingCartActions usersShoppingCart = new Logic.ShoppingCartActions())
            {
                string cartId = usersShoppingCart.GetCartId();

                Logic.ShoppingCartActions.ShoppingCartUpdates[] cartUpdates = new Logic.ShoppingCartActions.ShoppingCartUpdates[CartGrid.Rows.Count];
                for (int i = 0; i < CartGrid.Rows.Count; i++)
                {
                    //IOrderedDictionary rowValues = new OrderedDictionary();
                    IOrderedDictionary rowValues = GetValues(CartGrid.Rows[i]);
                    cartUpdates[i].ProductId = Convert.ToInt32(rowValues["ProductID"]);

                    //CheckBox cbRemove = new CheckBox();
                    CheckBox cbRemove = (CheckBox)CartGrid.Rows[i].FindControl("Remove");
                    cartUpdates[i].RemoveItem = cbRemove.Checked;

                    //TextBox quantityTextBox = new TextBox();
                    TextBox quantityTextBox = (TextBox)CartGrid.Rows[i].FindControl("PurchaseQuantity");
                    cartUpdates[i].PurchaseQuantity = Convert.ToInt16(quantityTextBox.Text.ToString());
                }
                usersShoppingCart.UpdateShoppingCartDatabase(cartId, cartUpdates);
                CartGrid.DataBind();
                lblTotal.Text = string.Format("{0:c}", usersShoppingCart.GetTotal());
                return usersShoppingCart.GetCartItems();
            }
        }

        public static IOrderedDictionary GetValues(GridViewRow row)
        {
            IOrderedDictionary values = new OrderedDictionary();
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.Visible)
                {
                    cell.ContainingField.ExtractValuesFromCell(values, cell, row.RowState, true);
                }
            }
            return values;
        }

        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            UpdateCartItems();
        }
    }
}