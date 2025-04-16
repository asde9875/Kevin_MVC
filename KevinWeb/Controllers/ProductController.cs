using System.Linq;
using Kevin.DataAccess.Data;
using Kevin.Models.Entities;
using Kevin.Utility;

//using KevinWeb.Models;
using KevinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;


namespace Kevin.Models.Controllers
{
    [Authorize(Roles = SD.Role_Admin)] //Avoid someone using URL to access the Content Management
    public class ProductController : Controller
    {

        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;

        public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment, ApplicationDbContext db)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var obj = _productService.GetAllProducts();
            return View(obj);
        }

        [HttpGet]
        public IActionResult Create()
        {

            // Fetch category names and create a mapping of CategoryId to Name
            var categoryList = _productService.GetAllCategoriesName()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            //ViewBag.CategoryList = CategoryList;
            ViewData["CategoryList"] = categoryList;
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductsEntity objProduct, IFormFile? ImageUrl, ProductImage objImage, List<IFormFile> ProductImages)
        {
            ModelState.Remove("Product");
            ModelState.Remove("ImageUrl");

            // Validate the input object
            if (ModelState.IsValid)
            {

                // Products Cover upload
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(ImageUrl != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        ImageUrl.CopyTo(fileStream);
                    }
                    //obj.ImageUrl = $"/images/product/{fileName}";
                    objProduct.ImageUrl = @"\images\product\" + fileName;
                }
                else
                {
                    objProduct.ImageUrl = "";
                }

                _productService.AddProduct(objProduct);


                // Product Detail Images Upload
                int lastProductId = _db.Products.OrderByDescending(p => p.Id).Select(p => p.Id).FirstOrDefault();

                ProductImage productImage= new ProductImage();
                if (ProductImages != null && ProductImages.Any())
                {
                    string productImagePath = Path.Combine(wwwRootPath, @"images\products\product-" + lastProductId);

                    if (!Directory.Exists(productImagePath))
                    {
                        Directory.CreateDirectory(productImagePath);
                    }

                    foreach (var file in ProductImages)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        using (var fileStream = new FileStream(Path.Combine(productImagePath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        productImage = new ProductImage
                        {
                            ProductId = lastProductId,
                            ImageUrl = $@"\images\products\product-{lastProductId}\{fileName}"
                        };

                        if (objProduct.ProductImages == null)
                        {
                            objProduct.ProductImages = new List<ProductImage>();
                        }
                        _productService.AddProductImage(productImage);
                    }
                }

                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            if (id == 0) // id == 0 is sufficient since int cannot be null
            {
                return NotFound();
            }

            var product = _productService.GetProductById(id); // Use the service layer

            if (product == null)
            {
                return NotFound();
            }

            // Populate CategoryList with selection logic
            var categoryList = _productService.GetAllCategoriesName()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = u.Id == product.CategoryId // Set the selected category
                });

            ViewData["CategoryList"] = categoryList;

            return View(product); // Pass the result to the view

        }

       
        //ProductsEntity obj, IFormFile? file
        [HttpPost]
        public IActionResult Edit(ProductsEntity objProduct, IFormFile? ImageUrl, ProductImage objImage, List<IFormFile> ProductImages)
        {
            ModelState.Remove("Product");
            ModelState.Remove("ImageUrl");
            // Validate the input object
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Retrieve the original product from the database
                var existingProduct = _productService.GetProductById(objProduct.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (ImageUrl != null)
                {
                    string oldFilePath = Path.Combine(wwwRootPath, existingProduct.ImageUrl.TrimStart('\\')); // Convert URL to physical path
                    // Delete the original image if it exists
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl) && System.IO.File.Exists(oldFilePath))
                    {
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath); // Delete the file
                        }

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                        string productPath = Path.Combine(wwwRootPath, @"images\product");

                        using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                        {
                            ImageUrl.CopyTo(fileStream);
                        }
                        //obj.ImageUrl = $"/images/product/{fileName}";
                        objProduct.ImageUrl = @"\images\product\" + fileName;
                    }
                    else if(string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
                        string productPath = Path.Combine(wwwRootPath, @"images\product");

                        using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                        {
                            ImageUrl.CopyTo(fileStream);
                        }
                        //obj.ImageUrl = $"/images/product/{fileName}";
                        objProduct.ImageUrl = @"\images\product\" + fileName;
                    }
                }
                else
                {
                    // If no new file is uploaded, keep the existing image URL
                    objProduct.ImageUrl = existingProduct.ImageUrl;
                }

                _productService.UpdateProduct(objProduct);



                // Product Detail Images Upload
                ProductImage productImage = new ProductImage();
                if (ProductImages != null && ProductImages.Any())
                {
                    string productImagePath = Path.Combine(wwwRootPath, @"images\products\product-" + objProduct.Id);

                    if (!Directory.Exists(productImagePath))
                    {
                        Directory.CreateDirectory(productImagePath);
                    }

                    foreach (var file in ProductImages)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        using (var fileStream = new FileStream(Path.Combine(productImagePath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        productImage = new ProductImage
                        {
                            ProductId = objProduct.Id,
                            ImageUrl = $@"\images\products\product-{objProduct.Id}\{fileName}"
                        };

                        //if (objProduct.ProductImages == null)
                        //{
                        //    objProduct.ProductImages = new List<ProductImage>();
                        //}
                        _productService.AddProductImage(productImage);
                    }
                }


                TempData["success"] = "Product updated successfully";
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", new { id = objProduct.Id });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {

            if (id == 0) // id == 0 is sufficient since int cannot be null
            {
                return NotFound();
            }

            var category = _productService.GetProductById(id); // Use the service layer

            if (category == null)
            {
                return NotFound();
            }

            // Populate CategoryList with selection logic
            var categoryList = _productService.GetAllCategoriesName()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = u.Id == id // Set the selected category
                });

            ViewData["CategoryList"] = categoryList;

            return View(category); // Pass the result to the view

        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Retrieve the original product from the database
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            string oldFilePath = Path.Combine(wwwRootPath, existingProduct.ImageUrl.TrimStart('\\')); // Convert URL to physical path
                                                                                                      // Delete the original image if it exists
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl) && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath); // Delete the file
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            _productService.DeleteProduct(id);
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var obj = _productService.GetAllProducts();
            return Json(new { data = obj });
        }

        #endregion

        [HttpDelete]
        public IActionResult QuickDelete(int id)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Retrieve the original product from the database
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            string oldFilePath = Path.Combine(wwwRootPath, existingProduct.ImageUrl.TrimStart('\\')); // Convert URL to physical path
                                                                                                      // Delete the original image if it exists
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl) && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath); // Delete the file
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }
                Directory.Delete(finalPath);
            }

            _productService.DeleteProduct(id);
            return Json(new { success = true, message= "Delete Successfully" });
        }


        public IActionResult DeleteImage(int imageId)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            var imageToBeDeleted = _db.ProductImages.FirstOrDefault(p => p.Id == imageId);

            int productId = imageToBeDeleted.ProductId;

            string oldFilePath = Path.Combine(wwwRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\')); // Convert URL to physical path
                                                                                                      // Delete the original image if it exists
            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl) && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath); // Delete the file
            }

            _productService.DeleteProductImageById(imageId);

            TempData["success"] = "Product updated successfully";
            //return RedirectToAction("Index");
            return RedirectToAction("Edit", new { id = productId });
        }
    }
}
