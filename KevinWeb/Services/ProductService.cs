using Kevin.Models.Entities;
using Kevin.DataAccess.DAO;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductDao _productDao;

        public ProductService(IProductDao productDao)
        {
            _productDao = productDao;
        }

        public void AddProduct(ProductsEntity product)
        {
            _productDao.AddProduct(product);
        }

        public void UpdateProduct(ProductsEntity product)
        {
            _productDao.UpdateProduct(product);
        }

        public void DeleteProduct(int id)
        {
            _productDao.DeleteProduct(id);
        }

        public IEnumerable<ProductsEntity> GetAllProducts()
        {
            return _productDao.GetAllProducts();
        }

        public ProductsEntity GetProductById(int id)
        {
            return _productDao.GetProductById(id);
        }
        public IEnumerable<CategoryEntity> GetAllCategoriesName()
        {
            return _productDao.GetAllCategoriesName();
        }
        public void UpdateProductImage(ProductImage productImage)
        {
            _productDao.UpdateProductImage(productImage);
        }
        public void AddProductImage(ProductImage productImage) {
            _productDao.AddProductImage(productImage);
        }
        public void DeleteProductImageById(int id)
        {
            _productDao.DeleteProductImageById(id);
        }
    }
}
