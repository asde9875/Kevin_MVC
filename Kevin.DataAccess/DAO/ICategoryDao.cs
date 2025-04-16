using Kevin.Models.Entities;
using System.Collections.Generic;

namespace Kevin.DataAccess.DAO
{
    public interface ICategoryDao
    {
        void AddCategory(CategoryEntity category);
        void UpdateCategory(CategoryEntity category);
        void DeleteCategory(int id);
        IEnumerable<CategoryEntity> GetAllCategories();
        CategoryEntity GetCategoryById(int id);
    }
}
