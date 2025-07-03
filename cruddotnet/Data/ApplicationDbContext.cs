using cruddotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cruddotnet.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
        {

        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Spending> Spendings { get; set; }
    }
}
 