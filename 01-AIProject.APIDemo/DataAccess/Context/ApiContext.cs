using _01_AIProject.APIDemo.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace _01_AIProject.APIDemo.DataAccess.Context
{
    public class ApiContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=TUNA\\SQLEXPRESS;initial catalog=ApiAIDb; integrated security=true;trustservercertificate=true");
        }

        public DbSet<Customer> Customers { get; set; }
    }

}
