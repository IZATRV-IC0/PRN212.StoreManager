using StoreManagement.BLL.Services;
using StoreManagement.DAL.Entities;
using System;
using System.Windows;

namespace StoreManager
{
    public partial class ManageCategoryWindow : Window
    {
        CategoryService _categoryService;
        public Category categoryEdit { get; set; } = null;

        public ManageCategoryWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (categoryEdit != null)
            {
                FillCategoryForm(categoryEdit);
                txtCategoryID.IsEnabled = false; // disable editing ID
            }
            else
            {
                lblCategoryID.Visibility = Visibility.Hidden;
                txtCategoryID.Visibility = Visibility.Hidden;
            }
        }

        private void btn_CancelCategory_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtCategoryName.Text))
            {
                MessageBox.Show("Please fill in required fields (Category Name).",
                                "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public void FillCategoryForm(Category category)
        {
            categoryEdit = category;
            txtCategoryID.Text = category.CategoryId.ToString();
            txtCategoryName.Text = category.CategoryName;
            txtDescription.Text = category.Description;
        }

        private void btn_SaveCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            _categoryService = new CategoryService();

            if (categoryEdit != null)
            {
                // Update
                categoryEdit.CategoryName = txtCategoryName.Text.Trim();
                categoryEdit.Description = txtDescription.Text.Trim();

                _categoryService.UpdateCategory(categoryEdit);
                MessageBox.Show("Category updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Create
                Category newCategory = new Category
                {
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                _categoryService.AddCategory(newCategory);
                MessageBox.Show("Category saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            this.Close();
        }
    }
}
