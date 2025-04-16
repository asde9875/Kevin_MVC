using Kevin.Models.Entities;
using Kevin.DataAccess.DAO;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IApplicationUserDao _applicationUserDao;

        public ApplicationUserService(IApplicationUserDao applicationUserDao)
        {
            _applicationUserDao = applicationUserDao;
        }

        //public void AddCompany(CompanyEntity company)
        //{
        //    _companyDao.AddCompany(company);
        //}


        public void UpdateApplicationUser(ApplicationUser applicationUser)
        {
            _applicationUserDao.UpdateApplicationUser(applicationUser);
        }

        public void DeleteApplicationUser(int id)
        {
            _applicationUserDao.DeleteApplicationUser(id);
        }

        public IEnumerable<ApplicationUser> GetAllApplicationUsers()
        {
            return _applicationUserDao.GetAllApplicationUsers();
        }

        public ApplicationUser GetApplicationUserById(int id)
        {
            return _applicationUserDao.GetApplicationUserById(id);
        }
    }
}
