using Microsoft.EntityFrameworkCore;
using NetCoreAI_Project01_ApiDemo.Entities;

namespace NetCoreAI_Project01_ApiDemo.Context
{
    public class ApiContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-62QMFUD\\SQLEXPRESS; initial catalog=ApiAIDb; integrated security=true;trustservercertificate=true");
        }
        public DbSet<Customer> Customers { get; set; }
    }
}
