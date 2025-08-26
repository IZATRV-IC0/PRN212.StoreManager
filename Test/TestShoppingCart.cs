using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;

namespace Test
{
    public class TestShoppingCart
{
    public static void TestCartFunctionality()
    {
        Console.WriteLine("=== Testing Shopping Cart Functionality ===");
        
        // Create test product
        var product1 = new Product
        {
            ProductId = 1,
            ProductName = "Test Product 1",
            UnitPrice = 10.99m,
            UnitsInStock = 100,
            Discontinued = false
        };
        
        var product2 = new Product
        {
            ProductId = 2,
            ProductName = "Test Product 2",
            UnitPrice = 25.50m,
            UnitsInStock = 50,
            Discontinued = false
        };
        
        var cart = ShoppingCartService.Instance;
        
        // Test adding products
        Console.WriteLine("Adding products to cart...");
        cart.AddToCart(product1, 2);
        cart.AddToCart(product2, 1);
        
        Console.WriteLine($"Total items in cart: {cart.GetTotalItems()}");
        Console.WriteLine($"Total amount: ${cart.GetTotalAmount():F2}");
        
        // Test cart items
        var items = cart.GetCartItems();
        Console.WriteLine("\nCart contents:");
        foreach (var item in items)
        {
            Console.WriteLine($"- {item.ProductName}: {item.Quantity} x ${item.UnitPrice:F2} = ${item.Subtotal:F2}");
        }
        
        // Test removing item
        Console.WriteLine("\nRemoving product 1...");
        cart.RemoveFromCart(1);
        
        Console.WriteLine($"Total items after removal: {cart.GetTotalItems()}");
        Console.WriteLine($"Total amount after removal: ${cart.GetTotalAmount():F2}");
        
        // Test clearing cart
        Console.WriteLine("\nClearing cart...");
        cart.ClearCart();
        
        Console.WriteLine($"Cart is empty: {cart.IsEmpty()}");
        Console.WriteLine($"Total items: {cart.GetTotalItems()}");
        
        Console.WriteLine("=== Cart functionality test completed ===\n");
    }
    }
}
