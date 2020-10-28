using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private Models.MyDbContext _db = new Models.MyDbContext();

        public const string CartSessionKey = "CartId";

        public void AddToCart(int id)
        {
            // Retrieve the product from the database.
            ShoppingCartId = GetCartId();

            var cartItem = _db.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId && c.ProductId == id);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.
                cartItem = new Models.CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartId,
                    Product = _db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity.
                cartItem.Quantity++;
            }
            _db.SaveChanges();
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public string GetCartId()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.
                    Guid tempCardId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCardId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<Models.CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();
            return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        }

        public decimal GetTotal()
        {
            ShoppingCartId = GetCartId();
            // Multiply product price by quantity of that product to get the current price for each of those products in the cart.
            // Sum all product price totals to get the cart total.
            return (decimal?)(from i in _db.ShoppingCartItems
                              where i.CartId == ShoppingCartId
                              select (int?)i.Quantity * i.Product.UnitPrice).Sum() ?? decimal.Zero;
        }

        public ShoppingCartActions GetCart(HttpContext context)
        {
            using (var cart = new ShoppingCartActions())
            {
                cart.ShoppingCartId = cart.GetCartId();
                return cart;
            }
        }

        public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] CartItemUpdates)
        {
            using (var db = new Models.MyDbContext())
            {
                try
                {
                    int CartItemCount = CartItemUpdates.Count();
                    List<Models.CartItem> myCart = GetCartItems();
                    foreach (var cartItem in myCart)
                    {
                        // Iterate through all rows within shopping cart list
                        for (int i = 0; i < CartItemCount; i++)
                        {
                            if (cartItem.Product.ProductID == CartItemUpdates[i].ProductId)
                            {
                                if (CartItemUpdates[i].PurchaseQuantity < 1 || CartItemUpdates[i].RemoveItem)
                                {
                                    RemoveItem(cartId, cartItem.ProductId);
                                }
                                else
                                {
                                    UpdateItem(cartId, cartItem.ProductId, CartItemUpdates[i].PurchaseQuantity);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("ERROR: Unable to Update Cart Database - " + ex.Message.ToString(), ex);
                }
            }
        }

        public void RemoveItem(string removeCartID, int removeProductID)
        {
            using (var db = new Models.MyDbContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems
                                  where c.CartId == removeCartID && c.Product.ProductID == removeProductID
                                  select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        db.ShoppingCartItems.Remove(myItem);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("ERROR: Unable to Remove Cart Item - " + ex.Message.ToString(), ex);
                }
            }
        }

        public void UpdateItem(string updateCartID, int updateProductID, int quantity)
        {
            using (var db = new Models.MyDbContext())
            {
                try
                {
                    var myItem = (from c in db.ShoppingCartItems
                                  where c.CartId == updateCartID && c.Product.ProductID == updateProductID
                                  select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        myItem.Quantity = quantity;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("ERROR: Unable to Update Cart Item - " + ex.Message.ToString(), ex);
                }
            }
        }

        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            var cartItems = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId);
            foreach (var c in cartItems)
            {
                _db.ShoppingCartItems.Remove(c);
            }
            _db.SaveChanges();
        }

        public int GetCount()
        {
            ShoppingCartId = GetCartId();
            // Get the count of each item in the cart and sum them up
            return (from i in _db.ShoppingCartItems
                    where i.CartId == ShoppingCartId
                    select (int?)i.Quantity).Sum() ?? 0;
        }

        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
    }
}