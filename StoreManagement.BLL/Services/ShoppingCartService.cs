using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class ShoppingCartService
    {
        private static ShoppingCartService? _instance;
        private static readonly object _lock = new object();
        private readonly List<CartItem> _cartItems;

        private ShoppingCartService()
        {
            _cartItems = new List<CartItem>();
        }

        public static ShoppingCartService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ShoppingCartService();
                        }
                    }
                }
                return _instance;
            }
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.ProductId == product.ProductId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice ?? 0,
                    Quantity = quantity,
                    Product = product
                });
            }
        }

        public void RemoveFromCart(int productId)
        {
            var item = _cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (item != null)
            {
                _cartItems.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int newQuantity)
        {
            var item = _cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (item != null)
            {
                if (newQuantity <= 0)
                {
                    RemoveFromCart(productId);
                }
                else
                {
                    item.Quantity = newQuantity;
                }
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }

        public List<CartItem> GetCartItems()
        {
            return _cartItems.ToList();
        }

        public int GetTotalItems()
        {
            return _cartItems.Sum(item => item.Quantity);
        }

        public decimal GetTotalAmount()
        {
            return _cartItems.Sum(item => item.Subtotal);
        }

        public bool IsEmpty()
        {
            return _cartItems.Count == 0;
        }

        // Event để notify khi cart thay đổi
        public event EventHandler? CartChanged;

        private void OnCartChanged()
        {
            CartChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddToCartWithNotification(Product product, int quantity = 1)
        {
            AddToCart(product, quantity);
            OnCartChanged();
        }

        public void RemoveFromCartWithNotification(int productId)
        {
            RemoveFromCart(productId);
            OnCartChanged();
        }

        public void ClearCartWithNotification()
        {
            ClearCart();
            OnCartChanged();
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
        public Product? Product { get; set; }
    }
}
