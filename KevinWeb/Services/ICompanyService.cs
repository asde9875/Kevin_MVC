using Kevin.Models.Entities;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public interface ICompanyService
    {
        void AddCompany(CompanyEntity company);
        void UpdateCompany(CompanyEntity company);
        void DeleteCompany(int id);
        IEnumerable<CompanyEntity> GetAllCompanies();
        CompanyEntity GetCompanyById(int id);
    }
}
