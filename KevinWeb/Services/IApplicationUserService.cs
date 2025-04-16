using Kevin.Models.Entities;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public interface IApplicationUserService
    {
        //void AddCompany(CompanyEntity company);
        void UpdateApplicationUser(ApplicationUser applicationUser);
        void DeleteApplicationUser(int id);
        IEnumerable<ApplicationUser> GetAllApplicationUsers();
        ApplicationUser GetApplicationUserById(int id);
    }
}
