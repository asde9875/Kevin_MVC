using Kevin.Models.Entities;
using Kevin.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.DataAccess.DAO
{
    public class ApplicationUserDao : IApplicationUserDao
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserDao(ApplicationDbContext db)
        {
            _db = db;
        }

        //public void AddCompany(CompanyEntity company)
        //{
        //    string sql = "INSERT INTO Companies (Name, City, State, StreetAddress, PostalCode, PhoneNumber) VALUES (@Name, @City, @State, @StreetAddress, @PostalCode, @PhoneNumber)";
        //    _db.Database.ExecuteSqlRaw(sql,
        //        new SqlParameter("@Name", company.Name),
        //        new SqlParameter("@City", company.City),
        //        new SqlParameter("@State", company.State),
        //        new SqlParameter("@StreetAddress", company.StreetAddress),
        //        new SqlParameter("@PostalCode", company.PostalCode),
        //        new SqlParameter("@PhoneNumber", company.PhoneNumber));
        //}

        public void UpdateApplicationUser(ApplicationUser applicationUser)
        {
            string sql = "UPDATE AspNetUsers SET LockoutEnd = @LockoutEnd WHERE Id = @Id";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@LockoutEnd", applicationUser.LockoutEnd),
                new SqlParameter("@Id", applicationUser.Id));
        }

        public void DeleteApplicationUser(int id)
        {
            string sql = "DELETE FROM AspNetUsers WHERE Id = @Id";
            _db.Database.ExecuteSqlRaw(sql,
                new SqlParameter("@Id", id));
        }

        public IEnumerable<ApplicationUser> GetAllApplicationUsers()
        {
            string sql = "SELECT * FROM AspNetUsers";

            var users = _db.ApplicationUsers.FromSqlRaw(sql).Include(u => u.Company).ToList();

            var userRoless = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in users)
            {
                var roleId = userRoless.FirstOrDefault(ur => ur.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId)?.Name;
                if (user.Company == null)
                {
                    user.Company = new() { Name = "" };
                }
            }

            return users;
        }

        public ApplicationUser GetApplicationUserById(int id)
        {
            string sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return _db.ApplicationUsers.FromSqlRaw(sql,
                new SqlParameter("@Id", id)).FirstOrDefault()!;
        }
    }
}
