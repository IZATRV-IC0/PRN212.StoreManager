using StoreManagement.BLL.Services;
using System.Linq;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Store Manager Test Console");
            Console.WriteLine("==========================");
            
            // Test Shopping Cart
            TestShoppingCart.TestCartFunctionality();
            
            // Test Product Service
            Console.WriteLine("=== Testing Product Service ===");
            ProductService _service = new ProductService();
            var products = _service.GetAllProducts();
            Console.WriteLine($"Total products: {products.Count}");
            
            foreach (var product in products.Take(5)) // Show first 5 products
            {
                Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.UnitPrice}");
            }
            
            Console.WriteLine("=== Test completed ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
