using StoreManagement.BLL.Services;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProductService _service = new ProductService();
            var products = _service.GetAllProducts();
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, Price: {product.ProductName}");
            }
        }
    }
}
