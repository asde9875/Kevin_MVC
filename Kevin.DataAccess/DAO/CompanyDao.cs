using Kevin.Models.Entities;
using Kevin.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.DataAccess.DAO
{
    public class CompanyDao : ICompanyDao
    {
        private readonly ApplicationDbContext _db;

        public CompanyDao(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddCompany(CompanyEntity company)
        {
            string sql = "INSERT INTO Companies (Name, City, State, StreetAddress, PostalCode, PhoneNumber) VALUES (@Name, @City, @State, @StreetAddress, @PostalCode, @PhoneNumber)";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Name", company.Name),
                new SqlParameter("@City", company.City),
                new SqlParameter("@State", company.State),
                new SqlParameter("@StreetAddress", company.StreetAddress),
                new SqlParameter("@PostalCode", company.PostalCode),
                new SqlParameter("@PhoneNumber", company.PhoneNumber));
        }

        public void UpdateCompany(CompanyEntity company)
        {
            string sql = "UPDATE Companies SET Name = @Name,  City= @City,  State= @State,  StreetAddress= @StreetAddress,  PostalCode= @PostalCode,  PhoneNumber= @PhoneNumber WHERE Id = @Id";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Name", company.Name),
                new SqlParameter("@City", company.City),
                new SqlParameter("@State", company.State),
                new SqlParameter("@StreetAddress", company.StreetAddress),
                new SqlParameter("@PostalCode", company.PostalCode),
                new SqlParameter("@PhoneNumber", company.PhoneNumber),
                new SqlParameter("@Id", company.Id));
        }

        public void DeleteCompany(int id)
        {
            string sql = "DELETE FROM Companies WHERE Id = @Id";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Id", id));
        }

        public IEnumerable<CompanyEntity> GetAllCompanies()
        {
            string sql = "SELECT * FROM Companies";
            return _db.Companies.FromSqlRaw(sql).ToList();
        }

        public CompanyEntity GetCompanyById(int id)
        {
            string sql = "SELECT * FROM Companies WHERE Id = @Id";
            return _db.Companies.FromSqlRaw(sql,
                new SqlParameter("@Id", id)).FirstOrDefault()!;
        }
    }
}
