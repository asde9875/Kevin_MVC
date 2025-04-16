using Kevin.Models.Entities;
using System.Collections.Generic;

namespace Kevin.DataAccess.DAO
{
    public interface IApplicationUserDao
    {
        //void AddCompany(CompanyEntity company);

        void UpdateApplicationUser(ApplicationUser applicationUser);
        void DeleteApplicationUser(int id);
        IEnumerable<ApplicationUser> GetAllApplicationUsers();
        ApplicationUser GetApplicationUserById(int id);
    }
}
