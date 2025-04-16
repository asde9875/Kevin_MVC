using Kevin.DataAccess.Data;
using Kevin.Models.Entities;
using Kevin.Models;
using KevinWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Kevin.Utility;


namespace Kevin.Models.Controllers
{
    [Authorize(Roles = SD.Role_Admin)] //Avoid someone using URL to access the Content Management
    public class CategoryController : Controller
    {

        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var obj = _categoryService.GetAllCategories();
            return View(obj);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryEntity obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "the DisplayOrder cannot exactly match the name.");
            }

            // Validate the input object
            if (ModelState.IsValid)
            { 
                _categoryService.AddCategory(obj);
                TempData["success"] = "Category created successfully";
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

            var category = _categoryService.GetCategoryById(id); // Use the service layer

            if (category == null)
            {
                return NotFound();
            }

            return View(category); // Pass the result to the view

        }

        [HttpPost]
        public IActionResult Edit(CategoryEntity obj)
        {

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "the DisplayOrder cannot exactly match the name.");
            }

            // Validate the input object
            if (ModelState.IsValid)
            {
                _categoryService.UpdateCategory(obj);
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
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

            var category = _categoryService.GetCategoryById(id); // Use the service layer

            if (category == null)
            {
                return NotFound();
            }

            return View(category); // Pass the result to the view

        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            _categoryService.DeleteCategory(id);
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
