using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for ProductListWindow.xaml
    /// </summary>
    public partial class ProductListWindow : Window
    {
        private Customer _customer;
        private ProductService _productService;
        private CategoryService _categoryService;
        private List<Product> _allProducts = new List<Product>();
        private List<Product> _cartProducts = new List<Product>();

        public ProductListWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
            _productService = new ProductService();
            _categoryService = new CategoryService();
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
                foreach (var category in categories)
                {
                    cmbCategory.Items.Add(new ComboBoxItem { Content = category.CategoryName, Tag = category.CategoryId });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                _allProducts = _productService.GetAllProducts();
                dgProducts.ItemsSource = _allProducts;
                lblTotalProducts.Text = _allProducts.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            var selectedCategory = cmbCategory.SelectedItem as ComboBoxItem;
            
            var filteredProducts = _allProducts.AsEnumerable();

            // Filter by search text
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.ProductName.ToLower().Contains(searchText) ||
                    (p.QuantityPerUnit != null && p.QuantityPerUnit.ToLower().Contains(searchText)));
            }

            // Filter by category
            if (selectedCategory != null && selectedCategory.Tag != null)
            {
                int categoryId = (int)selectedCategory.Tag;
                filteredProducts = filteredProducts.Where(p => p.CategoryId == categoryId);
            }

            dgProducts.ItemsSource = filteredProducts.ToList();
            lblTotalProducts.Text = filteredProducts.Count().ToString();
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int productId = (int)button.Tag;
                var product = _allProducts.FirstOrDefault(p => p.ProductId == productId);
                
                if (product != null)
                {
                    if (product.UnitsInStock > 0 && !product.Discontinued)
                    {
                        // Add to shopping cart service
                        ShoppingCartService.Instance.AddToCartWithNotification(product, 1);
                        
                        MessageBox.Show($"{product.ProductName} has been added to your cart!\n" +
                                      $"Cart now has {ShoppingCartService.Instance.GetTotalItems()} items.", 
                                      "Added to Cart", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("This product is currently unavailable.", "Unavailable", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
