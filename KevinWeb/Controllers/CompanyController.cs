using Kevin.DataAccess.Data;
using Kevin.Models.Entities;
using Kevin.Models;
using KevinWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Kevin.Utility;
using Microsoft.AspNetCore.Hosting;


namespace Kevin.Models.Controllers
{
    [Authorize(Roles = SD.Role_Admin)] //Avoid someone using URL to access the Content Management
    public class CompanyController : Controller
    {

        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var obj = _companyService.GetAllCompanies();
            return View(obj);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CompanyEntity obj)
        {

            // Validate the input object
            if (ModelState.IsValid)
            {
                _companyService.AddCompany(obj);
                TempData["success"] = "Company created successfully";
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

            var company = _companyService.GetCompanyById(id); // Use the service layer

            if (company == null)
            {
                return NotFound();
            }

            return View(company); // Pass the result to the view

        }

        [HttpPost]
        public IActionResult Edit(CompanyEntity obj)
        {

            // Validate the input object
            if (ModelState.IsValid)
            {
                _companyService.UpdateCompany(obj);
                TempData["success"] = "Company updated successfully";
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

            var company = _companyService.GetCompanyById(id); // Use the service layer

            if (company == null)
            {
                return NotFound();
            }

            return View(company); // Pass the result to the view

        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            _companyService.DeleteCompany(id);
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {

            var obj = _companyService.GetAllCompanies();
            return Json(new { data = obj });
        }

        [HttpDelete]
        public IActionResult QuickDelete(int id)
        {
            _companyService.DeleteCompany(id);
            return Json(new { success = true, message = "Delete Successfully" });
        }

        #endregion


    }
}
