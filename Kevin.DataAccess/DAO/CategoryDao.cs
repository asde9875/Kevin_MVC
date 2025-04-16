using Kevin.Models.Entities;
using Kevin.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.DataAccess.DAO
{
    public class CategoryDao : ICategoryDao
    {
        private readonly ApplicationDbContext _db;

        public CategoryDao(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddCategory(CategoryEntity category)
        {
            string sql = "INSERT INTO Categories (Name, DisplayOrder) VALUES (@Name, @DisplayOrder)";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@DisplayOrder", category.DisplayOrder));
        }

        public void UpdateCategory(CategoryEntity category)
        {
            string sql = "UPDATE Categories SET Name = @Name, DisplayOrder = @DisplayOrder WHERE Id = @Id";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@DisplayOrder", category.DisplayOrder),
                new SqlParameter("@Id", category.Id));
        }

        public void DeleteCategory(int id)
        {
            string sql = "DELETE FROM Categories WHERE Id = @Id";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Id", id));
        }

        public IEnumerable<CategoryEntity> GetAllCategories()
        {
            string sql = "SELECT * FROM Categories";
            return _db.Categories.FromSqlRaw(sql).ToList();
        }

        public CategoryEntity GetCategoryById(int id)
        {
            string sql = "SELECT * FROM Categories WHERE Id = @Id";
            return _db.Categories.FromSqlRaw(sql, 
                new SqlParameter("@Id", id)).FirstOrDefault()!;
        }

    }
}
