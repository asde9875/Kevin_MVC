using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Kevin.DataAccess.Data;
using Kevin.Models;
using Kevin.Models.Entities;
using Kevin.Utility;
using KevinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KevinWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _ShoppingCartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService ShoppingCartService)
        {
            _logger = logger;
            _productService = productService;
            _ShoppingCartService = ShoppingCartService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var obj = _productService.GetAllProducts();
            return View(obj);
        }

        [HttpGet]
        public IActionResult Details(int productId)
        {
            var cart = new ShoppingCartEntity
            {
                Product = _productService.GetProductById(productId), // Fetch and assign the product
                Count = 1, // Default quantity
                ProductId = productId
            };

            // Populate the category list (if needed for dropdowns in the view)
            var categoryList = _productService.GetAllCategoriesName()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = u.Id == cart.Product.CategoryId // Set the selected category
                });

            return View(cart); // Pass ShoppingCartEntity to the view
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCartEntity shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            // Check if the shopping cart already exists for this user and product
            var cartFromDb = _ShoppingCartService.GetAllShoppingCarts()
                .FirstOrDefault(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                // If the shopping cart exists, increment the count
                cartFromDb.Count += shoppingCart.Count;
                _ShoppingCartService.UpdateShoppingCart(cartFromDb); // Update the shopping cart
            }
            else
            {
                // If it doesn't exist, add a new shopping cart
                _ShoppingCartService.AddShoppingCart(shoppingCart);
                //HttpContext.Session.SetInt32(SD.SessionCart, 
                //    _ShoppingCartService.GetAllShoppingCarts()
                //    .Where(u => u.ApplicationUserId == userId).ToList().Count);
            }
            TempData["Success"] = "Cart updated successfully";

            //return RedirectToAction("Index");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()

        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
