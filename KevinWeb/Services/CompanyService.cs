using Kevin.Models.Entities;
using Kevin.DataAccess.DAO;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyDao _companyDao;

        public CompanyService(ICompanyDao companyDao)
        {
            _companyDao = companyDao;
        }

        public void AddCompany(CompanyEntity company)
        {
            _companyDao.AddCompany(company);
        }


        public void UpdateCompany(CompanyEntity company)
        {
            _companyDao.UpdateCompany(company);
        }

        public void DeleteCompany(int id)
        {
            _companyDao.DeleteCompany(id);
        }

        public IEnumerable<CompanyEntity> GetAllCompanies()
        {
            return _companyDao.GetAllCompanies();
        }

        public CompanyEntity GetCompanyById(int id)
        {
            return _companyDao.GetCompanyById(id);
        }
    }
}
