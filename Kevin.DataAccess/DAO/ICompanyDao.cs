using Kevin.Models.Entities;
using System.Collections.Generic;

namespace Kevin.DataAccess.DAO
{
    public interface ICompanyDao
    {
        void AddCompany(CompanyEntity company);
        void UpdateCompany(CompanyEntity company);
        void DeleteCompany(int id);
        IEnumerable<CompanyEntity> GetAllCompanies();
        CompanyEntity GetCompanyById(int id);
    }
}
