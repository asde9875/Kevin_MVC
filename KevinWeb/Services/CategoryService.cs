using Kevin.Models.Entities;
using Kevin.DataAccess.DAO;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDao _categoryDao;

        public CategoryService(ICategoryDao categoryDao)
        {
            _categoryDao = categoryDao;
        }

        public void AddCategory(CategoryEntity category)
        {
            _categoryDao.AddCategory(category);
        }

        public void UpdateCategory(CategoryEntity category)
        {
            _categoryDao.UpdateCategory(category);
        }

        public void DeleteCategory(int id)
        {
            _categoryDao.DeleteCategory(id);
        }

        public IEnumerable<CategoryEntity> GetAllCategories()
        {
            return _categoryDao.GetAllCategories();
        }

        public CategoryEntity GetCategoryById(int id)
        {
            return _categoryDao.GetCategoryById(id);
        }
    }
}
