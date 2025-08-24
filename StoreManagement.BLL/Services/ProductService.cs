using StoreManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.BLL.Services
{
    public class ProductService
    {
        ProductRepository _productRepository;
        public ProductService()
        {
            _productRepository = new ProductRepository();
        }
        public List<StoreManagement.DAL.Entities.Product> GetAllProducts()
        {
            if (_productRepository == null)
            {
                throw new InvalidOperationException("Product repository is not initialized or whether the database is not connected.");
            }
            return _productRepository.GetAll();
        }
        public List<StoreManagement.DAL.Entities.Product> SearchProductsByNameAndCategory(string name, string categoryName)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("Both search parameters cannot be null or empty");
            }
            return _productRepository.SearchProductByNameAndCategory(name, categoryName);
        }
        public void UpdateProduct(StoreManagement.DAL.Entities.Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }
            _productRepository.Update(product);
        }
        public void DeleteProduct(int productId)
        {
            if (productId < 0)
            {
                throw new ArgumentException("Invalid product ID", nameof(productId));
            }
            _productRepository.Delete(productId);
        }
        public void AddProduct(StoreManagement.DAL.Entities.Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }
            _productRepository.Add(product);
        }
    }
}
