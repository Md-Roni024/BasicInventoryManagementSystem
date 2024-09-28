//using BasicInventoryManagementSystem.Models;
//using Microsoft.EntityFrameworkCore;

//namespace BasicInventoryManagementSystem.Data
//{
//    public class ApplicationDbContext : DbContext
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
//        {

//        }
//        public DbSet<Product> Products { get; set; }
//        public DbSet<User> Users { get; set; }
//        public DbSet<Sale> Sales { get; set; }

//        public DbSet<Purchase> Purchases { get; set; }
//    }
//}


using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BasicInventoryManagementSystem.Models;

namespace BasicInventoryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Add any custom configurations here
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
    }
}
