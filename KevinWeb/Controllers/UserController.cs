using Kevin.DataAccess.Data;
using Kevin.Models.Entities;
using Kevin.Models;
using KevinWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Kevin.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;


namespace Kevin.Models.Controllers
{
    [Authorize(Roles = SD.Role_Admin)] //Avoid someone using URL to access the Content Management
    public class UserController : Controller
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly ApplicationDbContext _db;
        private UserManager<IdentityUser> _userMananger;
        public UserController(IApplicationUserService applicationUserService, ApplicationDbContext db, UserManager<IdentityUser> userMananger)
        {
            _applicationUserService = applicationUserService;
            _db = db;
            _userMananger = userMananger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var obj = _companyService.GetAllCompanies();
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _applicationUserService.GetAllApplicationUsers().FirstOrDefault(u => u.Id == userId),

                RoleList = _db.Roles
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),

                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // A role was updated
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

                _userMananger.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userMananger.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {

            var obj = _applicationUserService.GetAllApplicationUsers();
            return Json(new { data = obj });
        }

        [HttpDelete]
        public IActionResult QuickDelete(int id)
        {
            _applicationUserService.DeleteApplicationUser(id);
            return Json(new { success = true, message = "Delete Successfully" });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _applicationUserService.GetAllApplicationUsers().FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // User is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _applicationUserService.UpdateApplicationUser(objFromDb);

            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion

    }
}
