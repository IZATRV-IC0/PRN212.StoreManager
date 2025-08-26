using StoreManagement.DAL.Repositories;

namespace StoreManagement.BLL.Services
{
    public class CategoryService
    {
        CategoryRepository _categoryRepository;
        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
        }
        public List<StoreManagement.DAL.Entities.Category> GetAllCategories()
        {
            if (_categoryRepository == null)
            {
                throw new InvalidOperationException("Category repository is not initialized or whether the database is not connected.");
            }
            return _categoryRepository.GetAll();
        }
        public void AddCategory(StoreManagement.DAL.Entities.Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null");
            }
            _categoryRepository.Add(category);
        }
        public void UpdateCategory(StoreManagement.DAL.Entities.Category category)
        {
            if(category == null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null");
            }
            _categoryRepository.Update(category);
        }
        public bool DeleteCategory(int categoryId)
        {
            if (categoryId < 0)
            {
                throw new ArgumentException("Invalid category ID", nameof(categoryId));
            }
            if (_categoryRepository.Delete(categoryId))
            {
                return true;
            }
            return false;
        }
    }
}
