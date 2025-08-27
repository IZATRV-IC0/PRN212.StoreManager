using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Windows;

namespace StoreManager
{
    /// <summary>
    /// Interaction logic for ManageProductWindow.xaml
    /// </summary>
    public partial class ManageProductWindow : Window
    {
        ProductService _productService;
        public Product productEdit { get; set; } = null;

        public ManageProductWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
            
            if (productEdit != null)
            {
                FillProductForm(productEdit);
                txtProductID.IsEnabled = false; // disable editing ProductID
            }
            else
            {
                pnlProductID.Visibility = Visibility.Hidden;
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categoryService = new CategoryService();
                var categories = categoryService.GetAllCategories();
                cboCategory.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_CancelProduct_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                MessageBox.Show("Please enter product name.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void FillProductForm(Product product)
        {
            productEdit = product;
            txtProductID.Text = product.ProductId.ToString();
            txtProductName.Text = product.ProductName;
            txtSupplierID.Text = product.SupplierId?.ToString();
            cboCategory.SelectedValue = product.CategoryId;
            txtQuantityPerUnit.Text = product.QuantityPerUnit;
            txtUnitPrice.Text = product.UnitPrice?.ToString();
            txtUnitsInStock.Text = product.UnitsInStock?.ToString();
            txtUnitsOnOrder.Text = product.UnitsOnOrder?.ToString();
            txtReorderLevel.Text = product.ReorderLevel?.ToString();
            chkDiscontinued.IsChecked = product.Discontinued;
        }

        private void btn_SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            _productService = new ProductService();

            if (productEdit != null)
            {
                // Update existing product
                productEdit.ProductName = txtProductName.Text.Trim();
                productEdit.SupplierId = int.TryParse(txtSupplierID.Text, out int supplier) ? supplier : null;
                productEdit.CategoryId = cboCategory.SelectedValue != null ? (int)cboCategory.SelectedValue : null;
                productEdit.QuantityPerUnit = txtQuantityPerUnit.Text.Trim();
                productEdit.UnitPrice = decimal.TryParse(txtUnitPrice.Text, out decimal price) ? price : null;
                productEdit.UnitsInStock = int.TryParse(txtUnitsInStock.Text, out int stock) ? stock : null;
                productEdit.UnitsOnOrder = int.TryParse(txtUnitsOnOrder.Text, out int order) ? order : null;
                productEdit.ReorderLevel = int.TryParse(txtReorderLevel.Text, out int reorder) ? reorder : null;
                productEdit.Discontinued = chkDiscontinued.IsChecked ?? false;

                _productService.UpdateProduct(productEdit);
                MessageBox.Show("Product updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Create new product
                Product newProduct = new Product
                {
                    ProductName = txtProductName.Text.Trim(),
                    SupplierId = int.TryParse(txtSupplierID.Text, out int supplier) ? supplier : null,
                    CategoryId = cboCategory.SelectedValue != null ? (int)cboCategory.SelectedValue : null,
                    QuantityPerUnit = txtQuantityPerUnit.Text.Trim(),
                    UnitPrice = decimal.TryParse(txtUnitPrice.Text, out decimal price) ? price : null,
                    UnitsInStock = int.TryParse(txtUnitsInStock.Text, out int stock) ? stock : null,
                    UnitsOnOrder = int.TryParse(txtUnitsOnOrder.Text, out int order) ? order : null,
                    ReorderLevel = int.TryParse(txtReorderLevel.Text, out int reorder) ? reorder : null,
                    Discontinued = chkDiscontinued.IsChecked ?? false
                };

                _productService.AddProduct(newProduct);
                MessageBox.Show("Product saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Close();
        }
    }
}
