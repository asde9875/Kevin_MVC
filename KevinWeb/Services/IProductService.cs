using Kevin.Models.Entities;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public interface IProductService
    {
        void AddProduct(ProductsEntity product);
        void UpdateProduct(ProductsEntity product);
        void DeleteProduct(int id);
        IEnumerable<ProductsEntity> GetAllProducts();
        ProductsEntity GetProductById(int id);
        IEnumerable<CategoryEntity> GetAllCategoriesName();
        void UpdateProductImage(ProductImage productImage);
        void AddProductImage(ProductImage productImage);
        void DeleteProductImageById(int id);
    }
}
