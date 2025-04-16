using Kevin.Models.Entities;
using Kevin.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.DataAccess.DAO
{
    public class ProductDao : IProductDao
    {
        private readonly ApplicationDbContext _db;

        public ProductDao(ApplicationDbContext db)
        {
            _db = db;
            //_db.Products.Include(u => u.Category).Include(u =>u.CategoryId);
        }

        // Add a product using raw SQL and SaveChanges with transaction
        public void AddProduct(ProductsEntity product)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                // Raw SQL for inserting the product
                string sql = "INSERT INTO Products (Title, Description, ISBN, Author, ListPrice, Price, Price50, Price100, CategoryId, ImageUrl) " +
                             "VALUES (@Title, @Description, @ISBN, @Author, @ListPrice, @Price, @Price50, @Price100, @CategoryId, @ImageUrl)";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@Title", product.Title),
                    new SqlParameter("@Description", product.Description),
                    new SqlParameter("@ISBN", product.ISBN),
                    new SqlParameter("@Author", product.Author),
                    new SqlParameter("@ListPrice", product.ListPrice),
                    new SqlParameter("@Price", product.Price),
                    new SqlParameter("@Price50", product.Price50),
                    new SqlParameter("@Price100", product.Price100),
                    new SqlParameter("@CategoryId", product.CategoryId),
                    new SqlParameter("@ImageUrl", product.ImageUrl));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Update a product using raw SQL and SaveChanges with transaction
        public void UpdateProduct(ProductsEntity product)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = "UPDATE Products SET " +
                             "Title = @Title, " +
                             "Description = @Description, " +
                             "ISBN = @ISBN, " +
                             "Author = @Author, " +
                             "ListPrice = @ListPrice, " +
                             "Price = @Price, " +
                             "Price50 = @Price50, " +
                             "Price100 = @Price100, " +
                             "CategoryId = @CategoryId, " +
                             "ImageUrl = @ImageUrl " +
                             "WHERE Id = @Id";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@Title", product.Title),
                    new SqlParameter("@Description", product.Description),
                    new SqlParameter("@ISBN", product.ISBN),
                    new SqlParameter("@Author", product.Author),
                    new SqlParameter("@ListPrice", product.ListPrice),
                    new SqlParameter("@Price", product.Price),
                    new SqlParameter("@Price50", product.Price50),
                    new SqlParameter("@Price100", product.Price100),
                    new SqlParameter("@CategoryId", product.CategoryId),
                    new SqlParameter("@ImageUrl", product.ImageUrl),
                    new SqlParameter("@Id", product.Id));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Delete a product by ID using raw SQL and SaveChanges with transaction
        public void DeleteProduct(int id)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = "DELETE FROM Products WHERE Id = @Id";
                _db.Database.ExecuteSqlRaw(sql, new SqlParameter("@Id", id));

                _db.ProductImages.RemoveRange(_db.ProductImages.Where(p => p.ProductId == id));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        //Get Category name -->  _db.Products.Include(u => u.Category).Include(u =>u.CategoryId);
        public IEnumerable<ProductsEntity> GetAllProducts()
        {
            //string sql = "SELECT * FROM Products";
            //return _db.Products.FromSqlRaw(sql).ToList();


            string sql = "SELECT * FROM Products";
            var products = _db.Products.FromSqlRaw(sql).ToList();

            // Load related Category data for each product
            foreach (var product in products)
            {
                _db.Entry(product).Reference(p => p.Category).Load();
                _db.Entry(product).Collection(p => p.ProductImages).Load();
            }

            return products;
        }

        public ProductsEntity GetProductById(int id)
        {
            string sql = "SELECT * FROM Products WHERE Id = @Id";
            var product = _db.Products.FromSqlRaw(sql,
                new SqlParameter("@Id", id)).FirstOrDefault()!;

            _db.Entry(product).Collection(p => p.ProductImages).Load();


            return product;
        }

        public IEnumerable<CategoryEntity> GetAllCategoriesName()
        {
            string sql = "SELECT * FROM Categories";
            return _db.Categories.FromSqlRaw(sql).ToList();
        }

        
        public void UpdateProductImage(ProductImage productImage)
        {

            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                _db.ProductImages.Update(productImage);

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void AddProductImage(ProductImage productImage)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                _db.ProductImages.Add(productImage);

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteProductImageById(int id)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = "DELETE FROM ProductImages WHERE Id = @Id";
                _db.Database.ExecuteSqlRaw(sql, new SqlParameter("@Id", id));
                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();
                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
